/*using UnityEngine;
using TMPro;
using UnityEngine.UI;
using YG; // PluginYG2

public class ResultView : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("UI")]
    [SerializeField] private TMP_Text resultText;

    [Header("Buttons Objects (show/hide)")]
    [SerializeField] private GameObject nextLevelButtonObj;
    [SerializeField] private GameObject restartButtonObj;

    [Header("Buttons Components (for click binding)")]
    [SerializeField] private Button nextLevelButton;   // компонент Button с NextLevelButton
    [SerializeField] private Button restartButton;     // компонент Button с RestartButton
    [SerializeField] private Button menuButton;        // компонент Button с MenuButton

    private bool _wired;

    private bool _adInProgress;
    private System.Action _pendingAction;
    private float _adStartTime;
    [SerializeField] private float adFallbackTimeout = 3.0f; // если фокус не теряется


    private void Awake()
    {
        // Прячем результат на старте
        Hide();

        // Один раз назначаем клики (самое надёжное)
        WireButtons();
    }

    private void WireButtons()
    {
        if (_wired) return;
        _wired = true;

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(OnNextClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OnMenuClicked);
        }
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (nextLevelButtonObj != null)
            nextLevelButtonObj.SetActive(false);

        if (restartButtonObj != null)
            restartButtonObj.SetActive(false);
    }

    public void ShowWin()
    {
        WireButtons();
        BringToFront(); // важно: чтобы клики не “терялись” под другими UI

        Show("Вы выиграли");

        if (nextLevelButtonObj != null)
            nextLevelButtonObj.SetActive(true);

        if (restartButtonObj != null)
            restartButtonObj.SetActive(false);

       // TryShowInterstitial();
        StickyBannerController.Set(true);
    }

    public void ShowLose()
    {
        WireButtons();
        BringToFront();

        Show("Вы проиграли");

        if (restartButtonObj != null)
            restartButtonObj.SetActive(true);

        if (nextLevelButtonObj != null)
            nextLevelButtonObj.SetActive(false);

        //TryShowInterstitial();
        StickyBannerController.Set(true);
    }

    private void Show(string text)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (resultText != null)
            resultText.text = text;
    }

    /// <summary>
    /// Делает панель результата последней в иерархии UI,
    /// чтобы её не перекрывали другие элементы Canvas.
    /// </summary>
    private void BringToFront()
    {
        // panelRoot обычно лежит внутри Canvas.
        // Поднимем именно его (если есть), иначе поднимем этот объект.
        if (panelRoot != null)
            panelRoot.transform.SetAsLastSibling();
        else
            transform.SetAsLastSibling();
    }

    *//*private void OnNextClicked()
    {
        //Debug.Log("[ResultView] Next clicked");
        AudioListener.pause = false;

        LevelManager.Instance?.ReloadGameScene();
        Hide();
    }*//*

    private void OnNextClicked()
    {
        // Следующий уровень (у тебя ReloadGameScene — ок)
        ShowInterstitialThen(() =>
        {
            LevelManager.Instance?.ReloadGameScene();
            Hide();
        });
    }

    private void OnRestartClicked()
    {
        ShowInterstitialThen(() =>
        {
            LevelManager.Instance?.RestartCurrentLevel();
            Hide();
        });
    }

    *//*private void OnRestartClicked()
    {
        //Debug.Log("[ResultView] Restart clicked");
        AudioListener.pause = false;

        LevelManager.Instance?.RestartCurrentLevel();
        Hide();
    }*//*

    private void OnMenuClicked()
    {
        //Debug.Log("[ResultView] Menu clicked");
        AudioListener.pause = false;

        LevelManager.Instance?.GoToMenu();
        Hide();
    }

    private void ShowInterstitialThen(System.Action afterAd)
    {
        if (_adInProgress) return; // защита от двойного клика
        _pendingAction = afterAd;

#if UNITY_WEBGL && !UNITY_EDITOR
    if (YG2.isSDKEnabled)
    {
        _adInProgress = true;
        _adStartTime = Time.unscaledTime;

        // Чтобы кнопки не нажимались повторно
        SetAllButtonsInteractable(false);

        // Пауза звука только на время рекламы
        AudioListener.pause = true;

        YG2.InterstitialAdvShow();
        return;
    }
#endif

        // если не WebGL/нет SDK — выполняем сразу
        _pendingAction?.Invoke();
        _pendingAction = null;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    if (hasFocus && _adInProgress)
        FinishAdFlow();
#endif
    }

    private void Update()
    {
        // fallback: иногда фокус не меняется
        if (_adInProgress && (Time.unscaledTime - _adStartTime) > adFallbackTimeout)
            FinishAdFlow();
    }

    private void FinishAdFlow()
    {
        if (!_adInProgress) return;

        _adInProgress = false;

        AudioListener.pause = false;
        SetAllButtonsInteractable(true);

        var action = _pendingAction;
        _pendingAction = null;
        action?.Invoke();
    }

    private void SetAllButtonsInteractable(bool value)
    {
        if (nextLevelButton != null) nextLevelButton.interactable = value;
        if (restartButton != null) restartButton.interactable = value;
        if (menuButton != null) menuButton.interactable = value;
    }


    *//*  private void TryShowInterstitial()
      {
          //Debug.Log($"[ADS] TryShowInterstitial: isSDKEnabled={YG2.isSDKEnabled}");

  #if UNITY_WEBGL && !UNITY_EDITOR
          // показываем только если SDK реально инициализирован
          if (YG2.isSDKEnabled)
          {
              AudioListener.pause = true;
              YG2.InterstitialAdvShow();
          }
  #endif
      }*//*
}

*/

