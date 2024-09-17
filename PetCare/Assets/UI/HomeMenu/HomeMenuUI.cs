using Dog;
using GameState;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HomeMenuUI : MonoBehaviour
{
    public enum Page
    {
        MainMenu,
        StartConditions,
        AnimalShelter,
    }
    private Page m_CurrentPage;
    private float m_ScrollPos;
    private float m_Velocity;
    private VisualElement m_Screens;
    private CustomRenderTexture m_RenderTexture;
    public SkinnedMeshRenderer dogMesh;
    public ResourceListSO resourceList;
    private VisualElement m_DogOverlay;
    private VisualElement m_CharacterTraitsArea;
    private VisualElement m_StartGames;
    private VisualElement m_SaveGameContainer;
    private Label m_DogDescriptionLabel;
    private TextField m_DogNameTextField;
    private int m_SelectedDog;
    private StartGameSO m_SelectedStartGame;
    private Button m_GoToAnimalShelterButton;
    private Toggle m_FastMode;
    void Start()
    {
        m_CurrentPage = Page.MainMenu;
        m_ScrollPos = 0f;
        m_Velocity = 0f;

        var root = GetComponent<UIDocument>().rootVisualElement;
        m_Screens = root.Q<VisualElement>("Screens");
        m_DogNameTextField = root.Q<TextField>("DogName");
        m_DogDescriptionLabel = root.Q<Label>("DogDescription");
        m_CharacterTraitsArea = root.Q<VisualElement>("CharacterTraits");
        m_StartGames = root.Q<RadioButtonGroup>("StartGames");
        m_SaveGameContainer = root.Q<VisualElement>("SaveGameContainer");
        var newGameButton = root.Q<Button>("NewGame");
        var backToMenuButton = root.Q<Button>("BackToMenu");
        var backToStartConditionsButton = root.Q<Button>("BackToStartConditions");
        m_GoToAnimalShelterButton = root.Q<Button>("GoToAnimalShelter");
        var nextDogButton = root.Q<Button>("NextDog");
        var prevDogButton = root.Q<Button>("PrevDog");
        var adoptButton = root.Q<Button>("Adopt");
        m_FastMode = root.Q<Toggle>("FastMode");
        m_GoToAnimalShelterButton.SetEnabled(false);
        newGameButton.clicked += () => m_CurrentPage = Page.StartConditions;
        backToMenuButton.clicked += () => m_CurrentPage = Page.MainMenu;
        backToStartConditionsButton.clicked += () => m_CurrentPage = Page.StartConditions;
        m_GoToAnimalShelterButton.clicked += () => m_CurrentPage = Page.AnimalShelter;
        nextDogButton.clicked += NextDog;
        prevDogButton.clicked += PreviousDog;
        adoptButton.clicked += AdoptNewDog;


        m_DogOverlay = root.Q<VisualElement>("DogOverlay");
        m_RenderTexture = new CustomRenderTexture(Screen.currentResolution.width, Screen.currentResolution.height, RenderTextureFormat.ARGB32);
        m_RenderTexture.antiAliasing = 4;
        m_RenderTexture.depth = 24;
        m_RenderTexture.updateMode = CustomRenderTextureUpdateMode.Realtime;
        var isCreated = m_RenderTexture.Create();
        Camera.main.ResetAspect();
        var posY = new BackgroundPosition(BackgroundPositionKeyword.Bottom, 0f);
        Camera.main.targetTexture = m_RenderTexture;
        m_DogOverlay.style.backgroundPositionY = posY;
        m_DogOverlay.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(m_RenderTexture));
        m_SelectedDog = 0;
        m_SelectedStartGame = null;
        LoadSaveGameList();
        LoadStartGamesScreen();
        LoadDogDescription();
    }
    private void OnDestroy()
    {
        m_RenderTexture.Release();
    }
    void LoadSaveGameList()
    {
        var names = GameStateManager.GetSaveGameNames();
        foreach (string name in names)
        {
            Debug.Log(name);
            var button = new Button();
            button.text = name;
            button.clicked += () => LoadGame(name);
            m_SaveGameContainer.Add(button);
        }
    }
    void LoadGame(string name)
    {
        GameStateManager.Instance.Load(name);
        SceneManager.LoadScene("Home");
    }
    void LoadStartGamesScreen()
    {
        foreach (StartGameSO startGame in resourceList.startGames)
        {
            var startGameCard = new RadioButton(startGame.title);
            startGameCard.Add(new Label(startGame.description));
            startGameCard.RegisterValueChangedCallback(v => { if (v.newValue) { UpdateSelectedStartGame(startGame);  } });
            m_StartGames.Add(startGameCard);
        }
    }
    void UpdateSelectedStartGame(StartGameSO startGame)
    {
        m_SelectedStartGame = startGame;
        m_GoToAnimalShelterButton.SetEnabled(true);
        Debug.Log($"New start game is {startGame.title}");
    }
    void LoadDogDescription()
    {
        AdoptableDogSO dog = resourceList.adoptableDogs[m_SelectedDog];
        m_DogDescriptionLabel.text = dog.description;
        m_DogNameTextField.SetValueWithoutNotify(dog.givenName);

        /*var litShader = Shader.Find("Universal Render Pipeline/Lit");
        var furMaterial = new Material(litShader);
        furMaterial.SetColor("_BaseColor", dog.breed.colorA);*/
        dogMesh.material = dog.breed.material;
        m_CharacterTraitsArea.Clear();
        foreach (Dog.Habit habit in dog.habits)
        {
            var traitCard = new VisualElement();
            traitCard.AddToClassList("condition-card");
            var icon = new VisualElement();
            icon.AddToClassList("icon");
            icon.style.backgroundImage = new StyleBackground(habit.trait.icon);

            var info = new VisualElement();
            info.AddToClassList("info");
            var title = new Label(habit.trait.title);
            var bar = new ProgressBar();
            bar.lowValue = 0.0f;
            bar.highValue = 100.0f;
            bar.SetValueWithoutNotify((float)habit.pronouncedness);
            bar.title = $"{habit.pronouncedness:N0}%";
            info.Add(title);
            info.Add(bar);
            traitCard.Add(icon);
            traitCard.Add(info);
            m_CharacterTraitsArea.Add(traitCard);
        }
        var spacer = new VisualElement();
        spacer.style.height = new StyleLength(64);
        m_CharacterTraitsArea.Add(spacer);
    }
    void PreviousDog()
    {
        m_SelectedDog -= 1;
        if (m_SelectedDog < 0)
            m_SelectedDog += resourceList.adoptableDogs.Length;
        LoadDogDescription();

    }
    void NextDog()
    {
        m_SelectedDog += 1;
        if (m_SelectedDog >= resourceList.adoptableDogs.Length)
            m_SelectedDog -= resourceList.adoptableDogs.Length;
        LoadDogDescription();
    }
    float ScrollPosForPage()
    {
        switch (m_CurrentPage)
        {
            case Page.MainMenu: return 0f;
            case Page.StartConditions: return -1f;
            case Page.AnimalShelter: return -2f;
        }
        return 0f;
    }
    void AdoptNewDog()
    {
        var startGame = m_SelectedStartGame;
        AdoptableDogSO dog = resourceList.adoptableDogs[m_SelectedDog];
        var dogState = new GameState.DogState();
        dogState.breed = dog.breed;
        dogState.habits = new GameState.HabitState[dog.habits.Length];
        for (int i = 0; i < dog.habits.Length; i += 1)
        {
            dogState.habits[i] = new GameState.HabitState()
            {
                pronouncedness = dog.habits[i].pronouncedness,
                characterTrait = dog.habits[i].trait,
            };
        }
        dogState.name = m_DogNameTextField.value.Trim();
        dogState.commands = new GameState.CommandState[dog.commands.commands.Length];
        for (int i = 0; i < dog.commands.commands.Length; i += 1)
        {
            dogState.commands[i] = new GameState.CommandState();
            dogState.commands[i].activity = dog.commands.commands[i].activity;
            if (dog.commands.commands[i].phrases == "[name]")
                dogState.commands[i].phrases = dogState.name.ToLower();
            else
                dogState.commands[i].phrases = (string)dog.commands.commands[i].phrases.Clone();
            dogState.commands[i].rehearsed = dog.commands.commands[i].rehearsed;
        }
        dogState.conditions = new GameState.ConditionValue[resourceList.conditions.Length];
        for (int i = 0; i < resourceList.conditions.Length; i += 1)
        {
            dogState.conditions[i] = new ConditionValue();
            dogState.conditions[i].type = resourceList.conditions[i];
            dogState.conditions[i].value = resourceList.conditions[i].initialValue;
        }
        dogState.lastConditionUpdate = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        dogState.furConditionTexture = new Texture2D(256, 256, TextureFormat.RG16, false);
        var texData = dogState.furConditionTexture.GetPixelData<byte>(0);
        for (int i = 0; i < 256*256; i += 1)
        {
            texData[i * 2] = 255;
            texData[i * 2 + 1] = 0;
        }
        dogState.furConditionTexture.Apply(true, false);
        dogState.lastFurUpdate = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        dogState.nextIllnessTime = 0;
        var items = new GameState.ItemState[startGame.items.Length];
        for (int i = 0; i < startGame.items.Length; i += 1)
        {
            items[i] = new GameState.ItemState()
            {
                amount = 1,
                slot = (uint)i,
                type = startGame.items[i],
            };
        }
        GameState.QuickTipState[] quicktips = new GameState.QuickTipState[resourceList.quicktips.Length];
        for (int i = 0; i < resourceList.quicktips.Length; i += 1)
        {
            quicktips[i] = new GameState.QuickTipState()
            {
                tooltip = resourceList.quicktips[i],
                timesToShowLeft = resourceList.quicktips[i].maxShowTimes,
            };
        }
        GameState.GameState gameState = new GameState.GameState()
        {
            fileVersion = 0,
            lastSaveTime = 0,
            simulationDayLength = 24.0f,
            inventory = new GameState.InventoryState()
            {
                coins = startGame.coins,
                coinsPerDay = startGame.coinsPerDay,
                lastCoinsPerDay = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                lastQuizTime = 0,
                items = items,
            },
            dogs = new GameState.DogState[]
            {
                dogState
            },
            quickTips = quicktips,
        };
        if (m_FastMode.value)
            gameState.simulationDayLength = 1200.0;
        else
            gameState.simulationDayLength = 86400.0;
        GameStateManager.Instance.gameState = gameState;
        GameStateManager.Instance.SetSaveGameName(dogState.name);
        GameStateManager.LoadScene("Home");
    }
    void Update()
    {
        m_ScrollPos = Mathf.SmoothDamp(m_ScrollPos, ScrollPosForPage(), ref m_Velocity, 0.2f);
        m_Screens.style.top = new StyleLength(new Length(m_ScrollPos * 100f, LengthUnit.Percent));
    }
}
