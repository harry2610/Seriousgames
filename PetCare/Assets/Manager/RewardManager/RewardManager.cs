using System.Collections;
using System.Collections.Generic;
using Dog;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RewardManager : MonoBehaviour
{
    public static RewardManager instance { get; private set; }
    public UnityEvent<int> onScoreChanged;
    public Transform dog;
    public GameObject pointChangeInfoParticles;
    [SerializeField]
    private int m_Score;

    private void Awake()
    {
        GlobalEvents.Instance.onDogSpawned.AddListener(SetDogObject);
        m_Score = GameStateManager.Instance.gameState.inventory.score;
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }
    private void SetDogObject(GameObject obj)
    {
        dog = obj.transform;
    }

    public void UpdateScore(int score)
    {
        m_Score += score;
        GameStateManager.Instance.gameState.inventory.score = m_Score;
        onScoreChanged?.Invoke(score);
        ShowScoreChange(score);
    }

    public void UpdateScoreWithoutAnimation(int score) 
    {
        m_Score += score;
        GameStateManager.Instance.gameState.inventory.score = m_Score;
        onScoreChanged?.Invoke(score);
    }

    private void ShowScoreChange(int score)
    {
        if (score > 0)
        {
            GameObject changeInfoLabel = Instantiate(pointChangeInfoParticles, dog.transform);
            changeInfoLabel.transform.localPosition = new Vector3(0, 0.3f, 0);
            Destroy(changeInfoLabel, 3);
        }
    }

}
