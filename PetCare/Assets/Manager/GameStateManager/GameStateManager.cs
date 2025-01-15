using Dog;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using GameState;
using System.Linq;
using UI.Items;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager m_Instance;
    public static GameStateManager Instance { 
        get
        {
            return m_Instance;
        }
    }
    public ResourceListSO resourceList;
    public GameState.GameState gameState;
    private string saveGamePath;
    const string saveGameFolder = "SaveGames";

    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    static public List<string> GetSaveGameNames()
    {
        var path = Path.Combine(Application.persistentDataPath, saveGameFolder);
        Debug.Log(path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        var files = Directory.EnumerateFiles(path);
        List<string> saveGameNames = new List<string>();
        foreach (string f in files)
        {
            if (Path.GetExtension(f) == ".sav")
                saveGameNames.Add(Path.GetFileNameWithoutExtension(f));
        }
        return saveGameNames;
    }
    public void SetSaveGameName(string name)
    {
        var path = Path.Combine(Application.persistentDataPath, saveGameFolder);
        saveGamePath = Path.Combine(path, $"{name}.sav");
    }

    public string SaveGamePath
    {
        get { return saveGamePath; }
    }

    public void Load(string saveGameName)
    {
        SetSaveGameName(saveGameName);
        using (var stream = File.Open(saveGamePath, FileMode.Open))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                gameState = new GameState.GameState();
                gameState.fileVersion = reader.ReadUInt32();
                gameState.lastSaveTime = reader.ReadInt64();
                gameState.simulationDayLength = reader.ReadDouble();
                gameState.inventory = new GameState.InventoryState();
                gameState.inventory.coins = reader.ReadInt32();
                gameState.inventory.coinsPerDay = reader.ReadInt32();
                gameState.inventory.lastCoinsPerDay = reader.ReadInt64();
                gameState.inventory.score = reader.ReadInt32();
                gameState.inventory.lastQuizTime = reader.ReadInt64();
                var itemCount = reader.ReadUInt32();
                gameState.inventory.items = new GameState.ItemState[itemCount];
                for (uint i = 0; i < itemCount; i += 1)
                {
                    var itemState = new ItemState();
                    var itemID = reader.ReadInt64();
                    Debug.Log($"Item ID {itemID}");
                    itemState.type = resourceList.items.Single(item => item.id == itemID);
                    itemState.amount = reader.ReadUInt32();
                    itemState.slot = reader.ReadUInt32();
                    gameState.inventory.items[i] = itemState;
                }
                var placedItemCount = reader.ReadUInt32();
                gameState.inventory.placedItems = new List<ItemSO>();
                for (uint i = 0; i < placedItemCount; i += 1)
                {
                    var itemID = reader.ReadInt64();
                    gameState.inventory.placedItems.Add(resourceList.items.Single(item => item.id == itemID));
                }
                var dogCount = reader.ReadUInt32();
                gameState.dogs = new GameState.DogState[dogCount];
                for (uint i = 0; i < dogCount; i += 1)
                {
                    var dog = new DogState();
                    dog.name = reader.ReadString();
                    var breedID = reader.ReadInt64();
                    dog.breed = resourceList.breeds.Single(breed => breed.id == breedID);
                    var habitCount = reader.ReadUInt32();
                    dog.habits = new GameState.HabitState[habitCount];
                    for (uint j = 0; j < habitCount; j += 1)
                    {
                        dog.habits[j] = new GameState.HabitState();
                        var traitID = reader.ReadInt64();
                        dog.habits[j].characterTrait = resourceList.characterTraits.Single(trait => trait.id == traitID);
                        dog.habits[j].pronouncedness = reader.ReadSingle();
                    }
                    var commandCount = reader.ReadUInt32();
                    dog.commands = new GameState.CommandState[commandCount];
                    for (uint j = 0; j < commandCount; j += 1)
                    {
                        dog.commands[j] = new GameState.CommandState();
                        var activityID = reader.ReadInt64();
                        dog.commands[j].activity = resourceList.activities.Single(activity => activity.id == activityID);
                        dog.commands[j].phrases = reader.ReadString();
                        dog.commands[j].rehearsed = reader.ReadSingle();
                    }
                    var conditionCount = reader.ReadUInt32();
                    dog.conditions = new GameState.ConditionValue[conditionCount];
                    for (uint j = 0; j < conditionCount; j += 1)
                    {
                        dog.conditions[j] = new GameState.ConditionValue();
                        var conditionID = reader.ReadInt64();
                        dog.conditions[j].type = resourceList.conditions.Single(condition => condition.id == conditionID);
                        dog.conditions[j].value = reader.ReadDouble();
                        dog.conditions[j].activeRanges = reader.ReadInt32();
                    }
                    dog.lastConditionUpdate = reader.ReadInt64();
                    dog.furConditionTexture = new Texture2D(256, 256, TextureFormat.RG16, false);
                    var texBuf = dog.furConditionTexture.GetPixelData<Byte>(0);
                    reader.Read(texBuf);
                    dog.furConditionTexture.Apply(true, false);
                    dog.lastFurUpdate = reader.ReadInt64();
                    dog.nextIllnessTime = reader.ReadInt64();
                    gameState.dogs[i] = dog;
                }
                var quickTipCount = reader.ReadUInt32();
                gameState.quickTips = new GameState.QuickTipState[quickTipCount];
                for (uint i = 0; i < quickTipCount; i += 1)
                {
                    var tip = new QuickTipState();
                    var tipID = reader.ReadInt64();
                    tip.tooltip = resourceList.quicktips.Single(tip => tip.id == tipID);
                    tip.timesToShowLeft = reader.ReadInt32();
                    gameState.quickTips[i] = tip;
                }
            }
        }
    }

    public void Save()
    {
        using (var stream = File.Open(saveGamePath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                // File Version 0
                writer.Write((UInt32) 1);
                writer.Write((Int64) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                writer.Write((Double)gameState.simulationDayLength);
                writer.Write((Int32)gameState.inventory.coins);
                writer.Write((Int32)gameState.inventory.coinsPerDay);
                writer.Write((Int64)gameState.inventory.lastCoinsPerDay);
                writer.Write((Int32)gameState.inventory.score);
                writer.Write((Int64)gameState.inventory.lastQuizTime);
                writer.Write((UInt32)gameState.inventory.items.Length);
                foreach (GameState.ItemState item in gameState.inventory.items)
                {
                    writer.Write((Int64)item.type.id);
                    writer.Write((UInt32)item.amount);
                    writer.Write((UInt32)item.slot);
                }
                if (gameState.inventory.placedItems == null)
                    gameState.inventory.placedItems = new List<ItemSO>();
                writer.Write((UInt32)gameState.inventory.placedItems.Count);
                foreach (ItemSO item in gameState.inventory.placedItems)
                {
                    writer.Write((Int64)item.id);
                }
                writer.Write((UInt32)gameState.dogs.Length);
                foreach (GameState.DogState dog in gameState.dogs)
                {
                    writer.Write(dog.name);
                    writer.Write((Int64)dog.breed.id);
                    writer.Write((UInt32)dog.habits.Length);
                    foreach (GameState.HabitState habit in dog.habits)
                    {
                        writer.Write((Int64)habit.characterTrait.id);
                        writer.Write((float)habit.pronouncedness);
                    }
                    writer.Write((UInt32)dog.commands.Length);
                    foreach (GameState.CommandState command in dog.commands)
                    {
                        writer.Write((Int64)command.activity.id);
                        writer.Write(command.phrases);
                        writer.Write((float)command.rehearsed);
                    }
                    writer.Write((UInt32)dog.conditions.Length);
                    foreach (GameState.ConditionValue condition in dog.conditions)
                    {
                        writer.Write((Int64)condition.type.id);
                        writer.Write((double)condition.value);
                        writer.Write((Int32)condition.activeRanges);
                    }
                    writer.Write((Int64)dog.lastConditionUpdate);
                    // Save fur condition texture
                    var furData = dog.furConditionTexture.GetPixelData<Byte>(0);
                    writer.Write(furData);
                    writer.Write((Int64) dog.lastFurUpdate);
                    writer.Write((Int64)dog.nextIllnessTime);
                }
                writer.Write((UInt32)gameState.quickTips.Length);
                foreach (GameState.QuickTipState tip in gameState.quickTips)
                {
                    writer.Write((Int64)tip.tooltip.id);
                    writer.Write((Int32)tip.timesToShowLeft);
                }
            }
        }
    }

    public static void LoadScene(int id)
    {
        Instance.Save();
        SceneManager.LoadScene(id);
    }
    public static void LoadScene(string id)
    {
        Instance.Save();
        SceneManager.LoadScene(id);
    }
}

