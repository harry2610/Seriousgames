using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIControllerMap : MonoBehaviour
{
    private Button m_LivingRoomButton;
    private Button m_ShopButton;
    private Button m_VetButton;
    private Button m_ParkButton;
    private Button m_TrainingButton;
    private Button m_InfoButton;
    private Button m_MapButton;

    public QuicktipSO tipForMap;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        m_LivingRoomButton = root.Q<Button>("Zuhause");
        m_ShopButton = root.Q<Button>("Shop");
        m_VetButton = root.Q<Button>("Tierarzt");
        m_ParkButton = root.Q<Button>("Park");
        m_TrainingButton = root.Q<Button>("Training");
        m_InfoButton = root.Q<Button>("Info");
        m_MapButton = root.Q<Button>("Map");

        m_TrainingButton.style.display = DisplayStyle.None;
        m_InfoButton.style.display = DisplayStyle.None;
        m_MapButton.style.display = DisplayStyle.None;

        m_LivingRoomButton.clicked += () => GameStateManager.LoadScene("Home");
        m_ShopButton.clicked += () => GameStateManager.LoadScene("ShopSample");
        m_VetButton.clicked += () => GameStateManager.LoadScene("Vet");
        m_ParkButton.clicked += () => GameStateManager.LoadScene("Park");

        StartCoroutine(ShowMapTip());
    }

    private IEnumerator ShowMapTip()
    {
        yield return new WaitForSeconds(0.1f);
        GlobalEvents.Instance.triggerQuickTip.Invoke(tipForMap);
    }
}