/*using UnityEngine;
using TMPro;
using UnityEngine.UI;
using YG; // PluginYG2

public class ResultView : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("UI")]
    [SerializeField] private TMP_Text resultText;

    [Header("Buttons Objects (show/hide)")]
    [SerializeField] private GameObject nextLevelButtonObj;
    [SerializeField] private GameObject restartButtonObj;

    [Header("Buttons Components (for click binding)")]
    [SerializeField] private Button nextLevelButton;   // компонент Button с NextLevelButton
    [SerializeField] private Button restartButton;     // компонент Button с RestartButton
    [SerializeField] private Button menuButton;        // компонент Button с MenuButton

    private bool _wired;

    // --- Ad flow ---
    private bool _adInProgress;
    private System.Action _pendingAction;
    private float _adStartTime;
    private bool _focusLostDuringAd;

    [SerializeField] private float adFallbackTimeout = 3.0f; // аварийный выход (если фокус не теряется)

    private void Awake()
    {
        Hide();
        WireButtons();
    }

    private void WireButtons()
    {
        if (_wired) return;
        _wired = true;

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(OnNextClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OnMenuClicked);
        }
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (nextLevelButtonObj != null)
            nextLevelButtonObj.SetActive(false);

        if (restartButtonObj != null)
            restartButtonObj.SetActive(false);
    }

    public void ShowWin()
    {
        WireButtons();
        BringToFront();

        Show("Вы выиграли");

        if (nextLevelButtonObj != null)
            nextLevelButtonObj.SetActive(true);

        if (restartButtonObj != null)
            restartButtonObj.SetActive(false);

        StickyBannerController.Set(true);
    }

    public void ShowLose()
    {
        WireButtons();
        BringToFront();

        Show("Вы проиграли");

        if (restartButtonObj != null)
            restartButtonObj.SetActive(true);

        if (nextLevelButtonObj != null)
            nextLevelButtonObj.SetActive(false);

        StickyBannerController.Set(true);
    }

    private void Show(string text)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (resultText != null)
            resultText.text = text;
    }

    /// <summary>
    /// Делает панель результата последней в иерархии UI,
    /// чтобы её не перекрывали другие элементы Canvas.
    /// </summary>
    private void BringToFront()
    {
        if (panelRoot != null)
            panelRoot.transform.SetAsLastSibling();
        else
            transform.SetAsLastSibling();
    }

    private void OnNextClicked()
    {
        ShowInterstitialThen(() =>
        {
            // После Win() у тебя уже вызван MarkLevelCompleted(), так что перезагрузка сцены = следующий уровень
            LevelManager.Instance?.ReloadGameScene();
            Hide();
        });
    }

    private void OnRestartClicked()
    {
        ShowInterstitialThen(() =>
        {
            LevelManager.Instance?.RestartCurrentLevel();
            Hide();
        });
    }

    private void OnMenuClicked()
    {
        // если вдруг реклама "в процессе" — сбрасываем состояние
        _adInProgress = false;
        _pendingAction = null;
        _focusLostDuringAd = false;

        AudioListener.pause = false;
        SetAllButtonsInteractable(true);

        LevelManager.Instance?.GoToMenu();
        Hide();
    }

    // ---------------------------
    // Ads: show then do action
    // ---------------------------
    private void ShowInterstitialThen(System.Action afterAd)
    {
        if (_adInProgress) return; // защита от двойного клика

        _pendingAction = afterAd;

#if UNITY_WEBGL && !UNITY_EDITOR
        if (YG2.isSDKEnabled)
        {
            _adInProgress = true;
            _adStartTime = Time.unscaledTime;
            _focusLostDuringAd = false;

            SetAllButtonsInteractable(false);

            // пауза звука только на время рекламы
            AudioListener.pause = true;

            YG2.InterstitialAdvShow();
            return;
        }
#endif

        // если не WebGL/нет SDK — выполняем сразу
        _pendingAction?.Invoke();
        _pendingAction = null;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!_adInProgress) return;

        if (!hasFocus)
        {
            _focusLostDuringAd = true;
            return;
        }

        // вернулся фокус -> считаем, что реклама закрыта
        FinishAdFlow();
#endif
    }

    private void Update()
    {
        // fallback: если фокус НИ РАЗУ не терялся, иногда можно не получить OnApplicationFocus
        if (_adInProgress && !_focusLostDuringAd && (Time.unscaledTime - _adStartTime) > adFallbackTimeout)
            FinishAdFlow();
    }

    private void FinishAdFlow()
    {
        if (!_adInProgress) return;

        _adInProgress = false;

        AudioListener.pause = false;
        SetAllButtonsInteractable(true);

        var action = _pendingAction;
        _pendingAction = null;
        action?.Invoke();
    }

    private void SetAllButtonsInteractable(bool value)
    {
        if (nextLevelButton != null) nextLevelButton.interactable = value;
        if (restartButton != null) restartButton.interactable = value;
        if (menuButton != null) menuButton.interactable = value;
    }
}*/
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("UI")]
    [SerializeField] private TMP_Text resultText;

    [Header("Buttons Objects (show/hide)")]
    [SerializeField] private GameObject nextLevelButtonObj;
    [SerializeField] private GameObject restartButtonObj;

    [Header("Buttons Components (for click binding)")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    private bool _wired;

    private void Awake()
    {
        Hide();
        WireButtons();
    }

    private void WireButtons()
    {
        if (_wired) return;
        _wired = true;

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(OnNextClicked);
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OnMenuClicked);
        }
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (nextLevelButtonObj != null) nextLevelButtonObj.SetActive(false);
        if (restartButtonObj != null) restartButtonObj.SetActive(false);
    }

    public void ShowWin()
    {
        WireButtons();
        BringToFront();
        Show("Вы выиграли");
        if (nextLevelButtonObj != null) nextLevelButtonObj.SetActive(true);
        if (restartButtonObj != null) restartButtonObj.SetActive(false);

        StickyBannerController.Set(true);
    }

    public void ShowLose()
    {
        WireButtons();
        BringToFront();
        Show("Вы проиграли");
        if (restartButtonObj != null) restartButtonObj.SetActive(true);
        if (nextLevelButtonObj != null) nextLevelButtonObj.SetActive(false);

        StickyBannerController.Set(true);
    }

    private void Show(string text)
    {
        if (panelRoot != null) panelRoot.SetActive(true);
        if (resultText != null) resultText.text = text;
    }

    private void BringToFront()
    {
        if (panelRoot != null) panelRoot.transform.SetAsLastSibling();
        else transform.SetAsLastSibling();
    }

    private void OnNextClicked()
    {
        SetAllButtonsInteractable(false);

        YG2InterstitialRunner.Instance.Show(() =>
        {
            LevelManager.Instance?.ReloadGameScene();
            Hide();
        });
    }

    private void OnRestartClicked()
    {
        SetAllButtonsInteractable(false);

        YG2InterstitialRunner.Instance.Show(() =>
        {
            LevelManager.Instance?.RestartCurrentLevel();
            Hide();
        });
    }

    private void OnMenuClicked()
    {
        SetAllButtonsInteractable(false);

        // В меню можно без рекламы (или по желанию тоже через Runner)
        LevelManager.Instance?.GoToMenu();
        Hide();
    }

    private void SetAllButtonsInteractable(bool value)
    {
        if (nextLevelButton != null) nextLevelButton.interactable = value;
        if (restartButton != null) restartButton.interactable = value;
        if (menuButton != null) menuButton.interactable = value;
    }
}

