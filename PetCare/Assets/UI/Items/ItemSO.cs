using UI.Items;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public enum ItemType
{
    Consumable,
    Equipment,
    Placeable,
    Throwable,
    Care,
    PooDeleter,
    Default,
    Minigame,
}

public class ItemSO : ScriptableObject
{
    [ScriptableObjectIdAttribute]
    public long id;
    public ItemType type;
    public string title;
    public Texture2D icon;
    public GameObject prefab;
    [TextArea(3, 10)]
    public string description;
    public int stackLimit;
    public int price;
    public ConditionEffect[] conditionEffects;
}