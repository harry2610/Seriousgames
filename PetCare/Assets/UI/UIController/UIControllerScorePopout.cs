using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using System;
using System.Threading.Tasks;

/// <summary>
/// Controls the coin popout UI element.
/// Use this if you use the CoinPopoutContainer in your UI but don't have the UIControllerMenu script.
/// </summary>
public class UIControllerScorePopout : MonoBehaviour
{
    private VisualElement m_ScorePopoutContainer; // The container for the coin popout UI element.
    private VisualElement m_ScorePopout; // The coin popout UI element.
    private Label m_ScorePopoutText; // The text label for the coin popout.

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        m_ScorePopoutContainer = root.Q<VisualElement>("ScorePopoutContainer");
        m_ScorePopout = root.Q<VisualElement>("ScorePopout");
        m_ScorePopoutText = root.Q<Label>("ScorePopoutLabel");

        m_ScorePopoutContainer.style.display = DisplayStyle.None;

        RewardManager.instance.onScoreChanged.AddListener(OnScoreChanged);
    }

    private void OnScoreChanged(int amount)
    {
        m_ScorePopout.RemoveFromClassList("CoinPopout_active");
        AudioManager.Instance.PlaySoundEffectByIndex(6);
        StartCoroutine(ScorePopoutAnimation(amount));
    }

    private IEnumerator ScorePopoutAnimation(int amount)
    {
        yield return new WaitForSeconds(0.05f);
        m_ScorePopoutText.text = amount > 0 ? "+" + amount : amount.ToString();
        m_ScorePopout.AddToClassList("CoinPopout_active");
        m_ScorePopoutContainer.style.display = DisplayStyle.Flex;
        StartCoroutine(ScorePopoutAnimationEnd());
    }

    private IEnumerator ScorePopoutAnimationEnd()
    {
        yield return new WaitForSeconds(2.5f);
        m_ScorePopoutContainer.style.display = DisplayStyle.None;
        m_ScorePopout.RemoveFromClassList("CoinPopout_active");
    }
}
