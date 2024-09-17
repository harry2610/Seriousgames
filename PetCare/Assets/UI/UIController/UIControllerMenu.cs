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
/// Controls the menu UI elements like settings and the leaderboard and the map button.
/// </summary>
public class UIControllerMenu : MonoBehaviour
{
    public UnityEngine.Object trainingUI;
    private VisualElement m_MenuScreen; // The container for the menu UI elements.
    private VisualElement m_MenuContainer; // The container for the menu UI elements.
    private Button m_MenuButton; // The button to open the menu.
    private Button m_ContinueButton; // The button to continue the game.
    private Button m_ContinueButton2; // The button to continue the game.
    private Button m_SettingsButton; // The button to open the settings.
    private Button m_HomeMenuButton; // The button to open the home menu.
    private Button m_TrainingButton; // The button to exit the game.
    private VisualElement m_MenuScrim; // The visual element representing the scrim overlay in the menu.
    private Button m_MapButton; // The button to open the map.
    private Button m_QuizButton; // The button to open the quiz.
    private VisualElement m_QuizQ;
    private Button m_ExitSettingsButton; // The button to exit the settings.
    private VisualElement m_SettingsContainer; // The container for the settings UI elements.
    private Slider m_MusicVolumeSlider; // The slider for the music volume.
    private Toggle m_MusicMuteToggle; // The toggle for muting the music.
    private Slider m_SoundVolumeSlider; // The slider for the sound volume.
    private Toggle m_SoundMuteToggle; // The toggle for muting the sound.
    private Toggle m_VibrationToggle; // The toggle for enabling vibration.
    private Toggle m_NotificationsToggle; // The toggle for enabling notifications.
    private Toggle m_TrainerToggle; // The toggle for enabling the female trainer.
    private Toggle m_QuickTipsToggle; // The toggle for enabling quick tips.
    private Button m_LeaderboardButton; // The button to open the leaderboard.
    private Button m_CloseLeaderboardButton; // The button to close the leaderboard.
    private Button m_UpdateLeaderboardButton; // The button to update the leaderboard.
    private VisualElement m_LeaderboardContainer; // The container for the leaderboard UI elements.
    private ScrollView m_LeaderboardList; // The list of leaderboard entries.
    private VisualElement m_UserNameInputContainer; // The container for the username input.
    private TextField m_UserNameInput; // The text field for the username input.
    private Button m_UserSubmitButton; // The button to submit the username.
    public VisualTreeAsset myVisualTreeAsset; // The visual tree asset for the leaderboard entry template.