namespace GameState
{
    [System.Serializable]
    public class GameState
    {
        public uint fileVersion;
        public long lastSaveTime; // Unix Time in seconds
        public double simulationDayLength;
        public InventoryState inventory;
        public DogState[] dogs;
        public QuickTipState[] quickTips;
    }
    [System.Serializable]
    public class InventoryState
    {
        public int coins;
        public int coinsPerDay;
        public long lastCoinsPerDay;
        public int score;
        public long lastQuizTime;
        public ItemState[] items;
        public List<ItemSO> placedItems;
    }
    [System.Serializable]
    public class ItemState
    {
        public ItemSO type;
        public uint amount;
        public uint slot;
    }
    [System.Serializable]
    public class DogState
    {
        public string name;
        public BreedSO breed;
        public HabitState[] habits;
        public CommandState[] commands;
        public ConditionValue[] conditions;
        public System.Int64 lastConditionUpdate;
        public Texture2D furConditionTexture;
        public System.Int64 lastFurUpdate;
        public System.Int64 nextIllnessTime;

        public void SetCondition(ConditionSO condition, double value)
        {
            foreach (ConditionValue conditionValue in conditions)
            {
                if (conditionValue.type.id != condition.id) continue;
                
                conditionValue.value = value;
                break;
            }
        }
        public double GetCondition(ConditionSO condition)
        {
            foreach (ConditionValue conditionValue in conditions)
            {
                if (conditionValue.type.id == condition.id)
                    return conditionValue.value;
            }
            return 0.0;
        }

