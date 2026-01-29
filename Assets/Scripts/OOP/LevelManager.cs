using UnityEngine;
using UnityEngine.SceneManagement;

// LevelManager отвечает ТОЛЬКО за уровни и перезагрузку сцены.
// - Хранит список LevelConfig по порядку
// - Помнит текущий уровень через PlayerPrefs
// - Умеет: получить текущий LevelConfig, отметить победу, перезагрузить сцену, рестарт, сброс прогресса
public class LevelManager : MonoBehaviour
{
    [Header("Levels (in order)")]
    [SerializeField] private LevelConfig[] levels;

    // Singleton
    public static LevelManager Instance { get; private set; }

    // Текущий индекс уровня (0 = первый уровень)
    private int _currentLevelIndex = 0;

    // Защита от двойной загрузки сцены (двойной клик по кнопке)
    private bool _isReloadingScene = false;

    // Ключ сохранения прогресса
    private const string KEY_CURRENT_LEVEL = "CurrentLevel";

    private void Awake()
    {// это обновление памяти 
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.Save();
        
        // Singleton защита: если такой объект уже есть — уничтожаем дубликат
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // ВАЖНО: LevelManager должен быть ROOT объектом в Hierarchy (не дочерним),
        // иначе DontDestroyOnLoad выдаст ошибку.
        DontDestroyOnLoad(gameObject);

        // Загружаем сохранённый уровень (по умолчанию 0)
        _currentLevelIndex = PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, 0);

        // Если уровни не назначены — сообщаем ошибку
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("LevelManager: levels array is empty! Assign LevelConfig assets in Inspector.");
            _currentLevelIndex = 0;
            return;
        }

        // На всякий случай зажимаем индекс в границы массива
        _currentLevelIndex = Mathf.Clamp(_currentLevelIndex, 0, levels.Length - 1);
    }

    // Получить LevelConfig текущего уровня (для LevelLoader)
    public LevelConfig GetCurrentLevel()
    {
        if (levels == null || levels.Length == 0)
            return null;

        _currentLevelIndex = Mathf.Clamp(_currentLevelIndex, 0, levels.Length - 1);
        return levels[_currentLevelIndex];
    }

    // Номер текущего уровня для UI (1..N)
    public int GetCurrentLevelNumber()
    {
        if (levels == null || levels.Length == 0)
            return 1;

        return Mathf.Clamp(_currentLevelIndex + 1, 1, levels.Length);
    }

    // Всего уровней
    public int GetTotalLevels()
    {
        return levels == null ? 0 : levels.Length;
    }

    // Отметить уровень как пройденный (вызывай ТОЛЬКО в Win())
    // Увеличивает индекс максимум до последнего уровня и сохраняет.
    public void MarkLevelCompleted()
    {
        if (levels == null || levels.Length == 0)
            return;

        if (_currentLevelIndex < levels.Length - 1)
            _currentLevelIndex++;

        SaveProgress();
    }

    // Перезагрузить сцену (для кнопки "Следующий уровень")
    // ВАЖНО: этот метод НЕ увеличивает уровень, он только перезагружает сцену.
    public void ReloadGameScene()
    {
        ReloadCurrentSceneInternal();
    }

    // Перезапустить текущий уровень (для кнопки "Играть заново")
    public void RestartCurrentLevel()
    {
        ReloadCurrentSceneInternal();
    }

    // Сбросить прогресс до 1 уровня и перезапустить (можно для меню/отладки)
    public void ResetProgressAndRestart()
    {
        _currentLevelIndex = 0;
        SaveProgress();
        ReloadCurrentSceneInternal();
    }

    // Установить уровень вручную (если сделаешь меню выбора уровней)
    public void SetLevelIndex(int index)
    {
        if (levels == null || levels.Length == 0)
            return;

        _currentLevelIndex = Mathf.Clamp(index, 0, levels.Length - 1);
        SaveProgress();
    }

    // Внутренний метод перезагрузки текущей сцены с защитой от двойного клика
    private void ReloadCurrentSceneInternal()
    {
        Debug.Log("LevelManager: ReloadCurrentSceneInternal called");

        if (_isReloadingScene)
        {
            Debug.Log("LevelManager: blocked by _isReloadingScene = true");
            return;
        }

        _isReloadingScene = true;

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("LevelManager: loading scene buildIndex = " + buildIndex);

        var op = SceneManager.LoadSceneAsync(buildIndex);
        op.completed += _ =>
        {
            _isReloadingScene = false;
            Debug.Log("LevelManager: scene reload completed, _isReloadingScene reset");
        };
    }


    // Сохранить прогресс
    private void SaveProgress()
    {
        PlayerPrefs.SetInt(KEY_CURRENT_LEVEL, _currentLevelIndex);
        PlayerPrefs.Save();
    }
}
