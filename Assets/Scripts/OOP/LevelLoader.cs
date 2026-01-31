using UnityEngine;

// Этот скрипт висит на объекте сцены (например, Systems/LevelLoader)
// и "применяет" данные из LevelConfig к твоим контроллерам.
public class LevelLoader : MonoBehaviour
{
    // --------- ДАННЫЕ УРОВНЯ ---------

    [Header("Level Data (ScriptableObject)")]

    // Ссылка на ассет LevelConfig (твой LevelConfig.asset)
    // Ты вручную перетащишь ассет сюда в Inspector
    //[SerializeField] private LevelConfig levelConfig;


    // --------- ССЫЛКИ НА КОНТРОЛЛЕРЫ ---------

    [Header("Controllers in Scene")]

    // Контроллер логики вопросов (будем создавать дальше)
    [SerializeField] private QuizController quizController;

    // Контроллер таймера (будем создавать дальше)
    [SerializeField] private TimerController timerController;

    // Контроллер состояний училки (будем создавать дальше)
    [SerializeField] private TeacherStateController teacherStateController;

    // Контроллер скримера (будем создавать дальше)
    [SerializeField] private ScreamerController screamerController;

    // Контроллер звуков (будем создавать дальше)
    [SerializeField] private AudioController audioController;
    //[SerializeField] private Transform teacherRoot;

    [Header("Background")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [Header("ОБЕКТЫ КОТОРЫЕ НЕ НУЖНЫ ВДРУГОЙ СЦЕНЕ")]
    [SerializeField] private GameObject[] levelDecorRoots;




    private void Start()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelLoader: LevelManager instance not found!");
            return;
        }

        LevelConfig config = LevelManager.Instance.GetCurrentLevel();

        if (config == null)
        {
            Debug.LogError("LevelLoader: No LevelConfig found!");
            return;
        }

        ApplyLevel(config);
    }

    // Этот метод "раскидывает" данные уровня по контроллерам
    private void ApplyLevel(LevelConfig config)
    {

        // 1) Настраиваем вопросы
        // Передаём список вопросов в QuizController
        if (quizController != null)
        {
            quizController.SetQuestions(config.questions);
        }

        // 2) Настраиваем таймер
        // Передаём сколько секунд на один вопрос
        if (timerController != null)
        {
            timerController.SetTimePerQuestion(config.timePerQuestion);
        }

        
        // 3) Настраиваем училку (картинки состояний)
        if (teacherStateController != null)
        {
            teacherStateController.SetStateSprites(config.teacherStateSprites);
        }

        // Позиции по состояниям училки (меняем только те, где overridePosition=true)
        
        if (teacherStateController != null)
            teacherStateController.ApplyStateOverrides(config.teacherStateSetups);




        // 4) Настраиваем скример (2 картинки + цвет фона)
        if (screamerController != null)
        {
            screamerController.SetScreamerSprites(config.screamerSpriteA, config.screamerSpriteB);
            screamerController.SetBackgroundColor(config.screamerBackgroundColor);
        }
        // НОВОЕ
        if (screamerController != null)
            screamerController.ApplyOverrides(config.screamerSetup);

        // 6) Настраиваем фон уровня
        if (backgroundRenderer != null && config.backgroundSprite != null)
        {
            backgroundRenderer.sprite = config.backgroundSprite;
        }


        // 5) Настраиваем звуки
        if (audioController != null)
        {
            audioController.SetAudioClips(
            config.backgroundMusic,
            config.tickSound,
            config.breathingSound,
            config.warningShoutSound,
            config.screamerSound,
            config.correctAnswerSound,
            config.wrongAnswerSound

            );
            audioController.ApplyVolumes(AudioSettingsModel.GetMusic(), AudioSettingsModel.GetSfx());
            //2) запускаем музыку
            audioController.PlayMusic();
        }
        int levelIndex = 0;
        if (LevelManager.Instance != null)
            levelIndex = LevelManager.Instance.GetCurrentLevelNumber() - 1;

        if (levelDecorRoots != null && levelDecorRoots.Length > 0)
        {
            for (int i = 0; i < levelDecorRoots.Length; i++)
                if (levelDecorRoots[i] != null)
                    levelDecorRoots[i].SetActive(i == levelIndex);
        }

    }
}
