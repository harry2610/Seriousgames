using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourTraining", menuName = "Dog/Training/Behaviour")]
public class BehaviourTrainingSO : ScriptableObject
{
    public string title;
    [Range(0, 100)]
    public double startProgress;
    public TrainingMilestone[] milestones;
}

[Serializable]
public class TrainingMilestone
{
    public string title;
    [Range(0, 100)]
    public double progressRequirement;
}
