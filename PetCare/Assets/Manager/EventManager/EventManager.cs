using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using System.Collections;
using Dog;

public class EventManager : MonoBehaviour
{
    private float checkInterval = 5f;
    private float minDaylength = 1f;
    private float maxDaylength = 3f;
    public ConditionSO illnessCondition;

    private void Start()
    {
        InvokeRepeating(nameof(CheckGameStatus), 0f, checkInterval);
    }

    private void CheckGameStatus()
    {
        var dogs = GameStateManager.Instance.gameState.dogs;
        var conditions = GameStateManager.Instance.resourceList.conditions;

        foreach (var dog in dogs)
        {
            var hungerCondition = conditions.FirstOrDefault(c => c.name == "Hunger");
            if (hungerCondition != null)
            {
                double hunger = dog.GetCondition(hungerCondition);
                if (hunger <= 0)
                {
                    EndGame("Dein Hund wurde dir vom Veterinäramt weggenommen, weil dein Hund fast verhungert wäre.");
                    return;
                }
            }

            var thirstCondition = conditions.FirstOrDefault(c => c.name == "Thirst");
            if (thirstCondition != null)
            {
                double thirst = dog.GetCondition(thirstCondition);
                if (thirst <= 0)
                {
                    EndGame("Dein Hund wurde dir vom Veterinäramt weggenommen, weil dein Hund fast verdurstet wäre.");
                    return;
                }
            }

            var illnessCondition = conditions.FirstOrDefault(c => c.name == "Illness");
            if (illnessCondition != null)
            {
                double illness = dog.GetCondition(illnessCondition);
                if (illness >= 100)
                {
                    EndGame("Dein Hund wurde dir vom Veterinäramt weggenommen, weil dein Hund schwer krank war.");
                    return;
                }
            }
            System.Int64 currentTime = (System.Int64)System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (dog.nextIllnessTime == 0 || currentTime >= dog.nextIllnessTime)
            {
                if (dog.nextIllnessTime != 0)
                {
                    double randomIllness = Random.Range(20, 99);
                    dog.SetCondition(illnessCondition, randomIllness);
                    Debug.Log($"Der Hund {dog.name} wurde krank mit einem Krankheitswert von {randomIllness}.");
                }
                float daylength = (float)GameStateManager.Instance.gameState.simulationDayLength;
                dog.nextIllnessTime = currentTime + (System.Int64)(Random.Range(minDaylength, maxDaylength) * daylength * 1000.0f);
            }
        }
    }

    private void EndGame(string reason)
    {
        // Save reason in PlayerPrefs
        PlayerPrefs.SetString("EndGameReason", reason);
        PlayerPrefs.Save();

        // Delete SaveGame
        DeleteCurrentSaveGame();

        // Change Scene
        SceneManager.LoadScene("Ending");
    }

    private void DeleteCurrentSaveGame()
    {
        string saveGameName = GameStateManager.Instance.SaveGamePath;
        if (!string.IsNullOrEmpty(saveGameName))
        {
            string fullPath = saveGameName;
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Debug.Log($"Deleted save game file at: {fullPath}");
            }
            else
            {
                Debug.LogWarning($"Save game file not found at: {fullPath}");
            }
        }
        else
        {
            Debug.LogError("Current save game path is not set.");
        }
    }
}
