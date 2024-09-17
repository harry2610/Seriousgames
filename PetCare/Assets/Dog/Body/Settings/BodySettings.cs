using UnityEngine;


[CreateAssetMenu(fileName = "BodySettings", menuName = "Dog/Body Settings", order = 1)]
public class BodySettings : ScriptableObject
{
    public float breathingWobbleAmplitude;
    public float stepSize;
    public float acceleration;
    public float deceleration;
    public float frontPawMaxDisplacement = 0.10f;
    public float frontPawMinDisplacement = -0.08f;
    public float rearPawMaxDisplacement = 0.19f;
    public float rearPawMinDisplacement = -0.07f;
    public float pawInfluenceForPelvis = 0.25f;
    public float pawInfluenceForNeck = 0.25f;
    public float pawWalkSwerving = 0.1f;
    [Range(0f, 0.05f)]
    public float spineStretching;
    public AudioClip[] barkingSounds;
    public AudioClip[] howlingSounds;
    public AudioClip[] growlingSounds;
    public AudioClip[] sniffingSounds;
    public AudioClip[] eatingSounds;
    public AudioClip[] drinkingSounds;
}