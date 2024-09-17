using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable Item", menuName = "Inventory Items/Throwable Item")]
public class ThrowableItemSO : ItemSO
{
    public bool consumable;
    public void OnEnable()
    {
        type = ItemType.Throwable;
    }
}