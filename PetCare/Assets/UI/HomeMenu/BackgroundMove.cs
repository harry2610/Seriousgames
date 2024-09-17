using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundMove : MonoBehaviour
{
    private VisualElement m_BackgroundContainer;
    private float m_Time = 0.0f;
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        m_BackgroundContainer = root.Q<VisualElement>("BackgroundContainer");
        m_Time = 0.0f;
    }
    void Update()
    {
        var posX = new BackgroundPosition(BackgroundPositionKeyword.Left, m_Time * 16f);
        var posY = new BackgroundPosition(BackgroundPositionKeyword.Left, m_Time * 8f);
        m_BackgroundContainer.style.backgroundPositionX = posX;
        m_BackgroundContainer.style.backgroundPositionY = posY;
        m_Time += Time.deltaTime;
        if (m_Time >= 16f)
            m_Time -= 16f;
    }
}
