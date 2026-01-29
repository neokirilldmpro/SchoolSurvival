using System.Collections.Generic;
using UnityEngine;

// Этот класс отвечает ТОЛЬКО за вопросы:
// хранить список, отдавать текущий, проверять ответ.
public class QuizController : MonoBehaviour
{
    // Список вопросов текущего уровня
    private List<QuestionData> _questions = new List<QuestionData>();

    // Индекс текущего вопроса
    private int _currentIndex = 0;

    // Установить вопросы (вызывается LevelLoader-ом)
    public void SetQuestions(List<QuestionData> questions)
    {
        // Если пришёл null — значит данные не настроены
        if (questions == null)
        {
            Debug.LogError("QuizController: questions list is null!");
            _questions = new List<QuestionData>();
            _currentIndex = 0;
            return;
        }

        // Сохраняем ссылку на список вопросов
        _questions = questions;

        // Начинаем с первого вопроса
        _currentIndex = 0;
    }

    // Получить текущий вопрос (может вернуть null, если вопросов нет)
    public QuestionData GetCurrentQuestion()
    {
        // Если список пуст — возвращаем null
        if (_questions == null || _questions.Count == 0)
            return null;

        // Защита от выхода за границы
        if (_currentIndex < 0 || _currentIndex >= _questions.Count)
            return null;

        // Возвращаем текущий вопрос
        return _questions[_currentIndex];
    }

    // Проверить ответ по индексу (0/1/2)
    public bool IsAnswerCorrect(int answerIndex)
    {
        // Берём текущий вопрос
        var q = GetCurrentQuestion();

        // Если вопроса нет — считаем что неверно
        if (q == null)
            return false;

        // Сравниваем индекс ответа с правильным индексом
        return answerIndex == q.correctAnswerIndex;
    }

    // Перейти к следующему вопросу
    // Возвращает false, если вопросов больше нет (значит победа)
    public bool MoveNext()
    {
        // Увеличиваем индекс
        _currentIndex++;

        // Если вышли за последний вопрос — значит конец вопросов
        if (_questions == null || _currentIndex >= _questions.Count)
            return false;

        // Иначе вопросы ещё есть
        return true;
    }

    // Узнать сколько всего вопросов (иногда полезно для UI)
    public int GetTotalQuestions()
    {
        return _questions == null ? 0 : _questions.Count;
    }
}
