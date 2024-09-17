using GameState;
using UI.Items;
using UnityEngine;

public class ConditionContainer : MonoBehaviour
{
    public ConditionEffect[] Effects;
    private DogState m_DogState;

    private void Start()
    {
        m_DogState = GameStateManager.Instance.gameState.dogs[0];
    }

    public void Apply()
    {
        m_DogState.ApplyConditionEffects(Effects);
    }
}
