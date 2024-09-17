using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogSpawner : MonoBehaviour
{
    public GameObject dogPrefab;
    public Activity.ActivitySO startingActivity;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameState.DogState dogState in GameStateManager.Instance.gameState.dogs)
        {
            var dog = Instantiate(dogPrefab, transform.position, transform.rotation);
            dog.name = dogState.name;
            var skinnedMeshRenderer = dog.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material = dogState.breed.material;
            skinnedMeshRenderer.material.SetTexture("_FurLengthMap", dogState.furConditionTexture);
            if (dog.TryGetComponent(out Dog.Mind mind))
            {
                mind.EnqueueActivity(startingActivity, null, null);
                mind.dogState = dogState;
                mind.habits = new List<Dog.Habit>();
                for (int i = 0; i < dogState.habits.Length; i += 1)
                {
                    mind.habits.Add(new Dog.Habit
                    {
                        pronouncedness = dogState.habits[i].pronouncedness,
                        trait = dogState.habits[i].characterTrait
                    });
                }
            }
            GlobalEvents.Instance.onDogSpawned.Invoke(dog);
        }
    }
}
