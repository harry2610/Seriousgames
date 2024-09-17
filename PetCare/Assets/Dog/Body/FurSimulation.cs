using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class FurSimulation : MonoBehaviour
{
    
    public Texture2D dirtinessChangeTexture;
    private System.Int64 m_SimulationStepDelay;

    private void Update()
    {
        System.Int64 currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        GameState.DogState dogState = GameStateManager.Instance.gameState.dogs[0];
        System.Int64 timeSinceUpdate = currentTime - dogState.lastFurUpdate;
        m_SimulationStepDelay = (long)(GameStateManager.Instance.gameState.simulationDayLength * 10.0);
        if (timeSinceUpdate > m_SimulationStepDelay)
        {
            FurSimulationStep((int) (timeSinceUpdate / m_SimulationStepDelay));
            GameStateManager.Instance.gameState.dogs[0].lastFurUpdate = currentTime;
        }
    }

    public void FurSimulationStep(int simulationSteps)
    {
        foreach (GameState.DogState dog in GameStateManager.Instance.gameState.dogs)
        {
            Texture2D furConditionTexture = dog.furConditionTexture;
            NativeArray<System.Byte> conditionTextureArray = furConditionTexture.GetPixelData<System.Byte>(0);
            NativeArray<System.Byte> dirtinessTextureArray = dirtinessChangeTexture.GetPixelData<System.Byte>(0);
            for (int i = 0; i < conditionTextureArray.Length; i++)
            {
                conditionTextureArray[i] = (System.Byte) Mathf.Clamp(conditionTextureArray[i] + simulationSteps * (i % 2 == 0? 1 : dirtinessTextureArray[i/2] * 0.2f), 0, 255);
            }
            furConditionTexture.Apply(true, false);
        }
    }
    
    public void DrawAtFur(Vector2 positionUV, float radius, bool isDirtLayer, System.Byte value)
    {
        var furConditionTexture = GameStateManager.Instance.gameState.dogs[0].furConditionTexture;
        var data = furConditionTexture.GetPixelData<System.Byte>(0);
        var width = furConditionTexture.width;
        var height = furConditionTexture.height;
        for (int iy = System.Math.Max((int)((positionUV.y - radius) * height), 0); iy < System.Math.Min((int)((positionUV.y + radius) * height), height); iy += 1)
        {
            for (int ix = System.Math.Max((int)((positionUV.x - radius) * width), 0); ix < System.Math.Min((int)((positionUV.x + radius) * width), width); ix += 1)
            {
                data[(ix + iy * width) * 2 + (isDirtLayer ? 1 : 0)] = value;
            }
        }
        furConditionTexture.Apply(true, false);
    }
}
