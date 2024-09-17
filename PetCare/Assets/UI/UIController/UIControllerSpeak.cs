using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Impulses;
using GameState;
using System;

/// <summary>
/// Controls the speaking button and speech recognition.
/// </summary>
public class UIControllerSpeak : MonoBehaviour
{
    public ImpulseSO commandImpulse;
    // The button for speaking.
    private Button m_SpeakingButton;
    private VisualElement m_SpeakIcon; // The icon for speaking in the button.
    private Label m_SpeakingLabel; // The label for displaying speech recognition result (for debugging).
    private VisualElement m_SpeakingLabelContainer; // The container for the label.
    private SpeechRecognizer m_SpeechRecognizer; // The speech recognizer component.

    public QuicktipSO tipForSpeakButton;

    private void Awake()
    {
        GlobalEvents.Instance.onDogSpawned.AddListener((dog) => { });
    }

    void Start()
    {
        // Get the root visual element.
        var root = GetComponent<UIDocument>().rootVisualElement;

        m_SpeakingButton = root.Q<Button>("speak");
        m_SpeakIcon = m_SpeakingButton.Q<VisualElement>("SpeakIcon");
        m_SpeakingLabel = root.Q<Label>("speechtest");
        m_SpeakingLabelContainer = root.Q<VisualElement>("speechtestContainer");

        m_SpeakingLabelContainer.style.display = DisplayStyle.None;

        m_SpeechRecognizer = GetComponent<SpeechRecognizer>();

        m_SpeakingButton.RegisterCallback<ClickEvent>(OnSpeakButtonClicked);

    }

    private void OnFinalResult(string result)
    {
        CommandState[] commandStates = GameStateManager.Instance.gameState.dogs[0].commands;
        m_SpeakingLabel.text = GameStateManager.Instance.gameState.dogs[0].name + " versteht dich nicht";

        if (commandStates != null)
        {
            foreach (CommandState commandState in commandStates)
            {
                string[] phrases = commandState.phrases.Split(',');
                foreach (string phrase in phrases)
                {
                    if (result.Contains(phrase))
                    {
                        m_SpeakingLabel.text = phrase;
                    }
                }
            }
        }
        OnEndOfSpeech();
        GlobalEvents.Instance.onDogImpulse.Invoke(commandImpulse, result);
    }

    private void OnPartialResult(string result)
    {
        m_SpeakingLabel.text = result;
        OnEndOfSpeech();
        GlobalEvents.Instance.onDogImpulse.Invoke(commandImpulse, result);
    }

    private void OnEndOfSpeech()
    {

        m_SpeakIcon.AddToClassList("notspeaking");
        m_SpeakIcon.RemoveFromClassList("speaking");
        m_SpeakingButton.Blur();
        StartCoroutine(OnEndOfSpeechCoroutine(3.0f));

        m_SpeakingButton.UnregisterCallback<ClickEvent>(OnStopSpeakButtonClicked);
        m_SpeakingButton.RegisterCallback<ClickEvent>(OnSpeakButtonClicked);
    }

    private IEnumerator OnEndOfSpeechCoroutine(float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);
        m_SpeakingLabelContainer.style.display = DisplayStyle.None;
    }

    private void OnSpeakButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        GlobalEvents.Instance.triggerQuickTip.Invoke(tipForSpeakButton);
        StopAllCoroutines();

        m_SpeakIcon.AddToClassList("speaking");
        m_SpeakIcon.RemoveFromClassList("notspeaking");
        m_SpeakingLabel.text = "nimmt auf...";
        m_SpeakingLabelContainer.style.display = DisplayStyle.Flex;

        m_SpeechRecognizer.StartRecording(OnFinalResult);

        m_SpeakingButton.UnregisterCallback<ClickEvent>(OnSpeakButtonClicked);
        m_SpeakingButton.RegisterCallback<ClickEvent>(OnStopSpeakButtonClicked);
    }

    private void OnStopSpeakButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        m_SpeakingButton.Blur();
        m_SpeakIcon.AddToClassList("notspeaking");
        m_SpeakIcon.RemoveFromClassList("speaking");
        m_SpeakingButton.Blur();

        m_SpeakingButton.UnregisterCallback<ClickEvent>(OnStopSpeakButtonClicked);
        m_SpeakingButton.RegisterCallback<ClickEvent>(OnSpeakButtonClicked);
    }
}