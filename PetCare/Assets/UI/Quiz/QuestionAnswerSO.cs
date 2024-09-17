using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Question Answer Object", menuName = "Quiz/Question Answer")]
public class QuestionAnswerSO : ScriptableObject
{
    [TextArea(2, 10)]
    public string question;
    [TextArea(1, 10)]
    public string answer1;
    [TextArea(1, 10)]
    public string answer2;
    [TextArea(1, 10)]
    public string answer3;
    [TextArea(1, 10)]
    public string answer4;
    public int correctAnswer;
    [TextArea(2, 10)]
    public string explanation;
}