using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoneySystem : MonoBehaviour
{
    public static MoneySystem instance { get; private set; }
    [HideInInspector]
    public UnityEvent<int> onCoinsChanged;
    [field: SerializeField]
    public int coins { get; private set; }
    public int coinsPerDay = 100;
    public int coinsPerAchievement = 50;
    public int coinsPerQuiz = 10;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void SaveCoins()
    {
        GameStateManager gsm = GameStateManager.Instance;
        gsm.gameState.inventory.coins = coins;
    }

    private void Start()
    {
        coins = GameStateManager.Instance.gameState.inventory.coins;
        coinsPerDay = GameStateManager.Instance.gameState.inventory.coinsPerDay;
        StartCoroutine(AddCoinsPerDayRoutine());
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        onCoinsChanged?.Invoke(amount);
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
        SaveCoins();
        onCoinsChanged?.Invoke(-amount);
    }

    public void AddCoinsPerDay()
    {
        AddCoins(coinsPerDay);
    }

    public IEnumerator AddCoinsPerDayRoutine()
    {
        while (true)
        {
            if (IsNewDay())
            {
                AddCoinsPerDay();
                GameStateManager.Instance.gameState.inventory.lastCoinsPerDay = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            yield return new WaitForSeconds(60);
        }
    }

    public bool IsNewDay()
    {
        long lastCoinsPerDay = GameStateManager.Instance.gameState.inventory.lastCoinsPerDay;
        long simulationDayLength = ((long) GameStateManager.Instance.gameState.simulationDayLength) * 1000;
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (now > lastCoinsPerDay + simulationDayLength)
        {
            return true;
        }
        Debug.Log("New Coins Per Day in " + (lastCoinsPerDay + simulationDayLength - now) / 60000 + " minutes");
        return false;
    }

    public void AddCoinsPerQuiz()
    {
        AddCoins(coinsPerQuiz);
    }

    [ContextMenu("Add Coins Per Achievement")]
    public void AddCoinsPerAchievement()
    {
        AddCoins(coinsPerAchievement);
    }

    [ContextMenu("Reset Coins")]
    public void ResetCoins()
    {
        int diff = coins;
        coins = 0;
        SaveCoins();
        onCoinsChanged?.Invoke(-diff);
    }

    // private void Update()
    // {
    //     Debug.Log("Coins: " + Coins);
    // }
}
