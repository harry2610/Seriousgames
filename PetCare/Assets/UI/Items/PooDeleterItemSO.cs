using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Inventory Items/Poo Deleter Item")]
public class PooDeleterItemSO : ItemSO
{
    public bool requireWiping = true;
    public void OnEnable()
    {
        type = ItemType.PooDeleter;
    }
}