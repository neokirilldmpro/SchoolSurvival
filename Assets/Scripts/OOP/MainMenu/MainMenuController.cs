/*using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject buttonsPanel; // панель с 3 кнопками (Играть/Продолжить/Настройки)


    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button progressButton;

    [Header("Progress UI (optional)")]
    [SerializeField] private TextMeshProUGUI progressButtonText; // текст на кнопке прогресса
    [Header("Menu Music")]
    [SerializeField] private AudioClip menuMusic;
    
    



    private void Start()
    {
        *//*
        StickyBannerController.Set(true);

        RefreshProgressButton();
    }
    
    
    private void OnEnable()
    {
        RefreshProgressButton();
    }

    // Кнопка "Играть" — начать с 1 уровня
    public void Play()
    {
        if (WebGLAudioUnlocker.Instance != null)
            WebGLAudioUnlocker.Instance.Unlock();
        AudioController.Instance?.PlayMenuMusic(menuMusic);
        if (LevelManager.Instance != null)
            LevelManager.Instance.StartNewGame();
        
        
        




    }

    // Кнопка "Прогресс" — продолжить
    public void Progress()
    {
        if (WebGLAudioUnlocker.Instance != null)
            WebGLAudioUnlocker.Instance.Unlock();
        AudioController.Instance?.PlayMenuMusic(menuMusic);
        if (LevelManager.Instance != null)
            LevelManager.Instance.ContinueGame();

        
        

    }

    public void OpenSettings()
    {
        if (buttonsPanel != null)
            buttonsPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        WebGLAudioUnlocker.Instance?.Unlock();
        AudioController.Instance?.PlayMenuMusic(menuMusic);
    }


    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (buttonsPanel != null)
            buttonsPanel.SetActive(true);

        RefreshProgressButton(); // на всякий случай обновим текст/активность
    }


    // Делает кнопку прогресса серой/недоступной, если прогресса нет
    private void RefreshProgressButton()
    {
        if (progressButton == null || LevelManager.Instance == null)
            return;

        int savedIndex = LevelManager.Instance.GetSavedLevelIndex(); // 0..N-1
        bool hasProgress = savedIndex > 0; // прогресс считается, если дошёл дальше 1 уровня

        progressButton.interactable = hasProgress;

        // Красивый текст на кнопке (если есть ссылка)
        if (progressButtonText != null)
        {
            if (hasProgress)
            {
                int levelNumber = savedIndex + 1;
                int total = LevelManager.Instance.GetTotalLevels();
                progressButtonText.text = $"ПРОДОЛЖИТЬ (УРОВЕНЬ {levelNumber}/{total})";
            }
            else
            {
                progressButtonText.text = "ПРОДОЛЖИТЬ (НЕТ ПРОГРЕССА)";
            }
        }
    }
}
*/
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_WEBGL && !UNITY_EDITOR
using YG;
#endif

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject buttonsPanel; // панель с кнопками (Играть/Продолжить/Настройки)

    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button progressButton;

    [Header("Progress UI (optional)")]
    [SerializeField] private TextMeshProUGUI progressButtonText;

    [Header("Menu Music")]
    [SerializeField] private AudioClip menuMusic;

    private void OnEnable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Важно: когда Яндекс SDK отдаст данные, LevelManager применит сейвы,
        // и мы должны обновить кнопку "Продолжить"
        YG2.onGetSDKData += OnYGData;
#endif
        RefreshProgressButton();
    }

    private void OnDisable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        YG2.onGetSDKData -= OnYGData;
#endif
    }

    private void Start()
    {
        StickyBannerController.Set(true);
        RefreshProgressButton();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private void OnYGData()
    {
        // SDK data пришли -> прогресс мог измениться
        RefreshProgressButton();
    }
#endif

    // Кнопка "Играть" — начать с 1 уровня
    public void Play()
    {
        WebGLAudioUnlocker.Instance?.Unlock();
        AudioController.Instance?.PlayMenuMusic(menuMusic);

        LevelManager.Instance?.StartNewGame();
    }

    // Кнопка "Продолжить"
    public void Progress()
    {
        WebGLAudioUnlocker.Instance?.Unlock();
        AudioController.Instance?.PlayMenuMusic(menuMusic);

        LevelManager.Instance?.ContinueGame();
    }

    public void OpenSettings()
    {
        if (buttonsPanel != null)
            buttonsPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        WebGLAudioUnlocker.Instance?.Unlock();
        AudioController.Instance?.PlayMenuMusic(menuMusic);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (buttonsPanel != null)
            buttonsPanel.SetActive(true);

        RefreshProgressButton();
    }

    private void RefreshProgressButton()
    {
        if (progressButton == null || LevelManager.Instance == null)
            return;

        int savedIndex = LevelManager.Instance.GetSavedLevelIndex(); // 0..N-1
        bool hasProgress = savedIndex > 0; // прогресс если дошел дальше 1 уровня

        progressButton.interactable = hasProgress;

        if (progressButtonText != null)
        {
            if (hasProgress)
            {
                int levelNumber = savedIndex + 1;
                int total = LevelManager.Instance.GetTotalLevels();
                progressButtonText.text = $"ПРОДОЛЖИТЬ (УРОВЕНЬ {levelNumber}/{total})";
            }
            else
            {
                // В Яндексе иногда SDK подгружается чуть позже.
                // Можно оставить нейтральный текст:
#if UNITY_WEBGL && !UNITY_EDITOR
                if (YG2.isSDKEnabled)
                    progressButtonText.text = "ПРОДОЛЖИТЬ (ЗАГРУЗКА...)";
                else
                    progressButtonText.text = "ПРОДОЛЖИТЬ (НЕТ ПРОГРЕССА)";
#else
                progressButtonText.text = "ПРОДОЛЖИТЬ (НЕТ ПРОГРЕССА)";
#endif
            }
        }
    }
}
