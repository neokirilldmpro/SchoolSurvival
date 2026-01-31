using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button progressButton;

    [Header("Progress UI (optional)")]
    [SerializeField] private TextMeshProUGUI progressButtonText; // текст на кнопке прогресса

    private void Start()
    {
        /*PlayerPrefs.DeleteKey("MusicVolume");
        PlayerPrefs.DeleteKey("SfxVolume");
        PlayerPrefs.Save();*/
        RefreshProgressButton();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    
    private void OnEnable()
    {
        RefreshProgressButton();
    }

    //  нопка "»грать" Ч начать с 1 уровн€
    public void Play()
    {
        if (WebGLAudioUnlocker.Instance != null)
            WebGLAudioUnlocker.Instance.Unlock();

        if (LevelManager.Instance != null)
            LevelManager.Instance.StartNewGame();
        

    }

    //  нопка "ѕрогресс" Ч продолжить
    public void Progress()
    {
        if (WebGLAudioUnlocker.Instance != null)
            WebGLAudioUnlocker.Instance.Unlock();

        if (LevelManager.Instance != null)
            LevelManager.Instance.ContinueGame();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // ƒелает кнопку прогресса серой/недоступной, если прогресса нет
    private void RefreshProgressButton()
    {
        if (progressButton == null || LevelManager.Instance == null)
            return;

        int savedIndex = LevelManager.Instance.GetSavedLevelIndex(); // 0..N-1
        bool hasProgress = savedIndex > 0; // прогресс считаетс€, если дошЄл дальше 1 уровн€

        progressButton.interactable = hasProgress;

        //  расивый текст на кнопке (если есть ссылка)
        if (progressButtonText != null)
        {
            if (hasProgress)
            {
                int levelNumber = savedIndex + 1;
                int total = LevelManager.Instance.GetTotalLevels();
                progressButtonText.text = $"ѕ–ќƒќЋ∆»“№ (”–ќ¬≈Ќ№ {levelNumber}/{total})";
            }
            else
            {
                progressButtonText.text = "ѕ–ќƒќЋ∆»“№ (Ќ≈“ ѕ–ќ√–≈——ј)";
            }
        }
    }
}
