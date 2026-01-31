using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Главный оркестратор: управляет логикой игры.
// ВАЖНО ДЛЯ "единый AudioController (DontDestroyOnLoad)":
// - НИЧЕГО не ищем в сцене через инспектор для AudioController
// - ВСЕГДА берём звук через AudioController.Instance
// - Музыку уровня запускает LevelLoader (после SetAudioClips), поэтому здесь PlayMusic НЕ вызываем
public class GameFlowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuizController quiz;
    [SerializeField] private TimerController timer;
    [SerializeField] private TeacherStateController teacher;
    [SerializeField] private ScreamerController screamer;
    [SerializeField] private UIQuizView ui;
    [SerializeField] private ResultView resultView;

    [Header("Rules")]
    [SerializeField] private int maxMistakes = 3;

    private int _mistakes;
    private bool _gameOver;
    private bool _awaitingAnswer;

    private void Awake()
    {
        // Подписки на события таймера / UI
        if (timer != null)
            timer.SecondChanged += OnTimerSecondChanged;

        if (ui != null)
            ui.AnswerClicked += OnAnswerClicked;

        if (timer != null)
            timer.TimeUp += OnTimeUp;
    }

    // ВАЖНО: StartGame нужно запускать НЕ сразу в Start(),
    // потому что LevelLoader тоже стартует в этом же кадре и именно он задаёт клипы AudioController через SetAudioClips.
    // Если мы дернем звук раньше — клипы ещё null.
    private IEnumerator Start()
    {
        // Подождать 1 кадр, чтобы LevelLoader успел ApplyLevel() -> SetAudioClips(...)
        yield return null;

        StartGame();
    }

    // Старт игры / уровня
    private void StartGame()
    {
        _gameOver = false;
        _mistakes = 0;

        // Учитель в спокойное состояние
        if (teacher != null)
            teacher.SetState(0);

        // ВАЖНО: PlayMusic тут НЕ вызываем.
        // Музыку уровня запускает LevelLoader после того, как задаст клипы:
        // AudioController.Instance.SetAudioClips(...) + PlayMusic()
        // Иначе получишь ошибку "_music is NULL".

        // Показываем первый вопрос и запускаем таймер
        ShowCurrentQuestionAndStartTimer();
    }

    // Показать текущий вопрос и запустить таймер
    private void ShowCurrentQuestionAndStartTimer()
    {
        if (_gameOver)
            return;

        var q = quiz != null ? quiz.GetCurrentQuestion() : null;

        // Если вопросов нет — победа
        if (q == null)
        {
            Win();
            return;
        }

        // Показать вопрос
        if (ui != null)
            ui.ShowQuestion(q);

        _awaitingAnswer = true;

        // Запустить таймер
        if (timer != null)
            timer.StartTimer();
    }

    // Когда игрок нажал на кнопку ответа
    private void OnAnswerClicked(int answerIndex)
    {
        if (_gameOver)
            return;

        _awaitingAnswer = false;

        // Блокируем кнопки
        if (ui != null)
            ui.SetButtonsInteractable(false);

        // Останавливаем таймер
        if (timer != null)
            timer.StopTimer();

        bool correct = quiz != null && quiz.IsAnswerCorrect(answerIndex);

        // Звук правильного / неправильного ответа
        // ВАЖНО: берём глобальный AudioController
        var audio = AudioController.Instance;
        if (audio != null)
        {
            // Выключим тик, чтобы не "пиликал" во время выбора
            audio.SetTicking(false);

            if (correct)
                audio.PlayCorrect();
            else
                audio.PlayWrong();
        }

        // Подсветка кнопки
        if (ui != null)
            ui.HighlightAnswer(answerIndex, correct);

        // Ждём 1 секунду, затем продолжаем логику
        StartCoroutine(AnswerDelayRoutine(correct));
    }

    // Когда таймер закончился
    private void OnTimeUp()
    {
        _awaitingAnswer = false;

        if (_gameOver)
            return;

        // Таймер закончился = ошибка
        AddMistake();
    }

    private void OnCorrectAnswer()
    {
        if (_gameOver)
            return;

        // Следующий вопрос
        bool hasNext = quiz != null && quiz.MoveNext();

        if (!hasNext)
        {
            Win();
            return;
        }

        ShowCurrentQuestionAndStartTimer();
    }

    // Добавить ошибку и обновить состояние училки/игры
    private void AddMistake()
    {
        if (_gameOver)
            return;

        _mistakes++;

        var audio = AudioController.Instance;

        // 1 ошибка
        if (_mistakes == 1)
        {
            if (teacher != null) teacher.SetState(1);
            if (audio != null) audio.PlayBreathing();
        }
        // 2 ошибка
        else if (_mistakes == 2)
        {
            if (teacher != null) teacher.SetState(2);
            if (audio != null) audio.PlayWarning();
        }
        // 3 ошибка = проигрыш
        else if (_mistakes >= maxMistakes)
        {
            Lose();
            return;
        }

        // Переходим к следующему вопросу
        bool hasNext = quiz != null && quiz.MoveNext();
        if (!hasNext)
        {
            Win();
            return;
        }

        ShowCurrentQuestionAndStartTimer();
    }

    // Победа
    private void Win()
    {
        if (_gameOver)
            return;

        _gameOver = true;

        // Отмечаем уровень как пройденный
        if (LevelManager.Instance != null)
            LevelManager.Instance.MarkLevelCompleted();

        // Останавливаем таймер
        if (timer != null)
            timer.StopTimer();

        // Скрываем UI вопросов
        if (ui != null)
            ui.SetPanelVisible(false);

        // Отключаем тик
        var audio = AudioController.Instance;
        if (audio != null)
            audio.SetTicking(false);

        // Показываем результат
        if (resultView != null)
            resultView.ShowWin();

        Debug.Log("WIN");
    }

    // Поражение (скример)
    private void Lose()
    {
        _gameOver = true;

        // Останавливаем таймер
        if (timer != null)
            timer.StopTimer();

        // Скрываем UI вопросов
        if (ui != null)
            ui.SetPanelVisible(false);

        // Прячем училку
        if (teacher != null)
            teacher.HideAll();

        var audio = AudioController.Instance;

        // Остановить эффекты и тик
        if (audio != null)
        {
            audio.SetTicking(false);
            audio.StopSfx();
        }

        // Визуальный скример
        if (screamer != null)
            screamer.Play();

        // Звук скримера
        if (audio != null)
            audio.PlayScreamer();

        Debug.Log("LOSE: started, will show panel in 3 seconds...");

        // Показать результат через 3 секунды (Realtime, чтобы не зависеть от Time.timeScale)
        StartCoroutine(ShowLoseResultDelayed());
    }

    // Вызывается каждый раз, когда меняется отображаемая секунда таймера
    private void OnTimerSecondChanged(int secondsLeft)
    {
        if (_gameOver)
            return;

        // Включаем тиканье, когда осталось 5..1
        bool shouldTick = secondsLeft <= 5 && secondsLeft >= 1;

        var audio = AudioController.Instance;
        if (audio != null)
            audio.SetTicking(shouldTick);
    }

    private void OnDestroy()
    {
        if (ui != null)
            ui.AnswerClicked -= OnAnswerClicked;

        if (timer != null)
        {
            timer.TimeUp -= OnTimeUp;
            timer.SecondChanged -= OnTimerSecondChanged;
        }
    }

    private void Update()
    {
        if (_gameOver)
            return;

        // Проверяем клавишу Q через новый Input System
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleQuizPanel();
        }
    }

    private void ToggleQuizPanel()
    {
        if (ui == null)
            return;

        bool show = !ui.IsVisible;

        // Просто показываем/скрываем панель
        ui.SetPanelVisible(show);

        // Если показываем — восстанавливаем правильное состояние кнопок
        if (show)
            ui.SetButtonsInteractable(_awaitingAnswer);
    }

    private IEnumerator AnswerDelayRoutine(bool correct)
    {
        // Задержка 1 секунда
        yield return new WaitForSeconds(1f);

        if (_gameOver)
            yield break;

        if (correct)
            OnCorrectAnswer();
        else
            AddMistake();
    }

    private IEnumerator ShowLoseResultDelayed()
    {
        yield return new WaitForSecondsRealtime(3f);

        if (resultView != null)
            resultView.ShowLose();
        else
            Debug.LogError("ResultView is NULL in GameFlowController!");
    }
}
