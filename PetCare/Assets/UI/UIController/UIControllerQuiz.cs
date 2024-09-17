using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;
using System;

/// <summary>
/// Controls the quiz UI elements.
/// </summary>
public class UIControllerQuiz : MonoBehaviour
{
    private VisualElement m_OverlayScrim; // The scrim overlay for the quiz to prevent interaction.
    private Button m_BackButton; // The button to go back to the main menu.
    private Button m_SkipButton; // The button to skip the current question.
    private ProgressBar m_ProgressBar; // The progress bar for the quiz.
    private Label m_DoneLabel; // The label for the amount of done questions.
    private Label m_AllLabel; // The label for the total amount of questions.
    private Label m_QuestionLabel; // The label for the question text.
    private Button[] m_AnswerButtons = new Button[5]; // The buttons for the answers (1-4).
    public QuizListSO quizList; // The ScriptableObject containing the quiz questions.
    private List<QuestionAnswerSO> m_QuestionList = new List<QuestionAnswerSO>(); // The list of questions.
    private int m_CurrentQuestionIndex = 0; // The index of the current question.
    private float m_QuestionTextAnimationDuration = 2f; // The speed of the question text animation.
    private int m_CurrentCorrectAnswerIndex = 0; // The index of the current correct question.

    /// <summary>
    /// The amount of questions in the quiz.
    /// </summary>
    public int questionAmount = 5;

    /// <summary>
    /// The speed at which the text is typed out.
    /// Higher values mean faster typing.
    /// </summary>
    public float typingSpeed = 23f;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        m_OverlayScrim = root.Q<VisualElement>("OverlayScrim");
        m_BackButton = root.Q<Button>("QuizBackButton");
        m_SkipButton = root.Q<Button>("QuizSkipButton");
        m_ProgressBar = root.Q<ProgressBar>("QuizProgressBar");
        m_DoneLabel = root.Q<Label>("DoneQuizes");
        m_AllLabel = root.Q<Label>("AllQuizes");
        m_QuestionLabel = root.Q<Label>("QuizQuestion");

        for (int i = 1; i <= 4; i++)
        {
            int index = i;
            m_AnswerButtons[i] = root.Q<Button>("Answer" + i);
            m_AnswerButtons[i].RegisterCallback<ClickEvent>(evt => OnAnswerButtonClicked(evt, index));
        }

        m_BackButton.RegisterCallback<ClickEvent>(OnQuizBackButtonClicked);
        m_SkipButton.RegisterCallback<ClickEvent>(OnQuizSkipButtonClicked);

