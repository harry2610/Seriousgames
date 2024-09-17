using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Item", menuName = "Inventory Items/Equipment Item")]
public class EquipmentItemSO : ItemSO
{
    public void OnEnable()
    {
        type = ItemType.Equipment;
    }
}