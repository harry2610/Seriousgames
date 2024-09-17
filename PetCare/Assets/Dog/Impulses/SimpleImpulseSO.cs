using Dog;
using System;
using UnityEngine;



namespace Impulses
{
    [Serializable]
    public class ActivityOption
    {
        public Activity.ActivitySO activity;
        [Range(0, 100)]
        public float basePriority;
        public TraitModifier[] traitModifier;
        public ConditionRangeModifier[] conditionModifier;
        public float GetPriority(Dog.Mind mind)
        {
            float priority = basePriority;
            foreach (var modifier in traitModifier) {
                foreach (var habit in mind.habits)
                {
                    if (habit.trait == modifier.trait)
                        priority += habit.pronouncedness * modifier.priorityModifier * 0.01f;
                }
            }
            foreach (var modifier in conditionModifier)
            {
                if (mind.dogState.IsInConditionRange(modifier.condition, modifier.rangeID))
                {
                    priority += modifier.priorityModifier;
                }
            }
            if (priority > 0.0)
                return priority;
            else
                return 0.0f;
        }
    }
    [Serializable]
    public class TraitModifier
    {
        public CharacterTraitSO trait;
        [Range(0, 100)]
        public float priorityModifier;

    }

    [CreateAssetMenu(fileName = "SimpleImpulse", menuName = "Dog/Impulses/Simple")]
    public class SimpleImpulseSO : ImpulseSO
    {
        public ActivityOption[] activityOptions;
        public override Activity.ActivitySO DecideActivity(Dog.Mind mind, object argument)
        {
            float totalStimulus = 0.0f;
            foreach (var opt in activityOptions)
                totalStimulus += opt.GetPriority(mind);
            float randomStimulus = totalStimulus * UnityEngine.Random.value;
            foreach (var activityOption in activityOptions)
            {
                randomStimulus -= activityOption.GetPriority(mind);
                if (randomStimulus <= 0f)
                    return activityOption.activity;
            }
            return null;
        }
    }
}