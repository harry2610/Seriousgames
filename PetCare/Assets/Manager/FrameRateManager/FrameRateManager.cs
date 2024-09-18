using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FrameRateManager : MonoBehaviour
{
    // [Header("Frame Settings")]
    // int MaxRate = 9999;
    public int TargetFrameRate = 60;
    // float currentFrameTime;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Debug.Log("FrameRateManager: " + TargetFrameRate + " FPS");
        // currentFrameTime = Time.realtimeSinceStartup;
        // StartCoroutine(WaitForNextFrame());
    }

    // void Update()
    // {
    //     int fps = (int)(1f / Time.unscaledDeltaTime);
    //     Debug.Log("FPS: " + fps);
    // }

    // IEnumerator WaitForNextFrame()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForEndOfFrame();
    //         currentFrameTime += 1.0f / TargetFrameRate;
    //         var t = Time.realtimeSinceStartup;
    //         var sleepTime = currentFrameTime - t - 0.01f;
    //         if (sleepTime > 0)
    //             Thread.Sleep((int)(sleepTime * 1000));
    //         while (t < currentFrameTime)
    //             t = Time.realtimeSinceStartup;
    //     }
    // }
}