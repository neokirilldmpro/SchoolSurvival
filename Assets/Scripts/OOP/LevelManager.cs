using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Levels (in order)")]
    [SerializeField] private LevelConfig[] levels;

    [Header("Scene Names")]
    [SerializeField] private string menuSceneName = "MenuScene";
    [SerializeField] private string gameSceneName = "GameScene";

    public static LevelManager Instance { get; private set; }

    private int _currentLevelIndex;
    private const string KEY_CURRENT_LEVEL = "CurrentLevel";

    


    private void Awake()
    {// это обновление памяти 
        /*PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.Save();*/
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _currentLevelIndex = PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, 0);
        _currentLevelIndex = Mathf.Clamp(_currentLevelIndex, 0, Mathf.Max(0, levels.Length - 1));
    }

    public LevelConfig GetCurrentLevel()
    {
        if (levels == null || levels.Length == 0) return null;
        _currentLevelIndex = Mathf.Clamp(_currentLevelIndex, 0, levels.Length - 1);
        return levels[_currentLevelIndex];
    }

    public void SetLevelIndex(int index)
    {
        if (levels == null || levels.Length == 0) return;
        _currentLevelIndex = Mathf.Clamp(index, 0, levels.Length - 1);
        SaveProgress();
    }

    public void MarkLevelCompleted()
    {
        if (levels == null || levels.Length == 0) return;
        if (_currentLevelIndex < levels.Length - 1) _currentLevelIndex++;
        SaveProgress();
    }

    public int GetSavedLevelIndex()
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, 0), 0, Mathf.Max(0, levels.Length - 1));
    }

    public void StartNewGame()
    {
        SetLevelIndex(0);
        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        _currentLevelIndex = GetSavedLevelIndex();
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(KEY_CURRENT_LEVEL, _currentLevelIndex);
        PlayerPrefs.Save();
    }

    // -------------------------------------------------
    // Backward compatibility (старый UI/кнопки)
    // -------------------------------------------------

    // Старые кнопки "Next level" / "Reload scene" могли вызывать этот метод
    public void ReloadGameScene()
    {
        // Просто перезагружаем игровую сцену
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    // Старые кнопки "Restart" могли вызывать этот метод
    public void RestartCurrentLevel()
    {
        // Перезагружаем игровую сцену
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }
    // Текущий номер уровня для UI/логики (1..N)
    public int GetCurrentLevelNumber()
    {
        if (levels == null || levels.Length == 0)
            return 1;

        return Mathf.Clamp(_currentLevelIndex + 1, 1, levels.Length);
    }

    public int GetTotalLevels()
    {
        return levels == null ? 0 : levels.Length;
    }


}