        m_QuestionList = quizList.GetRandomQuestions(questionAmount);
        questionAmount = m_QuestionList.Count;
        PopulateQuiz();
    }

    // Event Handler for the back button.
    private void OnQuizBackButtonClicked(ClickEvent evt)
    {
        // Go back to the main menu.
        StartCoroutine(QuizDone());
    }

    // Event Handler for the skip button.
    private void OnQuizSkipButtonClicked(ClickEvent evt)
    {
        // Skip the current question.
        m_CurrentQuestionIndex++;
        if (m_CurrentQuestionIndex < questionAmount)
        {
            StartCoroutine(PopulateQuizCoroutine());
        }
        else
        {
            // Quiz is done.
            StartCoroutine(QuizDone());
        }
    }

    // Event Handler for the answer buttons.
    private void OnAnswerButtonClicked(ClickEvent evt, int answerIndex)
    {
        // Check if the answer is correct.
        if (m_CurrentCorrectAnswerIndex == answerIndex)
        {
            // Correct answer.
            MoneySystem.instance.AddCoinsPerQuiz();
            m_AnswerButtons[answerIndex].AddToClassList("quiz-answer-button-correct");
            m_AnswerButtons[answerIndex].Q<VisualElement>("AnswerIcon").AddToClassList("answer-icon-correct");
        }
        else
        {
            // Wrong answer.
            m_AnswerButtons[answerIndex].AddToClassList("quiz-answer-button-wrong");
            m_AnswerButtons[answerIndex].Q<VisualElement>("AnswerIcon").AddToClassList("answer-icon-wrong");
            m_AnswerButtons[m_CurrentCorrectAnswerIndex].AddToClassList("quiz-answer-button-correct");
            m_AnswerButtons[m_CurrentCorrectAnswerIndex].Q<VisualElement>("AnswerIcon").AddToClassList("answer-icon-correct");
        }
        m_OverlayScrim.style.display = DisplayStyle.Flex;

        m_CurrentQuestionIndex++;
        if (m_CurrentQuestionIndex < questionAmount)
        {
            StartCoroutine(PopulateQuizCoroutine(1.5f));
        }
        else
        {
            // Quiz is done.
            StartCoroutine(QuizDone(1.5f));
        }
    }

    // Resets the answer buttons.
    private void ResetAnswerButtons()
    {
        for (int i = 1; i <= 4; i++)
        {
            m_AnswerButtons[i].RemoveFromClassList("quiz-answer-button-correct");
            m_AnswerButtons[i].RemoveFromClassList("quiz-answer-button-wrong");
            m_AnswerButtons[i].Q<VisualElement>("AnswerIcon").RemoveFromClassList("answer-icon-correct");
            m_AnswerButtons[i].Q<VisualElement>("AnswerIcon").RemoveFromClassList("answer-icon-wrong");
            m_AnswerButtons[i].AddToClassList("quiz-answer-button-inactive");
        }
    }

    // Coroutine to populate the quiz after a delay.
    private IEnumerator PopulateQuizCoroutine(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        DOTween.KillAll();
        PopulateQuiz();
    }

    // Populates the quiz with the current question.
    private void PopulateQuiz()
    {
        m_DoneLabel.text = m_CurrentQuestionIndex.ToString();
        m_AllLabel.text = "/" + questionAmount.ToString();
        m_ProgressBar.value = (float)m_CurrentQuestionIndex / questionAmount;

        UpdateTypingSpeed(m_QuestionList[m_CurrentQuestionIndex].question.Length);
        m_QuestionLabel.text = "";
        DOTween.To(() => m_QuestionLabel.text, x => m_QuestionLabel.text = x,
            m_QuestionList[m_CurrentQuestionIndex].question, m_QuestionTextAnimationDuration).SetEase(Ease.Linear);

        ResetAnswerButtons();
        m_OverlayScrim.style.display = DisplayStyle.None;
        // m_AnswerButtons[1].Q<Label>("Answer1Text").text = m_QuestionList[m_CurrentQuestionIndex].answer1;
        // m_AnswerButtons[2].Q<Label>("Answer1Text").text = m_QuestionList[m_CurrentQuestionIndex].answer2;
        // m_AnswerButtons[3].Q<Label>("Answer1Text").text = m_QuestionList[m_CurrentQuestionIndex].answer3;
        // m_AnswerButtons[4].Q<Label>("Answer1Text").text = m_QuestionList[m_CurrentQuestionIndex].answer4;

        int correctAnswer = m_QuestionList[m_CurrentQuestionIndex].correctAnswer;

        string[] answers = new string[5];
        answers[1] = m_QuestionList[m_CurrentQuestionIndex].answer1;
        answers[2] = m_QuestionList[m_CurrentQuestionIndex].answer2;
        answers[3] = m_QuestionList[m_CurrentQuestionIndex].answer3;
        answers[4] = m_QuestionList[m_CurrentQuestionIndex].answer4;

        for (int i = 4; i > 1; i--)
        {
            int j = UnityEngine.Random.Range(1, i + 1);
            string temp = answers[i];
            answers[i] = answers[j];
            answers[j] = temp;
            if (j == correctAnswer)
            {
                correctAnswer = i;
            }
            else if (i == correctAnswer)
            {
                correctAnswer = j;
            }
        }

        for (int i = 1; i <= 4; i++)
        {
            m_AnswerButtons[i].Q<Label>("Answer1Text").text = answers[i];
        }

        m_CurrentCorrectAnswerIndex = correctAnswer;

        for (int i = 1; i <= 4; i++)
        {
            StartCoroutine(ShowAnswerButtonDelayed(i, i * 0.11f));
        }
    }

    // Coroutine to show the answer buttons after a delay.
    private IEnumerator ShowAnswerButtonDelayed(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        m_AnswerButtons[index].RemoveFromClassList("quiz-answer-button-inactive");
    }

    // Coroutine to finish the quiz after a delay.
    private IEnumerator QuizDone(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        GameStateManager.Instance.gameState.inventory.lastQuizTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        GameStateManager.LoadScene("Home");
    }

    // Updates the typing speed based on the length of the question.
    private void UpdateTypingSpeed(int length)
    {
        m_QuestionTextAnimationDuration = length / typingSpeed;
    }
}
