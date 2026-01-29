using System;
using UnityEngine;


// "Этот класс можно показывать в Inspector"
[Serializable]
public class QuestionData
{
    // ------------------ ТЕКСТ ВОПРОСА ------------------

    // Сам вопрос (то, что видит игрок)
    public string questionText;


    // ------------------ ВАРИАНТЫ ОТВЕТОВ ------------------

    // Массив вариантов ответа
    // Например: ["3", "4", "5"]
    public string[] answers;


    // ------------------ ПРАВИЛЬНЫЙ ОТВЕТ ------------------

    // Индекс правильного ответа в массиве answers
    // 0 = первый, 1 = второй, 2 = третий
    public int correctAnswerIndex;
}
