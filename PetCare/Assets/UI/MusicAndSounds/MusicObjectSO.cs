using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Object", menuName = "Music and Sounds/Music")]
public class MusicObjectSO : ScriptableObject
{
    public AudioClip[] MusicList;
}