using System.Collections.Generic;
using GameState;
using Impulses;
using UnityEngine;
using UnityEngine.Events;

namespace Dog
{
    [System.Serializable]
    public struct Command
    {
        public string phrases;
        [Range(0f, 100f)]
        public float rehearsed;
        public Activity.ActivitySO activity;
    }

    [System.Serializable]
    public struct Habit
    {
        [Range(0f, 100f)]
        public float pronouncedness;
        public CharacterTraitSO trait;
    }

    [System.Serializable]
    public struct ImprovableBehaviour
    {
        public ConditionSO educationalSource;
        public bool isGood;
    }

    public class Mind : MonoBehaviour
    {
        [System.Serializable]
        public struct ActivityInfo
        {
            public Activity.ActivitySO activity;
            public object argument;
            public UnityAction afterFinishAction;
            public ActivityInfo(Activity.ActivitySO activity, object argument)
            {
                this.activity = activity;
                this.argument = argument;
                this.afterFinishAction = null;
            }

            public ActivityInfo(Activity.ActivitySO currentActivitySO, object argument, UnityAction m_AfterFinishAction)
            {
                this.activity = currentActivitySO;
                this.argument = argument;
                this.afterFinishAction = m_AfterFinishAction;
            }

            public void ExecuteAfterFinishAction()
            {
                afterFinishAction?.Invoke();
            }

        }

        public DogState dogState;

        private ActivityInfo m_CurrentActivityInfo;
        private ActivityExecution m_CurrentActivity;

        private List<ActivityInfo> m_QueuedActivities;
        public List<Command> commands;
        public List<Habit> habits;
        // Activity that will execute after throwing
        public Activity.ActivitySO activityOnThrow;
        // Wait command
        public Activity.ActivitySO waitCommand;
        public Activity.ActivitySO sitCommand;
        public ImpulseSO idleImpulse;
        private RewardManager m_RewardManager;
        private TooltipStack m_TooltipStack;
        public ImprovableBehaviour lastImprovableBehaviour;

        private void Awake()
        {
            GlobalEvents.Instance.onDogImpulse.AddListener(OnImpulse);
            m_QueuedActivities = new List<ActivityInfo>();
            lastImprovableBehaviour.educationalSource = null;
        }
        private void OnImpulse(ImpulseSO impulse, object argument)
        {
            Debug.Log($"New impulse {impulse}");
            var activity = impulse.DecideActivity(this, argument);
            if (activity != null)
            {
                if (argument == null)
                    Debug.Log($"{gameObject.name} macht {activity.title}");
                else
                    Debug.Log($"{gameObject.name} macht {activity.title} mit {argument}");
                AddActivityInterruptingCurrent(activity, argument, null);
            }
        }
        private void Start()
        {
            m_RewardManager = RewardManager.instance;
            
            m_TooltipStack = FindObjectOfType<TooltipStack>();
            if (m_TooltipStack == null)
            {
                Debug.LogError("The TooltipStack is missing in the Scene!");
            }
        }
        private void OnActivityEnd(ActivityExecution activity)
        {
            activity.OnEnd();
            int points = activity.GetPoints();
            if (points != 0)
            {
                m_RewardManager.UpdateScore(points);
            }
            if (activity.GetActivitySO().rewardInfluence != 0.0f && lastImprovableBehaviour.educationalSource != null)
            {
                var edu_value = dogState.GetCondition(lastImprovableBehaviour.educationalSource);
                var rewardInfluence = activity.GetActivitySO().rewardInfluence;
                if (lastImprovableBehaviour.isGood)
                {
                    Debug.Log($"Dog improved on good behaviour on {lastImprovableBehaviour.educationalSource.title} at {rewardInfluence}");
                    edu_value += rewardInfluence;
                }
                else
                {
                    Debug.Log($"Dog improved on bad behaviour on {lastImprovableBehaviour.educationalSource.title} at {rewardInfluence}");
                    edu_value -= rewardInfluence;
                }
                dogState.SetCondition(lastImprovableBehaviour.educationalSource, edu_value);
                lastImprovableBehaviour.educationalSource = null;
            }
            if (activity.GetActivitySO().goodEducation)
            {
                lastImprovableBehaviour.isGood = true;
                lastImprovableBehaviour.educationalSource = activity.GetActivitySO().goodEducation;
            }
            if (activity.GetActivitySO().poorEducation)
            {
                lastImprovableBehaviour.isGood = false;
                lastImprovableBehaviour.educationalSource = activity.GetActivitySO().poorEducation;
            }
        }
        void Update()
        {
            if (m_CurrentActivity != null && m_CurrentActivity.IsReadyToEnd())
            {
                OnActivityEnd(m_CurrentActivity);
                m_CurrentActivityInfo.ExecuteAfterFinishAction();
                m_CurrentActivityInfo = default;
                var (impulse, argument) = m_CurrentActivity.GetImpulseOnEnd();
                m_CurrentActivity = null;
                if (m_QueuedActivities.Count == 0)
                    OnImpulse(impulse, argument);
            }
            // Select next activity
            if (m_CurrentActivity == null && m_QueuedActivities.Count == 0)
            {
                OnImpulse(idleImpulse, null);
            }
            // Start next activity
            if (m_CurrentActivity == null && m_QueuedActivities.Count > 0 && !m_QueuedActivities[0].Equals(default(ActivityInfo)))
            {
                var drainedActivityInfo = m_QueuedActivities[0];
                m_QueuedActivities.RemoveAt(0);
                if (drainedActivityInfo.activity != null)
                {
                    m_CurrentActivityInfo = drainedActivityInfo;
                    m_CurrentActivity = drainedActivityInfo.activity.StartActivity(this.gameObject, drainedActivityInfo.argument);
                }
            }
            if (m_CurrentActivity != null)
            {
                m_CurrentActivity.Update();
            }
            var currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            double timeDelta = ((double)(currentTime - dogState.lastConditionUpdate)) / 1000.0f;
            dogState.lastConditionUpdate = currentTime;
            SimulateIdling(Time.deltaTime / GameStateManager.Instance.gameState.simulationDayLength);
        }

