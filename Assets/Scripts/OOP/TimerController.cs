using System;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    private float _timePerQuestion = 10f;
    private float _timeLeft;
    private bool _running;

    // Событие: время вышло
    public event Action TimeUp;

    // Событие: изменилась отображаемая секунда (10,9,8...)
    public event Action<int> SecondChanged;

    // Последняя секунда, которую мы уже показали/сообщили
    private int _lastShownSecond = -999;

    public void SetTimePerQuestion(float seconds)
    {
        _timePerQuestion = Mathf.Max(1f, seconds);
    }

    public void StartTimer()
    {
        _timeLeft = _timePerQuestion;
        _running = true;

        // Сбрасываем, чтобы событие точно сработало в начале
        _lastShownSecond = -999;

        // Обновим UI сразу
        UpdateTextAndSecondEvent();
    }

    public void StopTimer()
    {
        _running = false;
    }

    private void Update()
    {
        if (!_running)
            return;

        _timeLeft -= Time.deltaTime;

        // Обновляем UI и при необходимости кидаем SecondChanged
        UpdateTextAndSecondEvent();

        if (_timeLeft <= 0f)
        {
            _running = false;
            TimeUp?.Invoke();
        }
    }

    private void UpdateTextAndSecondEvent()
    {
        // Секунда для отображения: 10..9..8..1..0
        int shownSecond = Mathf.CeilToInt(_timeLeft);
        shownSecond = Mathf.Max(0, shownSecond);

        // Если секунда изменилась — кидаем событие ОДИН РАЗ
        if (shownSecond != _lastShownSecond)
        {
            _lastShownSecond = shownSecond;
            SecondChanged?.Invoke(shownSecond);
        }

        // Обновляем текст
        if (timerText != null)
            timerText.text = shownSecond.ToString();
    }
}