        public void ModifyCondition(ConditionSO condition, double value)
        {
            foreach (ConditionValue conditionValue in conditions)
            {
                if (conditionValue.type.id != condition.id) continue;
                
                conditionValue.value += value;
                break;
            }
        }
        
        public void ApplyConditionEffects(IEnumerable<ConditionEffect> effects)
        {
            foreach (ConditionEffect conditionEffect in effects)
            {
                foreach (ConditionValue conditionValue in conditions)
                {
                    if (conditionValue.type.id != conditionEffect.condition.id) continue;
                    
                    conditionValue.value += conditionEffect.modifier;
                    break;
                }
            }
        }
        
        public bool IsInConditionRange(ConditionSO condition, int rangeID)
        {
            foreach (ConditionValue conditionValue in conditions)
            {
                if (conditionValue.type.id == condition.id)
                    return (conditionValue.activeRanges & (1 << rangeID)) != 0;
            }
            return false;
        }
    }
    [System.Serializable]
    public class ConditionValue
    {
        public Dog.ConditionSO type;
        public double value;
        public Int32 activeRanges;
    }
    [System.Serializable]
    public class HabitState
    {
        public Dog.CharacterTraitSO characterTrait;
        public float pronouncedness;
    }
    [System.Serializable]
    public class CommandState
    {
        public Activity.ActivitySO activity;
        public string phrases;
        [Range(0f, 100f)]
        public float rehearsed;
    }

    [System.Serializable]
    public class QuickTipState
    {
        public QuicktipSO tooltip;
        public int timesToShowLeft;
    }
}