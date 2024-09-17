using Activity;
using Dog;
using Impulses;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Assertions;


[System.Flags]
public enum BodyParts
{
    None = 0,
    Legs = 1 << 0,
    Head = 1 << 1,
    Mouth = 1 << 2,
    Tail = 1 << 3,
}

public interface IActivityInfo
{
    public ActivityExecution StartActivity(GameObject dog);
}

public abstract class ActivityExecution
{
    private ActivitySO m_ActivitySO;
    protected bool m_IsReadyToEnd;
    public ActivityExecution(ActivitySO activitySO)
    {
        m_ActivitySO = activitySO;
        m_IsReadyToEnd = false;
    }
    public abstract void Update();
    public virtual void EndRequested()
    {
        if (m_ActivitySO.isAbortable)
            m_IsReadyToEnd = true;
    }
    public bool IsReadyToEnd()
    {
        return m_IsReadyToEnd;
    }
    public void EndActivity()
    {
        m_IsReadyToEnd = true;
    }
    public ActivitySO GetActivitySO()
    {
        return m_ActivitySO;
    }
    public virtual (ImpulseSO impulse, object argument) GetImpulseOnEnd()
    {
        return (m_ActivitySO.impulseOnEnd, null);
    }
    public virtual int GetPoints()
    {
        return m_ActivitySO.points;
    }
    public virtual void OnEnd()
    {

    }
}

namespace Activity
{
    public abstract class ActivitySO : ScriptableObject
    {
        [ScriptableObjectIdAttribute]
        public long id;
        [Header("Activity")]
        public string title;
        public Texture2D icon;
        [TextArea]
        public string description;
        public int points;
        public ImpulseSO impulseOnEnd;
        public bool isAbortable = true;
        public abstract ActivityExecution StartActivity(GameObject dog, object argument);
        public ConditionSO poorEducation;
        public ConditionSO goodEducation;
        public double rewardInfluence;
    }
}