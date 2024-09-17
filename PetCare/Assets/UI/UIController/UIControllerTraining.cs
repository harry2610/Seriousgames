using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIControllerTraining : MonoBehaviour
{
    public ResourceListSO resourceList;
    private RadioButton m_BehaviourButton;
    private RadioButton m_CommandsButton;
    private RadioButton m_NeedsButton;
    private Button m_CloseButton;
    private VisualElement m_BehaviourTrainingContainer;
    private VisualElement m_CommandsTrainingContainer;
    private VisualElement m_NeedsContainer;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        m_BehaviourTrainingContainer = root.Q<VisualElement>("BehaviourTrainingContainer");
        m_CommandsTrainingContainer = root.Q<VisualElement>("CommandsTrainingContainer");
        m_NeedsContainer = root.Q<VisualElement>("NeedsContainer");
        m_BehaviourButton = root.Q<RadioButton>("BehaviourButton");
        m_CommandsButton = root.Q<RadioButton>("CommandsButton");
        m_NeedsButton = root.Q<RadioButton>("NeedsButton");
        m_CloseButton = root.Q<Button>("Close");
        m_CloseButton.clicked += () => Destroy(gameObject);
        m_BehaviourButton.RegisterValueChangedCallback(v => ChangePageVisibility(m_BehaviourTrainingContainer, v.newValue));
        m_CommandsButton.RegisterValueChangedCallback(v => ChangePageVisibility(m_CommandsTrainingContainer, v.newValue));
        m_NeedsButton.RegisterValueChangedCallback(v => ChangePageVisibility(m_NeedsContainer, v.newValue));
        var gameState = GameStateManager.Instance.gameState;
        foreach (var training in gameState.dogs[0].conditions.Where(c => c.type.isTrainable))
        {
            var card = new VisualElement();
            card.AddToClassList("training-card");
            var bar = new ProgressBar();
            bar.lowValue = (float)training.type.minimumValue;
            bar.highValue = (float)training.type.maximumValue;
            bar.SetValueWithoutNotify((float)training.value);
            bar.title = training.type.title;
            Debug.Log($"Add {training.type.title}");
            foreach (var milestone in training.type.conditionRanges)
            {
                var label = new Label(milestone.title);
                if (milestone.minThreshold <= 5.0f)
                    label.style.left = new StyleLength(new Length(12.0f));
                else
                    label.style.left = new StyleLength(Length.Percent((float)milestone.minThreshold));
                Debug.Log($"Milestone {milestone.title} at {milestone.minThreshold}");
                bar.Add(label);
            }
            card.Add(bar);
            m_BehaviourTrainingContainer.Add(card);
        }
        foreach (var command in gameState.dogs[0].commands)
        {
            var card = new VisualElement();
            card.AddToClassList("condition-card");
            var icon = new VisualElement();
            icon.AddToClassList("icon");
            icon.style.backgroundImage = new StyleBackground(command.activity.icon);
            var info = new VisualElement();
            info.AddToClassList("info");
            var title = new Label(command.phrases);
            var bar = new ProgressBar();
            bar.lowValue = 0.0f;
            bar.highValue = 100.0f;
            bar.SetValueWithoutNotify((float)command.rehearsed);
            bar.title = $"{command.rehearsed:N0}%";
            info.Add(title);
            info.Add(bar);
            card.Add(icon);
            card.Add(info);
            m_CommandsTrainingContainer.Add(card);
        }
        foreach (var need in gameState.dogs[0].conditions.Where(c => !c.type.isTrainable))
        {
            var card = new VisualElement();
            card.AddToClassList("condition-card");
            var icon = new VisualElement();
            icon.AddToClassList("icon");
            icon.style.backgroundImage = new StyleBackground(need.type.icon);
            var info = new VisualElement();
            info.AddToClassList("info");
            var title = new Label(need.type.title);
            var bar = new ProgressBar();
            bar.lowValue = (float)need.type.minimumValue;
            bar.highValue = (float)need.type.maximumValue;
            bar.SetValueWithoutNotify((float) need.value);
            bar.title = $"{need.value:N0}{need.type.unit}";
            info.Add(title);
            info.Add(bar);
            card.Add(icon);
            card.Add(info);
            m_NeedsContainer.Add(card);
        }
    }

    void ChangePageVisibility(VisualElement element, bool enabled)
    {
        if (enabled)
            element.style.display = DisplayStyle.Flex;
        else
            element.style.display = DisplayStyle.None;
    }
}