        public void SimulateIdling(double simulatedTime)
        {

            foreach (var condition in dogState.conditions)
            {
                condition.value += condition.type.changePerDay * simulatedTime;
                condition.value = System.Math.Clamp(condition.value, condition.type.minimumValue, condition.type.maximumValue);
                int i = 0;
                foreach (ConditionRange range in condition.type.conditionRanges)
                {
                    if ((condition.activeRanges & (1 << i)) == 0)
                    {
                        if (condition.value >= range.minThreshold && condition.value <= range.maxThreshold)
                        {
                            Debug.Log($"Entered range {range.title}");
                            condition.activeRanges |= (1 << i);
                            if (range.impulse != null)
                            {
                                OnImpulse(range.impulse, null);
                            }
                            if (range.tooltip != null && m_TooltipStack != null)
                            {
                                m_TooltipStack.AddTooltip(range.tooltip);
                            }
                        }
                    }
                    else
                    {
                        if (condition.value < range.minThreshold || condition.value > range.maxThreshold)
                        {
                            condition.activeRanges ^= (1 << i);
                            if (range.tooltip != null && m_TooltipStack != null)
                            {
                                m_TooltipStack.RemoveTooltip(range.tooltip);
                            }
                        }
                    }
                    i += 1;
                }
            }
        }

        public void EnqueueActivity(Activity.ActivitySO activity, object argument, UnityAction afterFinishAction)
        {
            m_QueuedActivities.Add(new ActivityInfo(activity, argument, afterFinishAction));
        }

        public void AddActivityFirstInQueue(Activity.ActivitySO activity, object argument, UnityAction afterFinishAction)
        {
            m_QueuedActivities.Insert(0, new ActivityInfo(activity, argument, afterFinishAction));
        }

        public void AddActivityInterruptingCurrent(Activity.ActivitySO activity, object argument, UnityAction afterFinishAction)
        {
            AddActivityFirstInQueue(activity, argument, afterFinishAction);

            //In case current activity already terminated
            if (m_CurrentActivity != null)
            {
                m_CurrentActivity.EndRequested();
            }
        }

        private void AbortEnqueuedActivities()
        {
            m_QueuedActivities.Clear();
        }
    }
}