    void Start()
    {
        // Get the root visual element.
        var root = GetComponent<UIDocument>().rootVisualElement;

        //Find and assign the menu UI elements.
        m_MenuScreen = root.Q<VisualElement>("MenuScreen");
        m_MenuContainer = root.Q<VisualElement>("Container");
        m_MenuButton = root.Q<Button>("Menu");
        m_TrainingButton = root.Q<Button>("Training");
        m_ContinueButton = root.Q<Button>("Continue");
        m_ContinueButton2 = root.Q<Button>("Continue2");
        m_SettingsButton = root.Q<Button>("Settings");
        m_HomeMenuButton = root.Q<Button>("HomeMenu");
        m_MenuScrim = root.Q<VisualElement>("Scrim_Menu");
        m_MapButton = root.Q<Button>("Map");
        m_QuizButton = root.Q<Button>("QuizOpenButton");
        m_QuizQ = root.Q<VisualElement>("QuizQ");
        m_ExitSettingsButton = root.Q<Button>("SettingsCloseButton");
        m_SettingsContainer = root.Q<VisualElement>("SettingsContainer");
        m_MusicVolumeSlider = root.Q<Slider>("MusicSlider");
        m_MusicMuteToggle = root.Q<Toggle>("MusicToggle");
        m_SoundVolumeSlider = root.Q<Slider>("SoundSlider");
        m_SoundMuteToggle = root.Q<Toggle>("SoundToggle");
        m_VibrationToggle = root.Q<Toggle>("VibrationToggle");
        m_NotificationsToggle = root.Q<Toggle>("PushToggle");
        m_TrainerToggle = root.Q<Toggle>("TrainerToggle");
        m_QuickTipsToggle = root.Q<Toggle>("TipsToggle");
        m_LeaderboardButton = root.Q<Button>("LeaderboardButton");
        m_CloseLeaderboardButton = root.Q<Button>("LeaderboardCloseButton");
        m_UpdateLeaderboardButton = root.Q<Button>("LeaderboardUpdateButton");
        m_LeaderboardContainer = root.Q<VisualElement>("LeaderboardContainer");
        m_LeaderboardList = root.Q<ScrollView>("LeadeboardList");
        m_UserNameInputContainer = root.Q<VisualElement>("UserNameInputContainer");
        m_UserNameInput = root.Q<TextField>("UserNameField");
        m_UserSubmitButton = root.Q<Button>("UserNameInputConfirm");

        // Hide the menu container.
        m_MenuScreen.style.display = DisplayStyle.None;
        // m_SettingsContainer.style.display = DisplayStyle.None;
        m_UserNameInputContainer.style.display = DisplayStyle.None;

        m_QuizButton.style.display = DisplayStyle.None;
        // Start the quiz button routine.
        StartCoroutine(QuizButtonRoutine());
        AnimateQuizQ();

        // Register click event callbacks.
        m_MenuButton.RegisterCallback<ClickEvent>(OnMenuButtonClicked);
        m_TrainingButton.RegisterCallback<ClickEvent>(OnTrainingButtonClicked);
        m_MapButton.RegisterCallback<ClickEvent>(OnMapButtonClicked);
        m_QuizButton.RegisterCallback<ClickEvent>(OnQuizButtonClicked);
        m_SettingsButton.RegisterCallback<ClickEvent>(OnSettingsButtonClicked);
        m_HomeMenuButton.RegisterCallback<ClickEvent>(OnHomeMenuButtonClicked);
        m_ExitSettingsButton.RegisterCallback<ClickEvent>(OnSettingsExitButtonClicker);

        m_MusicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
        m_MusicMuteToggle.RegisterCallback<ClickEvent>(OnMusicMuteToggleClicked);
        m_MusicMuteToggle.RegisterValueChangedCallback(OnMusicMuteToggleChanged);

        m_SoundVolumeSlider.RegisterValueChangedCallback(OnSoundVolumeChanged);
        m_SoundMuteToggle.RegisterCallback<ClickEvent>(OnSoundMuteToggleClicked);
        m_SoundMuteToggle.RegisterValueChangedCallback(OnSoundMuteToggleChanged);

        m_VibrationToggle.RegisterValueChangedCallback(OnVibrationToggleChanged);
        m_NotificationsToggle.RegisterValueChangedCallback(OnNotificationsToggleChanged);
        m_TrainerToggle.RegisterValueChangedCallback(OnTrainerToggleChanged);
        m_QuickTipsToggle.RegisterValueChangedCallback(OnQuickTipsToggleChanged);

        m_LeaderboardButton.RegisterCallback<ClickEvent>(OnLeaderboardButtonClicked);
        m_CloseLeaderboardButton.RegisterCallback<ClickEvent>(OnCloseLeaderboardButtonClicked);
        m_UpdateLeaderboardButton.RegisterCallback<ClickEvent>(OnUpdateLeaderboardButtonClicked);
        m_UserSubmitButton.RegisterCallback<ClickEvent>(OnUserSubmitButtonClicked);

        // Set Values
        m_MusicVolumeSlider.value = PlayerPrefs.GetInt("MusicMuted", 0) == 0 ? PlayerPrefs.GetFloat("MusicVolume", 1) : 0;
        m_MusicMuteToggle.value = PlayerPrefs.GetInt("MusicMuted", 0) == 0;
        m_SoundVolumeSlider.value = PlayerPrefs.GetInt("SoundMuted", 0) == 0 ? PlayerPrefs.GetFloat("SoundVolume", 1) : 0;
        m_SoundMuteToggle.value = PlayerPrefs.GetInt("SoundMuted", 0) == 0;
        UIControllerHapticFeedback.VibrationsEnabled = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
        m_VibrationToggle.value = UIControllerHapticFeedback.VibrationsEnabled;
        m_NotificationsToggle.value = PlayerPrefs.GetInt("NotificationsEnabled", 1) == 1;
        m_TrainerToggle.value = PlayerPrefs.GetInt("FemaleTrainer", 1) == 1;
        m_QuickTipsToggle.value = PlayerPrefs.GetInt("QuickTipsEnabled", 1) == 1;
    }

