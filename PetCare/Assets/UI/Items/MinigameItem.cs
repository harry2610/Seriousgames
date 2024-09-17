using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Minigame Item", menuName = "Inventory Items/Minigame Item")]
public class MinigameItem : ItemSO
{
    public void OnEnable()
    {
        type = ItemType.Minigame;
    }
}