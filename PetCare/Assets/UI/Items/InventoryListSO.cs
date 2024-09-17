using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Inventory List", menuName = "Inventory System/Inventory List")]
public class InventoryListSO : ScriptableObject
{
    public List<InventorySlot> Inventory = new List<InventorySlot>();
    protected SortedDictionary<int, InventorySlot> InventoryDictionary = new SortedDictionary<int, InventorySlot>();
    [HideInInspector]
    public UnityEvent onInventoryChanged = new UnityEvent();
    public int AddItem(ItemSO item, int amount)
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            if (Inventory[i].Item == item)
            {
                Inventory[i].Amount = Mathf.Clamp(Inventory[i].Amount + amount, 0, item.stackLimit);
                onInventoryChanged.Invoke();
                return Inventory[i].SlotID;
            }
        }
        int newSlotID = GetUniqueSlotID();
        InventorySlot newSlot = new InventorySlot(item, amount, newSlotID);
        Inventory.Add(newSlot);
        InventoryDictionary.Add(newSlotID, newSlot);
        onInventoryChanged.Invoke();
        return newSlotID;
    }
    private int GetUniqueSlotID()
    {
        int newSlotID = 0;
        while (InventoryDictionary.ContainsKey(newSlotID))
        {
            newSlotID++;
        }
        return newSlotID;
    }
    public void RemoveItem(ItemSO item, int amount)
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            if (Inventory[i].Item == item)
            {
                Inventory[i].Amount -= amount;
                if (Inventory[i].Amount <= 0)
                {
                    InventoryDictionary.Remove(Inventory[i].SlotID);
                    Inventory.Remove(Inventory[i]);
                }
                onInventoryChanged.Invoke();
                break;
            }
        }
    }
    public InventorySlot GetItem(int slotID)
    {
        return InventoryDictionary.GetValueOrDefault(slotID);
    }
    public void RemoveItem(int slotID, int amount)
    {
        InventorySlot temp = InventoryDictionary.GetValueOrDefault(slotID);
        if (temp != null)
        {
            temp.Amount -= amount;
            if (temp.Amount <= 0)
            {
                InventoryDictionary.Remove(slotID);
                Inventory.Remove(temp);
            }
            onInventoryChanged.Invoke();
        }
    }
    public void MoveItem(int slotID, int newSlotID)
    {
        if (slotID == newSlotID) return;
        InventorySlot temp = InventoryDictionary.GetValueOrDefault(slotID);
        temp.SlotID = newSlotID;
        InventoryDictionary.Remove(slotID);
        InventoryDictionary.Add(newSlotID, temp);
        onInventoryChanged.Invoke();
    }
    public void SwapItems(int slotID1, int slotID2)
    {
        if (slotID1 == slotID2) return;
        InventorySlot temp = InventoryDictionary.GetValueOrDefault(slotID1);
        InventorySlot temp2 = InventoryDictionary.GetValueOrDefault(slotID2);
        if (temp != null && temp2 != null)
        {
            temp.SlotID = slotID2;
            temp2.SlotID = slotID1;
            InventoryDictionary.Remove(slotID1);
            InventoryDictionary.Remove(slotID2);
            InventoryDictionary.Add(slotID2, temp);
            InventoryDictionary.Add(slotID1, temp2);
        }
        if (temp != null && temp2 == null)
        {
            MoveItem(slotID1, slotID2);
        }
        if (temp == null && temp2 != null)
        {
            MoveItem(slotID2, slotID1);
        }
        onInventoryChanged.Invoke();
    }
    public void ClearInventory()
    {
        Inventory.Clear();
        InventoryDictionary.Clear();
        onInventoryChanged.Invoke();
    }
    public void SortInventory()
    {
        Inventory.Sort((x, y) => x.SlotID.CompareTo(y.SlotID));
    }
    public void SortInventoryByItemID()
    {
        Inventory.Sort((x, y) => x.Item.id.CompareTo(y.Item.id));
    }
    public void SortInventoryBySlotID()
    {
        Inventory.Sort((x, y) => x.SlotID.CompareTo(y.SlotID));
    }
    public void SortInventoryByAmount()
    {
        Inventory.Sort((x, y) => x.Amount.CompareTo(y.Amount));
    }
    public void SortInventoryByItemType()
    {
        Inventory.Sort((x, y) => x.Item.type.CompareTo(y.Item.type));
    }
    public void SortInventoryByItemName()
    {
        Inventory.Sort((x, y) => x.Item.title.CompareTo(y.Item.title));
    }

    public void UpdateInventoryDictionary()
    {
        InventoryDictionary.Clear();
        foreach (InventorySlot slot in Inventory)
        {
            InventoryDictionary.Add(slot.SlotID, slot);
        }
    }
    public void OnEnable()
    {
        UpdateInventoryDictionary();
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemSO Item;
    public int Amount;
    public int SlotID;
    public bool IsInUse;
    public InventorySlot(ItemSO newItem, int newAmount, int newSlotID)
    {
        Item = newItem;
        Amount = Mathf.Clamp(newAmount, 1, newItem.stackLimit);
        SlotID = newSlotID;
    }

    private GameObject itemObject;

    public GameObject OnClick(bool alwaysCreateNew = false)
    {
        if (itemObject == null || alwaysCreateNew)
        {
            itemObject = UnityEngine.Object.Instantiate(Item.prefab) as GameObject;
            ConditionContainer conditions = itemObject.AddComponent<ConditionContainer>();
            conditions.Effects = Item.conditionEffects;
        }
        else
        {
            UnityEngine.Object.Destroy(itemObject);
            itemObject = null;
        }
        return itemObject;
    }
}