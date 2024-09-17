using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

/// <summary>
/// provides haptic feedback for UI elements
/// </summary>
public static class UIControllerHapticFeedback
{
    /// <summary>
    /// Whether or not haptic feedback is enabled.
    /// </summary>
    public static bool VibrationsEnabled = true;

    /// <summary>
    /// The time to vibrate for basic button presses.
    /// </summary>
    public static long BasicButtonVibrationTime = 5;

    /// <summary>
    /// The time to vibrate for long button presses.
    /// </summary>
    public static long LongButtonVibrationTime = 150;

    /// <summary>
    /// The vibration pattern for basic button presses.
    /// </summary>
    public static long[] LongButtonVibrationPattern = new long[] { 0, 5, 5, 5 };

    /// <summary>
    /// vibrates for a short time
    /// </summary>
    /// <param name="milliseconds">
    /// The time to vibrate for in milliseconds.
    /// </param>
    public static void AndroidVibrateTime(long milliseconds)
    {
        if (VibrationsEnabled)
        {
            Debug.Log("Vibrating for " + milliseconds + " milliseconds.");
            #if UNITY_ANDROID
                Vibration.VibrateAndroid(milliseconds);
            #endif
        }
    }

    /// <summary>
    /// vibrates for a short time
    /// </summary>
    /// <param name="pattern">
    /// The vibration pattern to use.
    /// </param>
    /// <param name="repeat">
    /// The index into pattern at which to repeat, or -1 for no repeat.
    /// </param>
    public static void AndroidVibratePattern(long[] pattern, int repeat)
    {
        if (VibrationsEnabled)
        {
            #if UNITY_ANDROID
                Vibration.VibrateAndroid(pattern, repeat);
            #endif
        }
    }
}