    // Event handler for the music volume slider value changed event.
    private void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
        m_MusicMuteToggle.value = evt.newValue > 0;
        if (AudioManager.Instance.MusicSource != null)
        {
            AudioManager.Instance.MusicSource.volume = Mathf.Pow(evt.newValue, 2);
        }
    }

    private float m_OldMusicVolume = 0; // The old music volume value.
    // Event handler for the music mute toggle click event.
    private void OnMusicMuteToggleClicked(ClickEvent evt)
    {
        if (m_MusicMuteToggle.value)
        {
            m_MusicVolumeSlider.value = m_OldMusicVolume > 0 ? m_OldMusicVolume : 1;
        }
        else
        {
            m_OldMusicVolume = m_MusicVolumeSlider.value;
            m_MusicVolumeSlider.value = 0;
        }
    }

    // Event handler for the music mute toggle value changed event.
    private void OnMusicMuteToggleChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            PlayerPrefs.SetInt("MusicMuted", 0);
            AudioManager.Instance.MusicSource.mute = false;
        }
        else
        {
            PlayerPrefs.SetInt("MusicMuted", 1);
            AudioManager.Instance.MusicSource.mute = true;
        }
    }

    // Event handler for the sound volume slider value changed event.
    private void OnSoundVolumeChanged(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat("SoundVolume", evt.newValue);
        m_SoundMuteToggle.value = evt.newValue > 0;
        if (AudioManager.Instance.SoundSource != null)
        {
            AudioManager.Instance.SoundSource.volume = Mathf.Pow(evt.newValue, 2);
        }
    }

    // The old sound volume value.
    private float m_OldSoundVolume = 0;
    // Event handler for the sound mute toggle click event.
    private void OnSoundMuteToggleClicked(ClickEvent evt)
    {
        if (m_SoundMuteToggle.value)
        {
            m_SoundVolumeSlider.value = m_OldSoundVolume > 0 ? m_OldSoundVolume : 1;
        }
        else
        {
            m_OldSoundVolume = m_SoundVolumeSlider.value;
            m_SoundVolumeSlider.value = 0;
        }
    }

    // Event handler for the sound mute toggle value changed event.
    private void OnSoundMuteToggleChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            PlayerPrefs.SetInt("SoundMuted", 0);
            AudioManager.Instance.SoundSource.mute = false;
        }
        else
        {
            PlayerPrefs.SetInt("SoundMuted", 1);
            AudioManager.Instance.SoundSource.mute = true;
        }
    }

    // Event handler for the vibration toggle value changed event.
    private void OnVibrationToggleChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            UIControllerHapticFeedback.VibrationsEnabled = true;
            PlayerPrefs.SetInt("VibrationEnabled", 1);
        }
        else
        {
            UIControllerHapticFeedback.VibrationsEnabled = false;
            PlayerPrefs.SetInt("VibrationEnabled", 0);
        }
    }

    // Event handler for the notifications toggle value changed event.
    private void OnNotificationsToggleChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            PlayerPrefs.SetInt("NotificationsEnabled", 1);
        }
        else
        {
            PlayerPrefs.SetInt("NotificationsEnabled", 0);
        }
    }

    // Event handler for the trainer toggle value changed event.
    private void OnTrainerToggleChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            PlayerPrefs.SetInt("FemaleTrainer", 1);
        }
        else
        {
            PlayerPrefs.SetInt("FemaleTrainer", 0);
        }
    }

    // Event handler for the quick tips toggle value changed event.
    private void OnQuickTipsToggleChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            PlayerPrefs.SetInt("QuickTipsEnabled", 1);
        }
        else
        {
            PlayerPrefs.SetInt("QuickTipsEnabled", 0);
        }
    }

    // Event handler for the menu button click event.
    private void OnMenuButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        // Show the menu container.
        m_MenuScreen.style.display = DisplayStyle.Flex;

        // Add classes to animate the menu and scrim overlay.
        m_MenuScrim.AddToClassList("scrim_faded");

        m_MenuContainer.AddToClassList("menu_active");

        m_LeaderboardButton.AddToClassList("leaderboard-button_active");

        m_ContinueButton.RegisterCallback<ClickEvent>(OnContinueButtonClicked);
        m_ContinueButton2.RegisterCallback<ClickEvent>(OnContinueButtonClicked);
    }

    // Event handler for the continue button click event.
    private void OnContinueButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        // Remove classes to animate the menu and scrim overlay.
        m_MenuScrim.RemoveFromClassList("scrim_faded");
        m_MenuContainer.RemoveFromClassList("menu_active");
        m_LeaderboardButton.RemoveFromClassList("leaderboard-button_active");
        m_LeaderboardContainer.RemoveFromClassList("settings_active");
        m_SettingsContainer.RemoveFromClassList("settings_active");
        float menuContainerAnimationDuration = m_MenuContainer.resolvedStyle.transitionDuration.FirstOrDefault().value;
        StartCoroutine(FadeOutMenu(menuContainerAnimationDuration / 2));
    }

    // Coroutine to fade out the menu after a delay.
    private IEnumerator FadeOutMenu(float time)
    {
        yield return new WaitForSeconds(time);
        m_MenuScreen.style.display = DisplayStyle.None;
    }

    // Event handler for the settings button click event.
    private void OnSettingsButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        // Show the settings container.
        // m_SettingsContainer.style.display = DisplayStyle.Flex;
        m_SettingsContainer.AddToClassList("settings_active");

        m_MenuContainer.RemoveFromClassList("menu_active");

        m_LeaderboardButton.RemoveFromClassList("leaderboard-button_active");
    }

    private void OnTrainingButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        Instantiate(trainingUI);
    }

    // Event handler for the settings exit button click event.
    private void OnSettingsExitButtonClicker(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        // Hide the settings container.
        // m_SettingsContainer.style.display = DisplayStyle.None;
        m_SettingsContainer.RemoveFromClassList("settings_active");

        m_MenuContainer.AddToClassList("menu_active");

        m_LeaderboardButton.AddToClassList("leaderboard-button_active");
    }

    private void OnHomeMenuButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        GameStateManager.Instance.Save();
        GameStateManager.LoadScene("HomeMenu");
    }

    // Event handler for the leaderboard button click event.
    private void OnLeaderboardButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        m_LeaderboardContainer.style.display = DisplayStyle.Flex;
        m_LeaderboardContainer.AddToClassList("settings_active");
        m_MenuContainer.RemoveFromClassList("menu_active");
        m_LeaderboardButton.RemoveFromClassList("leaderboard-button_active");
        UpdateLeaderboard();
    }

    // Request the leaderboard scores and update the leaderboard list.
    private async void UpdateLeaderboard()
    {
        LeaderboardEntries leaderboard = await Task.Run(() => LeaderboardFunctions.RequestLeaderboardScores());
        m_LeaderboardList.Clear();
        if (leaderboard != null)
        {
            foreach (LeaderboardEntry entry in leaderboard.entries)
            {
                CreateLeaderboardEntry(entry.player, entry.dog, entry.score);
            }
        }
    }

    // The list of leaderboard entries.
    private void CreateLeaderboardEntry(string playerName, string dogName, int points)
    {
        TemplateContainer template = myVisualTreeAsset.Instantiate();
        Label dogNameLabel = template.Q<Label>("LeaderboardItemDog");
        Label playerNameLabel = template.Q<Label>("LeaderboardItemPlayer");
        Label pointsLabel = template.Q<Label>("LeaderboardItemCoins");

        dogNameLabel.text = dogName;
        playerNameLabel.text = playerName;
        pointsLabel.text = points.ToString();

        m_LeaderboardList.Add(template);
    }

    // Event handler for the update leaderboard button click event.
    private void OnUpdateLeaderboardButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        m_UserNameInputContainer.style.display = DisplayStyle.Flex;
    }

    // Event handler for the user submit button click event.
    // Submit the user's score to the leaderboard.
    private async void OnUserSubmitButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);

        string dogName = GameStateManager.Instance.gameState.dogs[0].name;
        string playerName = m_UserNameInput.text;
        int points = GameStateManager.Instance.gameState.inventory.score;
        await Task.Run(() => LeaderboardFunctions.SubmitScore(new LeaderboardEntry(playerName, dogName, points)));
        m_UserNameInputContainer.style.display = DisplayStyle.None;
        UpdateLeaderboard();
    }

    // Event handler for the close leaderboard button click event.
    private void OnCloseLeaderboardButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        m_LeaderboardContainer.RemoveFromClassList("settings_active");
        m_MenuContainer.AddToClassList("menu_active");
        m_LeaderboardButton.AddToClassList("leaderboard-button_active");
    }

    // Event handler for the map button click event.
    private void OnMapButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        GameStateManager.LoadScene("Map");
    }

    // Event handler for the quiz button click event.
    private void OnQuizButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        GameStateManager.LoadScene("Quiz");
    }

    private IEnumerator QuizButtonRoutine()
    {
        while (true)
        {
            CheckQuizButton();
            yield return new WaitForSeconds(1);
        }
    }

    // Check if the quiz button should be displayed.
    private void CheckQuizButton()
    {
        long lastQuiz = GameStateManager.Instance.gameState.inventory.lastQuizTime;
        long simulationDayLength = ((long) GameStateManager.Instance.gameState.simulationDayLength) * 1000;
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (now > lastQuiz + simulationDayLength)
        {
            m_QuizButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            m_QuizButton.style.display = DisplayStyle.None;
        }
    }

    private void AnimateQuizQ()
    {
        StartCoroutine(AnimateQuizQLeft(3));
    }

    private IEnumerator AnimateQuizQLeft(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        m_QuizQ.AddToClassList("quizq-left");
        StartCoroutine(AnimateQuizQRightFull(0.5f));
    }

    private IEnumerator AnimateQuizQRightFull(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        m_QuizQ.AddToClassList("quizq-right-full");
        StartCoroutine(AnimateQuizQRight(0.8f));
    }

    private IEnumerator AnimateQuizQRight(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        m_QuizQ.AddToClassList("quizq-right");
        StartCoroutine(AnimateQuizQReset(0.5f));
    }

    private IEnumerator AnimateQuizQReset(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        m_QuizQ.RemoveFromClassList("quizq-left");
        m_QuizQ.RemoveFromClassList("quizq-right-full");
        m_QuizQ.RemoveFromClassList("quizq-right");
        StartCoroutine(AnimateQuizQLeft(3));
    }
}