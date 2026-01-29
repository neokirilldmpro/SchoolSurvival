using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Этот класс отвечает ТОЛЬКО за UI вопросов.
// Он показывает вопрос, варианты и сообщает наружу, какую кнопку нажали.
public class UIQuizView : MonoBehaviour
{
    [Header("Answer Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;

    private Image[] _buttonImages;


    [Header("UI References")]
    // Текст вопроса (TMP)
    [SerializeField] private TMP_Text questionText;

    // Кнопки ответов (у тебя их 3)
    [SerializeField] private Button[] answerButtons;

    // Тексты на кнопках (TMP, должны совпадать по количеству с answerButtons)
    [SerializeField] private TMP_Text[] answerButtonTexts;

    [Header("Optional")]
    // Можно выключать панель целиком (например, при скримере)
    [SerializeField] private GameObject panelRoot;

    // Событие: пользователь нажал ответ (передаем индекс 0/1/2)
    public event Action<int> AnswerClicked;

    // Здесь мы подпишемся на кнопки
    private void Awake()
    {
        // Защита от ошибок настройки
        if (answerButtons == null || answerButtons.Length == 0)
        {
            Debug.LogError("UIQuizView: answerButtons is empty!");
            return;
        }

        // Для каждой кнопки назначаем обработчик клика
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int capturedIndex = i; // важно: запоминаем i в отдельной переменной
            answerButtons[i].onClick.AddListener(() =>
            {
                // Когда кликнули — вызываем событие и передаем индекс
                AnswerClicked?.Invoke(capturedIndex);
            });
        }

        // Кешируем Image компонентов кнопок (для покраски)
        _buttonImages = new Image[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            _buttonImages[i] = answerButtons[i].GetComponent<Image>();
        }

    }

    // Показать вопрос и варианты
    public void ShowQuestion(QuestionData q)
    {
        if (q == null)
        {
            Debug.LogError("UIQuizView: question is null!");
            return;
        }

        // Пишем текст вопроса
        if (questionText != null)
            questionText.text = q.questionText;

        // Заполняем кнопки текстами ответов
        // Если ответов меньше, чем кнопок — лишние кнопки выключим
        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool hasAnswer = q.answers != null && i < q.answers.Length;

            if (answerButtons[i] != null)
                answerButtons[i].gameObject.SetActive(hasAnswer);

            if (hasAnswer && answerButtonTexts != null && i < answerButtonTexts.Length && answerButtonTexts[i] != null)
                answerButtonTexts[i].text = q.answers[i];
        }

        // Включаем панель (если она задана)
        if (panelRoot != null)
            panelRoot.SetActive(true);

        // Разблокируем кнопки
        SetButtonsInteractable(true);
        ResetButtonColors();

    }

    // Заблокировать/разблокировать кнопки
    public void SetButtonsInteractable(bool interactable)
    {
        if (answerButtons == null)
            return;

        foreach (var b in answerButtons)
        {
            if (b != null)
                b.interactable = interactable;
        }
    }
    // Видна ли панель сейчас
    public bool IsVisible => panelRoot != null && panelRoot.activeSelf;

    // Скрыть всю панель
    public void SetPanelVisible(bool visible)
    {
        if (panelRoot != null)
            panelRoot.SetActive(visible);
    }

    // Сбросить цвета кнопок к обычному состоянию
    public void ResetButtonColors()
    {
        if (_buttonImages == null)
            return;

        foreach (var img in _buttonImages)
        {
            if (img != null)
                img.color = normalColor;
        }
    }

    // Подсветить выбранный ответ
    public void HighlightAnswer(int index, bool correct)
    {
        if (_buttonImages == null)
            return;

        if (index < 0 || index >= _buttonImages.Length)
            return;

        if (_buttonImages[index] != null)
            _buttonImages[index].color = correct ? correctColor : wrongColor;
    }


}
