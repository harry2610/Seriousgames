using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;

/// <summary>
/// Controls the tips button and info panel.
/// </summary>
public class UIControllerTips : MonoBehaviour
{
    private Button m_PrevTipButton; // The button for the previous tip.
    private Button m_NextTipButton; // The button for next tip.
    private Label m_InfoAmount; // The label for the info amount.
    private VisualElement m_InfoContainer; // The container for the info UI elements.
    private Button m_InfoButton; // The button to open the info panel.
    private VisualElement m_InfoButtonBlink; // The VisualElement that makes the info button blink.
    private Button m_InfoCloseButton; // The button to close the info panel.
    private VisualElement m_InfoPerson; // The visual element representing the person in the info panel.
    private VisualElement m_InfoPersonIMG; // The visual element representing the person image in the info panel.
    private VisualElement m_InfoBubble; // The visual element representing the info bubble.
    private Label m_InfoHeader; // The visual element representing the header in the info panel.
    private Label m_InfoText; // The visual element representing the text in the info panel.
    private VisualElement m_InfoScrim; // The visual element representing the scrim overlay in the info panel.
    private float m_HeaderAnimationDuration = 0.7f; // The speed of the Header text animation.
    private float m_TextAnimationDuration = 2f; // The speed of the text animation.
    private float m_PersonAnimationDuration = 0.5f; // The speed of the person animation.
    private TooltipStack m_TooltipStack; // The tooltip stack.

    /// <summary>
    /// Sets the tooltip text.
    /// </summary>
    /// <param name="heading"></param>
    /// <param name="text"></param>
    public void SetTooltip(string heading, string text)
    {
        tooltipText = text;
        tooltipHeader = heading;
    }

    /// <summary>
    /// The speed at which the text is typed out.
    /// Higher values mean faster typing.
    /// </summary>
    public float TypingSpeed = 25f;

    /// <summary>
    /// The text for the tooltip.
    /// </summary>
    public string tooltipText = "Hier steht eine kurze Erklärung, was du tun musst...";

    /// <summary>
    /// The header text for the tooltip.
    /// </summary>
    public string tooltipHeader = "Das ist ein Tipp!";

    void Start()
    {
        // Get the root visual element.
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Find and assign the info UI elements.
        m_InfoContainer = root.Q<VisualElement>("InfoScreen");
        m_InfoButton = root.Q<Button>("Info");
        m_InfoButtonBlink = root.Q<VisualElement>("InfoBlink");
        m_InfoCloseButton = root.Q<Button>("Info_Close");
        m_InfoPerson = root.Q<VisualElement>("Person");
        m_InfoPersonIMG = root.Q<VisualElement>("PersonIMG");
        m_InfoBubble = root.Q<VisualElement>("Explanation");
        m_InfoHeader = root.Q<Label>("InfoHeader");
        m_InfoText = root.Q<Label>("InfoText");
        m_InfoScrim = root.Q<VisualElement>("Scrim_Info");
        m_PrevTipButton = root.Q<Button>("prevtip");
        m_NextTipButton = root.Q<Button>("nexttip");
        m_InfoAmount = root.Q<Label>("InfoAmount");
        

        // Hide the info container.
        m_InfoContainer.style.display = DisplayStyle.None;
        m_InfoAmount.style.display = DisplayStyle.None;

        // Register click event callbacks.
        m_InfoButton.RegisterCallback<ClickEvent>(OnInfoButtonClicked);

        m_TooltipStack = FindObjectOfType<TooltipStack>();
    }

    // Event handler for the info button click event.
    private void OnInfoButtonClicked(ClickEvent evt)
    {
        UpdateTypingSpeed();
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        // Show the info container.
        m_InfoContainer.style.display = DisplayStyle.Flex;
        StopAllCoroutines();
        m_InfoButtonBlink.RemoveFromClassList("info-scrim-blink");
        m_IsInfoButtonBlinking = false;

        if (m_TooltipStack != null && m_TooltipStack.tooltipQueue.Count > 0)
        {
            m_InfoAmount.style.display = DisplayStyle.Flex;
            m_InfoAmount.text = "1/" + m_TooltipStack.tooltipQueue.Count;
        }
        else
        {
            m_InfoAmount.style.display = DisplayStyle.None;
        }

        // Add classes to animate the person, info bubble, and scrim overlay.
        m_InfoPerson.AddToClassList("person_right");
        m_InfoBubble.AddToClassList("explaination_left");
        m_InfoScrim.AddToClassList("scrim_faded");

        int femaleTrainer = PlayerPrefs.GetInt("FemaleTrainer", 1);
        if (femaleTrainer == 1)
        {
            m_InfoPersonIMG.RemoveFromClassList("male-trainer");
            m_InfoPersonIMG.AddToClassList("female-trainer");
        }
        else
        {
            m_InfoPersonIMG.RemoveFromClassList("female-trainer");
            m_InfoPersonIMG.AddToClassList("male-trainer");
        }

        // Clear the info header and text.
        m_InfoHeader.text = string.Empty;
        m_InfoText.text = string.Empty;

        m_PersonAnimationDuration = m_InfoPerson.resolvedStyle.transitionDuration.FirstOrDefault().value;

        StartCoroutine(AnimatePerson(m_PersonAnimationDuration));
        m_InfoCloseButton.RegisterCallback<ClickEvent>(OnInfoCloseButtonClicked);
        m_PrevTipButton.RegisterCallback<ClickEvent>(OnPrevTipButtonClicked);
        m_NextTipButton.RegisterCallback<ClickEvent>(OnNextTipButtonClicked);
    }

