using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace walkie
{
    public enum IsVisible
    {
        Visible = 0,
        Hidden = 1,
    }

    public delegate void changeVisibilityDelegate(IsVisible isVisible);

    public class UIControllerWalkie : MonoBehaviour
    {
        public WalkieManager walkieManager;
        public AudioClip walkieSound;
        private Button m_WalkieButton;
        private VisualElement bottom;

        public void StartWalkieAction(ClickEvent clickEvent)
        {
            AudioManager.Instance.PlayMusic(walkieSound);
            walkieManager.StartWalkie(ChangeVisibility);
        }

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            m_WalkieButton = root.Q<Button>("Walkie");
            m_WalkieButton.style.display = DisplayStyle.Flex;
            m_WalkieButton.RegisterCallback<ClickEvent>(StartWalkieAction);
            bottom = root.Q<VisualElement>("bottom");
        }

        public void ChangeVisibility(IsVisible isVisible)
        {
            switch (isVisible)
            {
                case IsVisible.Visible:
                    m_WalkieButton.style.display = DisplayStyle.Flex;
                    bottom.style.display = DisplayStyle.Flex;
                    break;
                case IsVisible.Hidden:
                    m_WalkieButton.style.display = DisplayStyle.None;
                    bottom.style.display = DisplayStyle.None;
                    break;
            }
        }

    }
}
