using Dog;
using Impulses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalEvents : MonoBehaviour
{
    // Called if a dog has been spawned in the scene. The argument is the GameObject of the new spawned dog.
    public UnityEvent<GameObject> onDogSpawned;
    // Called if a command has been spoken with the phrase as string.
    public UnityEvent<ImpulseSO, object> onDogImpulse;
    // Called if the dog ended the searching game, indicating if it has found the object
    public UnityEvent<bool> onDogEndedSearchGame;
    // Called if the dog ended the searching game, indicating if it has found the object
    public UnityEvent onDogHasNothingFound;
    // Called if the dog checks a search spot
    public UnityEvent<SearchPoint> onDogCheckSearchSpot;
    /// <summary>
    /// Call this event to trigger a quick tip.
    /// </summary>
    /// <param name="heading">The heading of the tip.</param>
    /// <param name="text">The text of the tip.</param>
    public UnityEvent<QuicktipSO> triggerQuickTip;

    // Singleton handling
    private static GlobalEvents m_Instance;
    public static GlobalEvents Instance {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindAnyObjectByType<GlobalEvents>();
                if (m_Instance == null)
                {
                    Debug.LogError("GlobalEvents is missing in the Scene!");
                    Application.Quit();
                }
            }
            return m_Instance;
        }
    }
    private void OnDestroy()
    {
        m_Instance = null;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("Saving game state on pause.");
            GameStateManager.Instance.Save();
        }
    }
}
