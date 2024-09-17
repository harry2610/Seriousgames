using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;


public class EndingUI : MonoBehaviour
{
    private VisualElement root;
    private Label reasonLabel;
    private VisualElement endingScreen;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        endingScreen = root.Q<VisualElement>("EndingScreen");
        reasonLabel = root.Q<Label>("Reason");
        var tryAgainButton = root.Q<Button>("TryAgain");

        if (endingScreen == null) Debug.LogError("EndingScreen not found");
        if (reasonLabel == null) Debug.LogError("ReasonLabel not found");

        tryAgainButton.clicked += OnTryAgainClicked;
    }

    private void Start()
    {

        string endReason = PlayerPrefs.GetString("EndGameReason", "Grund nicht gefunden");
        ShowEnding(endReason);
    }

    public void ShowEnding(string reason)
    {
        Debug.Log("ShowEnding called with reason: " + reason);
        if (reasonLabel != null)
        {
            reasonLabel.text = reason;
        }
        else
        {
            Debug.LogError("ReasonLabel is null when attempting to set text");
        }

        if (endingScreen != null)
        {
            endingScreen.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.LogError("EndingScreen is null when attempting to show it");
        }
    }

    private void OnTryAgainClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeMenu");
    }
}
