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
public class UIControllerCoinPopout : MonoBehaviour
{
    private VisualElement m_CoinPopoutContainer; // The container for the coin popout UI element.
    private VisualElement m_CoinPopout; // The coin popout UI element.
    private Label m_CoinPopoutText; // The text label for the coin popout.

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        m_CoinPopoutContainer = root.Q<VisualElement>("CoinPopoutContainer");
        m_CoinPopout = root.Q<VisualElement>("CoinPopout");
        m_CoinPopoutText = root.Q<Label>("CoinPopoutLabel");

        m_CoinPopoutContainer.style.display = DisplayStyle.None;

        MoneySystem.instance.onCoinsChanged.AddListener(OnCoinsChanged);
    }

    private void OnCoinsChanged(int amount)
    {
        m_CoinPopout.RemoveFromClassList("CoinPopout_active");
        AudioManager.Instance.PlaySoundEffectByIndex(7);
        StartCoroutine(CoinPopoutAnimation(amount));
    }

    private IEnumerator CoinPopoutAnimation(int amount)
    {
        yield return new WaitForSeconds(0.05f);
        m_CoinPopoutText.text = amount > 0 ? "+" + amount : amount.ToString();
        m_CoinPopout.AddToClassList("CoinPopout_active");
        m_CoinPopoutContainer.style.display = DisplayStyle.Flex;
        StartCoroutine(CoinPopoutAnimationEnd());
    }

    private IEnumerator CoinPopoutAnimationEnd()
    {
        yield return new WaitForSeconds(2.5f);
        m_CoinPopoutContainer.style.display = DisplayStyle.None;
        m_CoinPopout.RemoveFromClassList("CoinPopout_active");
    }
}
