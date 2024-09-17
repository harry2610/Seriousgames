using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Dog;

public class VetUI : MonoBehaviour
{
    private VisualElement vetScreen;
    private Label dogNameLabel;
    private Label dogIllnessLabel; 
    private Label healingCostLabel;
    private Button healButton;
    private Button backButton;
    private VisualElement confirmDialog;
    private Button confirmHealButton;
    private Button cancelHealButton;

    private VisualElement infoBox;
    private Label dialogTextInfo;
    private Button okButton;

    public ConditionSO illnessCondition;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        vetScreen = root.Q<VisualElement>("VetScreen");
        dogNameLabel = root.Q<Label>("DogName");
        dogIllnessLabel = root.Q<Label>("DogIllness");
        healingCostLabel = root.Q<Label>("HealingCost");
        healButton = root.Q<Button>("Heal");
        confirmDialog = root.Q<VisualElement>("ConfirmDialog");
        confirmHealButton = root.Q<Button>("ConfirmHeal");
        cancelHealButton = root.Q<Button>("CancelHeal");
        infoBox = root.Q<VisualElement>("InfoBox");
        dialogTextInfo = root.Q<Label>("DialogTextInfo");
        okButton = root.Q<Button>("OKButton");

        UpdateDogInfo();

        healButton.clicked += ShowConfirmDialog;
        confirmHealButton.clicked += HealDog;
        cancelHealButton.clicked += CloseConfirmDialog;
        okButton.clicked += CloseInfoBox;

        vetScreen.style.display = DisplayStyle.Flex;
        confirmDialog.style.display = DisplayStyle.None;
        infoBox.style.display = DisplayStyle.None; 
    }

    void UpdateDogInfo()
    {
        var dog = GameStateManager.Instance.gameState.dogs[0];
        double currentIllness = dog.GetCondition(illnessCondition); 
        int roundedIllness = Mathf.FloorToInt((float)currentIllness); 

        dogNameLabel.text = dog.name;
        dogIllnessLabel.text = $"Krankheit: {roundedIllness}/{illnessCondition.maximumValue}";
        healingCostLabel.text = $"Heilungskosten: {CalculateHealingCost(roundedIllness, illnessCondition.maximumValue)}€";
    }

    int CalculateHealingCost(double currentIllness, double maxIllness)
    {
        return (int)((currentIllness / maxIllness) * 100); 
    }

    void ShowConfirmDialog()
    {
        double currentIllness = GameStateManager.Instance.gameState.dogs[0].GetCondition(illnessCondition);

        if (currentIllness > 0)
        {
            confirmDialog.style.display = DisplayStyle.Flex;
        }
        else
        {
            ShowInfoBox("Der Hund ist bereits gesund.");
        }
    }

    void HealDog()
    {
        var dog = GameStateManager.Instance.gameState.dogs[0];
        double currentIllness = dog.GetCondition(illnessCondition);
        int healingCost = CalculateHealingCost(currentIllness, illnessCondition.maximumValue);

        if (MoneySystem.instance.coins >= healingCost)
        {
            MoneySystem.instance.RemoveCoins(healingCost);
            dog.SetCondition(illnessCondition, 0);
            UpdateDogInfo();
            ShowInfoBox("Der Hund wurde geheilt.");
        }
        else
        {
            ShowInfoBox("Nicht genug Geld, um den Hund zu heilen.");
        }

        CloseConfirmDialog();
    }

    void CloseConfirmDialog()
    {
        confirmDialog.style.display = DisplayStyle.None;
    }

    void ShowInfoBox(string message)
    {
        dialogTextInfo.text = message;
        infoBox.style.display = DisplayStyle.Flex; 
    }

    void CloseInfoBox()
    {
        infoBox.style.display = DisplayStyle.None; 
        UpdateDogInfo();
    }
}
