using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StartGame", menuName = "Start Game")]
public class StartGameSO : ScriptableObject
{
    public string title;
    [TextArea]
    public string description;
    public int coins;
    public int coinsPerDay;
    public ItemSO[] items;
}
