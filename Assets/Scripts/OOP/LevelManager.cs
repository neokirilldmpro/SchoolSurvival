using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

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

    private const float CLOUD_SAVE_DEBOUNCE = 0.75f;
    private bool _applyingCloud;

    /// <summary>
    /// Запланировать облачное сохранение (debounce), чтобы не спамить SaveProgress при движении слайдера.
    /// </summary>
    public void RequestCloudSave()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    if (!YG2.isSDKEnabled) return;
    if (_applyingCloud) return;

    CancelInvoke(nameof(DoCloudSave));
    Invoke(nameof(DoCloudSave), CLOUD_SAVE_DEBOUNCE);
#endif
    }

    private void DoCloudSave()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    if (!YG2.isSDKEnabled) return;
    if (_applyingCloud) return;

    YG2.saves.currentLevel = _currentLevelIndex + 1;
    YG2.saves.musicVolume = AudioSettingsModel.GetMusic();
    YG2.saves.sfxVolume   = AudioSettingsModel.GetSfx();

    YG2.SaveProgress();
    Debug.Log($"[SAVE] Cloud saved (debounced): level={YG2.saves.currentLevel} music={YG2.saves.musicVolume} sfx={YG2.saves.sfxVolume}");
#endif
    }


    private void Awake()
    {// это обновление памяти 
        /*PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.Save();*/
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _currentLevelIndex = PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, 0);
        //Debug.Log($"[SAVE] Loaded local in Awake: index={_currentLevelIndex}");
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
        // Локально (как было)
        PlayerPrefs.SetInt(KEY_CURRENT_LEVEL, _currentLevelIndex);
        PlayerPrefs.Save();
        //Debug.Log($"[SAVE] Saved local: index={_currentLevelIndex}");

#if UNITY_WEBGL && !UNITY_EDITOR
    // В облако: сохраняем номер уровня 1..N
    if (YG2.isSDKEnabled)
    {
        YG2.saves.currentLevel = _currentLevelIndex + 1;

        // Громкости — если хочешь хранить тоже в облаке
        YG2.saves.musicVolume = AudioSettingsModel.GetMusic();
        YG2.saves.sfxVolume   = AudioSettingsModel.GetSfx();

        YG2.SaveProgress();
        Debug.Log($"[SAVE] Saved to cloud: level={YG2.saves.currentLevel}");
    }
#endif
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

#if UNITY_WEBGL && !UNITY_EDITOR
    private void OnEnable()
    {
        YG2.onGetSDKData += ApplySavesFromYG;
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= ApplySavesFromYG;
    }
#endif



    /*private void ApplySavesFromYG()
    {

        // cloud level хранится как 1..N
        int cloudLvl = YG2.saves.currentLevel;
        if (cloudLvl < 1) cloudLvl = 1;

        int cloudIdx = cloudLvl - 1;
        cloudIdx = Mathf.Clamp(cloudIdx, 0, Mathf.Max(0, levels.Length - 1));

        // локальный прогресс (0..N-1)
        int localIdx = PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, 0);

        // НИКОГДА не откатываем прогресс назад:
        _currentLevelIndex = Mathf.Max(localIdx, cloudIdx);

        // синхронизируем PlayerPrefs без вызова SaveProgress (чтобы не триггерить облако повторно)
        PlayerPrefs.SetInt(KEY_CURRENT_LEVEL, _currentLevelIndex);
        PlayerPrefs.Save();

        // звук (если используешь облако и для громкости)
        AudioSettingsModel.SetMusic(YG2.saves.musicVolume);
        AudioSettingsModel.SetSfx(YG2.saves.sfxVolume);

        Debug.Log($"[SAVE] Apply cloud: cloud={cloudIdx} local={localIdx} => use={_currentLevelIndex}");
    }*/
    private void ApplySavesFromYG()
    {
    #if UNITY_WEBGL && !UNITY_EDITOR
        _applyingCloud = true;
    #endif

        // Если уровней нет — нечего применять
        if (levels == null || levels.Length == 0)
        {
            Debug.LogWarning("[SAVE] ApplySavesFromYG: levels array is empty");
    #if UNITY_WEBGL && !UNITY_EDITOR
        _applyingCloud = false;
    #endif
            return;
        }

        // --- 1) Cloud: currentLevel хранится как 1..N ---
        int cloudLvl = 1;
    #if UNITY_WEBGL && !UNITY_EDITOR
        if (YG2.saves != null)
            cloudLvl = Mathf.Max(1, YG2.saves.currentLevel);
    #endif
        int cloudIdx = cloudLvl - 1; // -> 0..N-1
        cloudIdx = Mathf.Clamp(cloudIdx, 0, levels.Length - 1);

        // --- 2) Local: у тебя в PlayerPrefs индекс 0..N-1 ---
        int localIdx = PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, 0);
        localIdx = Mathf.Clamp(localIdx, 0, levels.Length - 1);

        // --- 3) Никогда не откатываем прогресс назад ---
        _currentLevelIndex = Mathf.Max(localIdx, cloudIdx);
        _currentLevelIndex = Mathf.Clamp(_currentLevelIndex, 0, levels.Length - 1);

        // --- 4) Синхронизируем локально (без SaveProgress, чтобы не триггерить облако повторно) ---
        PlayerPrefs.SetInt(KEY_CURRENT_LEVEL, _currentLevelIndex);
        PlayerPrefs.Save();

        // --- 5) Применяем громкости из облака (если используешь облако для настроек) ---
    #if UNITY_WEBGL && !UNITY_EDITOR
    if (YG2.saves != null)
    {
        float mv = Mathf.Clamp01(YG2.saves.musicVolume);
        float sv = Mathf.Clamp01(YG2.saves.sfxVolume);

        AudioSettingsModel.SetMusic(mv);
        AudioSettingsModel.SetSfx(sv);

        // Если у тебя ещё где-то читается MusicVolume/SfxVolume из PlayerPrefs — синхронизируем
        PlayerPrefs.SetFloat("MusicVolume", mv);
        PlayerPrefs.SetFloat("SfxVolume", sv);
        PlayerPrefs.Save();
    }
    #endif

        Debug.Log($"[SAVE] Apply cloud/local: cloudIdx={cloudIdx} (cloudLvl={cloudLvl}) localIdx={localIdx} => useIdx={_currentLevelIndex}");

    #if UNITY_WEBGL && !UNITY_EDITOR
        _applyingCloud = false;
    #endif
    }




}
