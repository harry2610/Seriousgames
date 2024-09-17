using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Inventory Items/Consumable Item")]
public class ConsumableItemSO : ItemSO
{
    public BowlType bowlType = BowlType.None;
    public void OnEnable()
    {
        type = ItemType.Consumable;
    }
}