using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;
using GameState;

public class UIControllerQuickTip : MonoBehaviour
{

    private VisualElement m_QuickTipContainer; // The container for the quick tip UI elements.
    private Label m_QuickTipHeader; // The visual element representing the header in the quick tip.
    private Label m_QuickTipText; // The visual element representing the text in the quick tip.
    private VisualElement m_QuickTipBubble; // The visual element representing the quick tip bubble.
    private VisualElement m_QuickTipScrim; // The visual element representing the scrim overlay in the quick tip.
    private Button m_QuickTipButton; // The button for the quick tip.
    private float m_BubbleAnimationDuration = 0.5f; // The speed of the bubble animation.
    // private int m_TextLengthCutOff = 75; // The length of the text before it is typed out.
    private string m_RestOfText; // The rest of the text that is not shown in the quick tip.

    /// <summary>
    /// The speed at which the text is typed out.
    /// Higher values mean faster typing.
    /// </summary>
    public float TypingSpeed = 25f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the root visual element.
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Find and assign the quick tip UI elements.
        m_QuickTipContainer = root.Q<VisualElement>("QuickInfoScreen");
        m_QuickTipHeader = root.Q<Label>("QuickInfoHeader");
        m_QuickTipText = root.Q<Label>("QuickInfoText");
        m_QuickTipBubble = root.Q<VisualElement>("QuickInfoExplanation");
        m_QuickTipScrim = root.Q<VisualElement>("Scrim_QuickInfo");
        m_QuickTipButton = root.Q<Button>("QuickInfoButton");

        GlobalEvents.Instance.triggerQuickTip.AddListener(ShowQuickTip);
    }

    private void ShowQuickTip(QuicktipSO quicktip)
    {
        bool enabled = PlayerPrefs.GetInt("QuickTipsEnabled", 1) == 1;
        if (!enabled)
        {
            return;
        }

        Debug.Log("Show Quick Tip: " + quicktip.heading);
        QuickTipState quickTipState = GameStateManager.Instance.gameState.quickTips.Single(x => x.tooltip == quicktip);
        if (quickTipState != null && quickTipState.timesToShowLeft <= 0)
        {
            return;
        }
        quickTipState.timesToShowLeft--;

        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        AudioManager.Instance.PlaySoundEffectByIndex(5);

        m_QuickTipContainer.style.display = DisplayStyle.Flex;
        m_QuickTipHeader.style.display = DisplayStyle.Flex;
        m_QuickTipBubble.AddToClassList("explaination_left");
        m_QuickTipScrim.AddToClassList("scrim_faded");

        m_QuickTipHeader.text = string.Empty;
        m_QuickTipText.text = string.Empty;

        // m_TextLengthCutOff = 75;

        m_RestOfText = quicktip.text;

        m_BubbleAnimationDuration = m_QuickTipBubble.resolvedStyle.transitionDuration.FirstOrDefault().value;

        StartCoroutine(TypeHeader(quicktip.heading, quicktip.text, m_BubbleAnimationDuration));
    }

    private IEnumerator TypeHeader(string header, string text, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        float headerTypingSpeed = GetTypingSpeed(header);
        m_QuickTipHeader.text = string.Empty;
        DOTween.To(() => m_QuickTipHeader.text, x => m_QuickTipHeader.text = x, header, headerTypingSpeed).SetEase(Ease.Linear);
        StartCoroutine(TypeText(text, headerTypingSpeed));
    }

    private string m_NewText;
    private IEnumerator TypeText(string text, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        m_NewText = text;
        float textTypingSpeed = GetTypingSpeed(m_NewText);

        int cutOffIndex = text.IndexOf('.');

        if (cutOffIndex != -1 && text.Length > cutOffIndex + 2)
        {
            m_NewText = text.Substring(0, cutOffIndex) + "...";
            m_RestOfText = text.Substring(cutOffIndex + 2);

            textTypingSpeed = GetTypingSpeed(m_NewText);
            StartCoroutine(SwitchButtonAfterDelay(1, textTypingSpeed));
        }
        else
        {
            StartCoroutine(SwitchButtonAfterDelay(2, textTypingSpeed));
        }
        SwitchButton(0);

        m_QuickTipText.text = string.Empty;
        DOTween.To(() => m_QuickTipText.text, x => m_QuickTipText.text = x, m_NewText, textTypingSpeed).SetEase(Ease.Linear);
    }

    private IEnumerator SwitchButtonAfterDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchButton(index);
    }

    private void SwitchButton(int index)
    {
        m_QuickTipButton.UnregisterCallback<ClickEvent>(CompleteQuickTip);
        m_QuickTipButton.UnregisterCallback<ClickEvent>(ContinueQuickTip);
        m_QuickTipButton.UnregisterCallback<ClickEvent>(CloseQuickTip);
        switch (index)
        {
            case 0:
                m_QuickTipButton.RegisterCallback<ClickEvent>(CompleteQuickTip);
                break;
            case 1:
                m_QuickTipButton.RegisterCallback<ClickEvent>(ContinueQuickTip);
                break;
            case 2:
                m_QuickTipButton.RegisterCallback<ClickEvent>(CloseQuickTip);
                break;
            default:
                break;
        }
    }

    private void CompleteQuickTip(ClickEvent evt)
    {
        StopAllCoroutines();
        DOTween.KillAll();
        m_QuickTipText.text = m_NewText;
        if (m_NewText.EndsWith("..."))
        {
            SwitchButton(1);
        }
        else
        {
            SwitchButton(2);
        }
    }

    private void ContinueQuickTip(ClickEvent evt)
    {
        StopAllCoroutines();
        // m_TextLengthCutOff = 125;
        m_QuickTipHeader.style.display = DisplayStyle.None;
        StartCoroutine(TypeText(m_RestOfText));
    }

    private void CloseQuickTip(ClickEvent evt)
    {
        m_QuickTipScrim.RemoveFromClassList("scrim_faded");
        m_QuickTipBubble.RemoveFromClassList("explaination_left");
        SwitchButton(-1);

        m_QuickTipContainer.style.display = DisplayStyle.None;
    }

    private float GetTypingSpeed(string text)
    {
        return text.Length / TypingSpeed;
    }
}
