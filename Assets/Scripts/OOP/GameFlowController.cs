using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


// Это главный "оркестратор".
// Он НЕ хранит вопросы в себе, НЕ рисует UI, НЕ включает объекты напрямую "как попало".
// Он просто управляет процессом игры и вызывает методы других контроллеров.
public class GameFlowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuizController quiz;
    [SerializeField] private TimerController timer;
    [SerializeField] private TeacherStateController teacher;
    [SerializeField] private ScreamerController screamer;
    [SerializeField] private AudioController audioController;
    [SerializeField] private UIQuizView ui;

    [Header("Rules")]
    // Сколько ошибок до конца игры (в твоей игре = 3)
    [SerializeField] private int maxMistakes = 3;

    // Сколько ошибок сейчас
    private int _mistakes;

    // Игра закончилась или нет
    private bool _gameOver;

    // Сейчас ждём ответ игрока на текущий вопрос
    private bool _awaitingAnswer;

    [SerializeField] private ResultView resultView;


    private void Awake()
    {
        // Подписываемся на изменение секунд таймера (для тика на последних секундах)
        if (timer != null)
            timer.SecondChanged += OnTimerSecondChanged;

        // Подписываемся на события UI (кнопки)
        if (ui != null)
            ui.AnswerClicked += OnAnswerClicked;

        // Подписываемся на событие таймера (время вышло)
        if (timer != null)
            timer.TimeUp += OnTimeUp;
    }

    private void Start()
    {
        // Запускаем игру
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

        // Запускаем музыку уровня
        if (audioController != null)
            audioController.PlayMusic();

        // Показываем первый вопрос
        ShowCurrentQuestionAndStartTimer();
    }

    // Показать текущий вопрос и запустить таймер
    private void ShowCurrentQuestionAndStartTimer()
    {
        if (_gameOver)
            return;

        // Получаем текущий вопрос
        var q = quiz != null ? quiz.GetCurrentQuestion() : null;

        // Если вопросов нет — считаем что игрок выиграл (уровень пройден)
        if (q == null)
        {
            Win();
            return;
        }

        // Показываем вопрос в UI
        if (ui != null)
            ui.ShowQuestion(q);

        _awaitingAnswer = true;


        // Запускаем таймер
        if (timer != null)
            timer.StartTimer();
    }

    // Когда игрок нажал на кнопку ответа
    /*private void OnAnswerClicked(int answerIndex)
    {
        _awaitingAnswer = false;

        if (_gameOver)
            return;

        // Блокируем кнопки, чтобы не спамили кликами
        if (ui != null)
            ui.SetButtonsInteractable(false);

        // Останавливаем таймер, потому что ответ уже дан
        if (timer != null)
            timer.StopTimer();

        if (audioController != null)
            audioController.SetTicking(false);

        // Проверяем правильность ответа
        bool correct = quiz != null && quiz.IsAnswerCorrect(answerIndex);

        if (correct)
        {
            // Правильно: идём дальше
            OnCorrectAnswer();
        }
        else
        {
            // Неправильно: ошибка
            AddMistake();
        }
    }*/
    private void OnAnswerClicked(int answerIndex)
    {
        if (_gameOver)
            return;

        _awaitingAnswer = false;

        if (ui != null)
            ui.SetButtonsInteractable(false);

        if (timer != null)
            timer.StopTimer();

        bool correct = quiz != null && quiz.IsAnswerCorrect(answerIndex);

        // Звук правильного / неправильного ответа
        if (audioController != null)
        {
            if (correct)
                audioController.PlayCorrect();
            else
                audioController.PlayWrong();
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

        // Таймер закончился = это ошибка
        AddMistake();
    }

    private void OnCorrectAnswer()
    {
        if (_gameOver)
            return;

        // Переходим к следующему вопросу
        bool hasNext = quiz != null && quiz.MoveNext();

        // Если больше вопросов нет — победа
        if (!hasNext)
        {
            Win();
            return;
        }

        // Показываем следующий вопрос
        ShowCurrentQuestionAndStartTimer();
    }

    // Добавить ошибку и обновить состояние училки/игры
    private void AddMistake()
    {
        if (_gameOver)
            return;

        _mistakes++;

        // Обновляем состояние училки по количеству ошибок:
        // 0 -> спокойная, 1 -> состояние 1, 2 -> состояние 2
        // А на 3 -> проигрыш (скример)
        if (_mistakes == 1)
        {
            if (teacher != null) teacher.SetState(1);
            if (audioController != null) audioController.PlayBreathing();
        }
        else if (_mistakes == 2)
        {
            if (teacher != null) teacher.SetState(2);
            if (audioController != null) audioController.PlayWarning();
        }
        else if (_mistakes >= maxMistakes)
        {
            Lose();
            return;
        }

        // После ошибки переходим к следующему вопросу (как у тебя сейчас)
        bool hasNext = quiz != null && quiz.MoveNext();
        if (!hasNext)
        {
            // Если вопросов больше нет — можно считать победой или концом.
            // Я делаю победу (логично: выжил и дошёл до конца вопросов).
            Win();
            return;
        }

        ShowCurrentQuestionAndStartTimer();
    }

    // Победа
    private void Win()
    {
        // Защита от повторного вызова
        if (_gameOver)
            return;

        _gameOver = true;

        // Отмечаем уровень как пройденный (ОДИН РАЗ)
        if (LevelManager.Instance != null)
            LevelManager.Instance.MarkLevelCompleted();

        // Останавливаем таймер
        if (timer != null)
            timer.StopTimer();

        // Скрываем UI вопросов
        if (ui != null)
            ui.SetPanelVisible(false);

        // Отключаем тик
        if (audioController != null)
            audioController.SetTicking(false);

        // Показываем результат (ОДИН РАЗ)
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

        // Спрятать училку
        if (teacher != null)
            teacher.HideAll();

        // Остановить все эффекты (крик, дыхание, тик)
        if (audioController != null)
        {
            audioController.SetTicking(false);
            audioController.StopSfx();
        }

        // Запускаем скример сразу
        if (screamer != null)
            screamer.Play();

        // Звук скримера
        if (audioController != null)
            audioController.PlayScreamer();

        Debug.Log("LOSE: started, will show panel in 3 seconds...");
       
        // ⏳ Показать результат ЧЕРЕЗ 3 секунды
        StartCoroutine(ShowLoseResultDelayed());
    }



    // Этот метод вызывается каждый раз, когда меняется отображаемая секунда таймера
    private void OnTimerSecondChanged(int secondsLeft)
    {
        if (_gameOver)
            return;

        // Включаем тиканье, когда осталось 5..1
        bool shouldTick = secondsLeft <= 5 && secondsLeft >= 1;

        if (audioController != null)
            audioController.SetTicking(shouldTick);
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
        // (ожидаем ответ -> можно нажимать; иначе кнопки остаются заблокированы)
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
        {
            OnCorrectAnswer();
        }
        else
        {
            AddMistake();
        }
    }
    /*private IEnumerator ShowLoseResultDelayed()
    {
        // Ждём 3 секунды (скример играет)
        yield return new WaitForSeconds(3f);

        // После задержки показываем результат
        if (resultView != null)
            resultView.ShowLose();
    }*/
    private IEnumerator ShowLoseResultDelayed()
    {
        yield return new WaitForSecondsRealtime(3f);

        if (resultView != null)
            resultView.ShowLose();
        else
            Debug.LogError("ResultView is NULL in GameFlowController!");
    }




}