    // Coroutine to animate the person after a delay.
    private IEnumerator AnimatePerson(float time)
    {
        yield return new WaitForSeconds(time);
        UpdateTypingSpeed();
        // Toggle classes to animate the person.
        m_InfoPersonIMG.AddToClassList("person_img_speak");

        // Register transition end event callback.
        m_InfoPersonIMG.RegisterCallback<TransitionEndEvent>(AnimatePersonBack);

        // Write the info header and text.
        WriteText();


        StartCoroutine(StopAnimatePerson(m_HeaderAnimationDuration + 0.1f + m_TextAnimationDuration));
    }

    // Event handler for the previous tip button click event.
    private void OnPrevTipButtonClicked(ClickEvent evt)
    {
        UpdateTypingSpeed();
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        DOTween.KillAll();
        StopAllCoroutines();
        if (m_TooltipStack != null && m_TooltipStack.tooltipQueue.Count > 0)
        {
            m_TooltipStack.ShowPreviousTooltip();
            m_InfoAmount.text = m_TooltipStack.GetCurrentTooltipIndex() + 1 + "/" + m_TooltipStack.tooltipQueue.Count;
        }

        StartCoroutine(AnimatePerson(0));
    }

    // Event handler for the next tip button click event.
    private void OnNextTipButtonClicked(ClickEvent evt)
    {
        UpdateTypingSpeed();
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        DOTween.KillAll();
        StopAllCoroutines();

        if (m_TooltipStack != null && m_TooltipStack.tooltipQueue.Count > 0)
        {
            m_TooltipStack.ShowNextTooltip();
            m_InfoAmount.text = m_TooltipStack.GetCurrentTooltipIndex() + 1 + "/" + m_TooltipStack.tooltipQueue.Count;
        }

        StartCoroutine(AnimatePerson(0));
    }

    // Event handler for the person animation end event.
    private void AnimatePersonBack(TransitionEndEvent evt)
    {
        m_InfoPersonIMG.ToggleInClassList("person_img_speak");
    }

    // Coroutine to stop the person animation after a delay.
    private IEnumerator StopAnimatePerson(float time)
    {
        yield return new WaitForSeconds(time);
        m_InfoPersonIMG.UnregisterCallback<TransitionEndEvent>(AnimatePersonBack);
        m_InfoPersonIMG.RemoveFromClassList("person_img_speak");
    }

    // Coroutine to speak the info text after a delay.
    private IEnumerator SpeakDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        m_InfoText.text = string.Empty;
        DOTween.To(() => m_InfoText.text, x => m_InfoText.text = x, tooltipText, m_TextAnimationDuration).SetEase(Ease.Linear);
    }

    // Write the info header and text.
    private void WriteText()
    {
        m_InfoHeader.text = string.Empty;
        m_InfoText.text = string.Empty;

        DOTween.To(() => m_InfoHeader.text, x => m_InfoHeader.text = x, tooltipHeader, m_HeaderAnimationDuration).SetEase(Ease.Linear);

        StartCoroutine(SpeakDelayed(m_HeaderAnimationDuration + 0.1f));
    }

    // Event handler for the info close button click event.
    private void OnInfoCloseButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        // Remove classes to animate the person, info bubble, and scrim overlay.
        m_InfoPerson.RemoveFromClassList("person_right");
        m_InfoBubble.RemoveFromClassList("explaination_left");
        m_InfoScrim.RemoveFromClassList("scrim_faded");
        DOTween.KillAll();
        StopAllCoroutines();
        StartCoroutine(StopAnimatePerson(0));
        StartCoroutine(InfoContainerDelayed(m_PersonAnimationDuration));
        m_InfoCloseButton.UnregisterCallback<ClickEvent>(OnInfoCloseButtonClicked);
    }

    // Coroutine to hide the info container after a delay.
    private IEnumerator InfoContainerDelayed(float time)
    {
        yield return new WaitForSeconds(time);
        m_InfoContainer.style.display = DisplayStyle.None;
    }

    private bool m_IsInfoButtonBlinking = false;
    /// <summary>
    /// Starts the info button blink animation.
    /// </summary>
    public void StartInfoButtonBlink()
    {
        UIControllerHapticFeedback.AndroidVibratePattern(UIControllerHapticFeedback.LongButtonVibrationPattern, -1);
        if (!m_IsInfoButtonBlinking)
        {
            m_IsInfoButtonBlinking = true;
            StartCoroutine(InfoButtonBlinkBack());   
        }
    }

    // Coroutine for the info button blink animation.
    private IEnumerator InfoButtonBlinkBack()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            m_InfoButtonBlink.ToggleInClassList("info-scrim-blink");
        }
    }

    private void UpdateTypingSpeed()
    {
        m_TextAnimationDuration = tooltipText.Length / TypingSpeed;
        m_HeaderAnimationDuration = tooltipHeader.Length / TypingSpeed;
    }
}