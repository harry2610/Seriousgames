using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Effect Object", menuName = "Music and Sounds/Sound Effects")]
public class SoundEffectsSO : ScriptableObject
{
    public AudioClip[] soundEffectList;
}