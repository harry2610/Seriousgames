using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placable Item", menuName = "Inventory Items/Placable Item")]
public class PlacableItemSO : ItemSO
{
    public long TimeToLive;
    public void OnEnable()
    {
        type = ItemType.Placeable;
    }
}