using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quiz List Object", menuName = "Quiz/Quiz List")]
public class QuizListSO : ScriptableObject
{
    public List<QuestionAnswerSO> questions;

    public List<QuestionAnswerSO> GetRandomQuestions(int amount)
    {
        int actualAmount = amount;
        if (amount > questions.Count)
        {
            actualAmount = questions.Count;
        }
        if (amount < 0)
        {
            actualAmount = 0;
        }
        List<QuestionAnswerSO> shuffledQuestions = ShuffleQuestions();
        return shuffledQuestions.GetRange(0, actualAmount);
    }

    public List<QuestionAnswerSO> ShuffleQuestions()
    {
        System.Random rng = new System.Random();
        List<QuestionAnswerSO> shuffledList = new List<QuestionAnswerSO>(questions);
        int n = shuffledList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            QuestionAnswerSO value = shuffledList[k];
            shuffledList[k] = shuffledList[n];
            shuffledList[n] = value;
        }
        return shuffledList;
    }
}