using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Item", menuName = "Inventory Items/Default Item")]
public class DefaultItemSO : ItemSO
{
    public void OnEnable()
    {
        type = ItemType.Default;
    }
}
