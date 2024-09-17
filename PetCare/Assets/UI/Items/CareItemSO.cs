using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Care Item", menuName = "Inventory Items/Care Item")]
public class CareItemSO : ItemSO
{
    public void OnEnable()
    {
        type = ItemType.Care;
    }
}