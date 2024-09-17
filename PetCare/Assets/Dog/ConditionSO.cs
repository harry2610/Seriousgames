using Impulses;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dog
{
    [Serializable]
    public class ConditionRangeModifier
    {
        public ConditionSO condition;
        public int rangeID;
        [Range(-100, 100)]
        public float priorityModifier;
    }

    [Serializable]
    public class ConditionRange
    {
        public string title;
        public ImpulseSO impulse;
        public TooltipSO tooltip;
        public double minThreshold;
        public double maxThreshold;
    }

    [CreateAssetMenu(fileName = "Condition", menuName = "Dog/Condition")]
    public class ConditionSO : ScriptableObject
    {
        [ScriptableObjectIdAttribute]
        public long id;
        public string title;
        public Texture2D icon;
        [TextArea]
        public string description;
        public string unit;
        public double minimumValue;
        public double maximumValue;
        public double initialValue;
        public double changePerDay;
        public bool isTrainable;
        public ConditionRange[] conditionRanges;
    }
}