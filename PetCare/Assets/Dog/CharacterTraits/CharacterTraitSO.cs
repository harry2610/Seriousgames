using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dog
{
    [System.Serializable]
    public class ConditionModifier
    {
        [Range(-100f, 100)]
        public float thirst;
        [Range(-100f, 100)]
        public float hunger;
        [Range(-100f, 100)]
        public float entertainment;
        [Range(-100f, 100)]
        public float aggressiveness;
    }
    [System.Serializable]
    public class ActivityTrigger
    {
        [SerializeReference, SubclassSelector]
        public Activity.ActivitySO activity;
    }
    [CreateAssetMenu(fileName = "Trait", menuName = "Dog/Character Trait", order = 1)]
    public class CharacterTraitSO : ScriptableObject
    {
        [ScriptableObjectIdAttribute]
        public long id;
        public string title;
        public Texture2D icon; 
        [TextArea]
        public string description;
        public ConditionModifier conditionModifier;
        public List<ActivityTrigger> triggeringActivities;
    }
}