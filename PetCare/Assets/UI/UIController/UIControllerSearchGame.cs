using GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIControllerSearchGame : MonoBehaviour
{
    private Label m_DescriptionLabel;
    private Label m_SearchLocationLabel;
    private Button m_PrevButton;
    private Button m_NextButton;
    private Button m_StartSearchGameButton;
    private SearchPoint[] m_SearchPoints;
    private int m_SearchID;
    private StaticCam m_Cam;
    private DogState m_DogState;
    private GameObject m_UI;
    public Impulses.ImpulseSO startSearchGameImpulse;
    void Start()
    {
        FindObjectOfType<UIControllerInventory>().ActivateUI(false);
        m_Cam = FindObjectOfType<StaticCam>();
        var root = GetComponent<UIDocument>().rootVisualElement;
        m_DescriptionLabel = root.Q<Label>("Description");
        m_SearchLocationLabel = root.Q<Label>("SearchLocation");
        m_StartSearchGameButton = root.Q<Button>("StartSeachGame");
        m_DogState = GameStateManager.Instance.gameState.dogs[0];
        m_StartSearchGameButton.text = $"{m_DogState.name} suchen lassen";
        m_StartSearchGameButton.clicked += StartSearchGame;
        m_PrevButton = root.Q<Button>("Prev");
        m_PrevButton.clicked += PrevSearchLocation;
        m_NextButton = root.Q<Button>("Next");
        m_NextButton.clicked += NextSearchLocation;
        m_SearchPoints = FindObjectsByType<SearchPoint>(FindObjectsSortMode.InstanceID);
        m_SearchID = 0;
        GlobalEvents.Instance.onDogCheckSearchSpot.AddListener(OnDogCheckSearchSpot);
        GlobalEvents.Instance.onDogEndedSearchGame.AddListener(OnDogEndedSearchGame);
        GlobalEvents.Instance.onDogHasNothingFound.AddListener(OnDogHasNothingFound);
        UpdateSearchLocation();
    }
    private void OnDogCheckSearchSpot(SearchPoint searchPoint)
    {
        m_DescriptionLabel.text = $"{m_DogState.name} \u00FCberpr\u00FCft {searchPoint.titleAccusative}";
    }
    private void OnDogHasNothingFound()
    {
        m_DescriptionLabel.text = $"{m_DogState.name} hat dort nichts gefunden";
    }
    private void OnDogEndedSearchGame(bool hasSomethingFound)
    {
        if (hasSomethingFound)
        {
            m_DescriptionLabel.text = $"{m_DogState.name} hat das Versteck gefunden!";
            if (TryGetComponent<ConditionContainer>(out ConditionContainer conditions))
            {
                conditions.Apply();
            }
        }
        else
        {
            m_DescriptionLabel.text = $"{m_DogState.name} hat das Spiel abgebrochen!";
        }
        Invoke("EndSearchGame", 3.0f);
    }
    private void EndSearchGame()
    {
        Destroy(gameObject);
    }
    private void NextSearchLocation()
    {
        m_SearchID += 1;
        if (m_SearchID >= m_SearchPoints.Length)
            m_SearchID = 0;
        UpdateSearchLocation();
    }
    private void PrevSearchLocation()
    {
        m_SearchID -= 1;
        if (m_SearchID < 0)
            m_SearchID = m_SearchPoints.Length - 1;
        UpdateSearchLocation();
    }
    private void UpdateSearchLocation()
    {
        var searchPoint = m_SearchPoints[m_SearchID];
        m_SearchLocationLabel.text = searchPoint.title;
        m_Cam.SetCustomTransform(searchPoint.camTransform.position, searchPoint.camTransform.rotation);
    }
    private void StartSearchGame()
    {
        m_PrevButton.style.display = DisplayStyle.None;
        m_NextButton.style.display = DisplayStyle.None;
        m_SearchLocationLabel.style.display = DisplayStyle.None;
        m_StartSearchGameButton.style.display = DisplayStyle.None;
        m_DescriptionLabel.text = $"{m_DogState.name} ist am Suchen...";
        m_SearchPoints[m_SearchID].isHidingSpot = true;
        var potentialSearchCam = GameObject.FindGameObjectWithTag("SearchCam");
        if (potentialSearchCam != null)
            m_Cam.SetInitialCameraPosition(potentialSearchCam.transform.position);
        m_Cam.SetFollowModus();
        GlobalEvents.Instance.onDogImpulse.Invoke(startSearchGameImpulse, null);
    }
    private void OnDestroy()
    {
        FindObjectOfType<UIControllerInventory>().ActivateUI(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
