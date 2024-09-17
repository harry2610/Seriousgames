using System;
using System.Collections;
using System.Collections.Generic;
using GameState;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Player Inventory List", menuName = "Inventory System/Player Inventory List")]
public class PlayerInventoryListSO : InventoryListSO
{
    public void SaveInventory()
    {
        GameStateManager gsm = GameStateManager.Instance;
        gsm.gameState.inventory.items = new GameState.ItemState[Inventory.Count];
        for (int i = 0; i < Inventory.Count; i++)
        {
            gsm.gameState.inventory.items[i] = new GameState.ItemState
            {
                type = Inventory[i].Item,
                amount = (uint)Inventory[i].Amount,
                slot = (uint)Inventory[i].SlotID
            };
        }
    }

    public void LoadInventory()
    {
        GameStateManager gsm = GameStateManager.Instance;
        Inventory.Clear();
        InventoryDictionary.Clear();
        if (gsm != null)
        {
            foreach (GameState.ItemState itemState in gsm.gameState.inventory.items)
            {
                Inventory.Add(new InventorySlot(itemState.type, (int)itemState.amount, (int)itemState.slot));
            }
        }
        UpdateInventoryDictionary();
        onInventoryChanged.Invoke();
    }
}