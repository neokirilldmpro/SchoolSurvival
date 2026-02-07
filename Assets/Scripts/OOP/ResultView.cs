/*using UnityEngine;
using TMPro;
using YG; // PluginYG2

public class ResultView : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("UI")]
    [SerializeField] private TMP_Text resultText;

    [Header("Buttons")]
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private GameObject restartButton;

    [SerializeField] private UnityEngine.UI.Button menuButton;


    private void Awake()
    {
        Hide();
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (nextLevelButton != null)
            nextLevelButton.SetActive(false);

        if (restartButton != null)
            restartButton.SetActive(false);
    }

    public void ShowWin()
    {
        Show("Вы выиграли");

        if (nextLevelButton != null)
            nextLevelButton.SetActive(true);

        if (restartButton != null)
            restartButton.SetActive(false);

        TryShowInterstitial();
        StickyBannerController.Set(true);

    }

    public void ShowLose()
    {
        Show("Вы проиграли");

        if (restartButton != null)
            restartButton.SetActive(true);

        if (nextLevelButton != null)
            nextLevelButton.SetActive(false);

        TryShowInterstitial();
        StickyBannerController.Set(true);

    }

    private void Show(string text)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (resultText != null)
            resultText.text = text;
    }

    private void TryShowInterstitial()
    {
        Debug.Log($"[ADS] TryShowInterstitial: isSDKEnabled={YG2.isSDKEnabled}");

#if UNITY_WEBGL && !UNITY_EDITOR
        // показываем только если SDK реально инициализирован (иначе в локальном запуске/вне Яндекса не сработает)
        if (YG2.isSDKEnabled)
        {
            AudioListener.pause = true;
            YG2.InterstitialAdvShow();
        }
#endif
    }
}
*/
/*using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Buttons")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Optional: make this canvas always on top")]
    [SerializeField] private Canvas resultCanvas; // можно назначить Canvas панели результата

    private bool _wired;

    private void Awake()
    {
        // Спрятать на старте
        if (panelRoot != null)
            panelRoot.SetActive(false);

        WireButtons(); // <-- назначаем сразу
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

    public void ShowWin()
    {
        WireButtons();            // <-- на всякий случай ещё раз
        BringToFrontIfNeeded();   // <-- поднимаем выше
        ShowCommon(true);
    }

    public void ShowLose()
    {
        WireButtons();
        BringToFrontIfNeeded();
        ShowCommon(false);
    }

    private void BringToFrontIfNeeded()
    {
        // 1) Если назначен отдельный Canvas — делаем его поверх
        if (resultCanvas != null)
        {
            resultCanvas.overrideSorting = true;
            resultCanvas.sortingOrder = 9999;
        }

        // 2) И просто на всякий случай поднимаем объект в конец иерархии
        transform.SetAsLastSibling();
    }

    private void ShowCommon(bool isWin)
    {
        if (panelRoot != null) panelRoot.SetActive(true);
        if (winPanel != null) winPanel.SetActive(isWin);
        if (losePanel != null) losePanel.SetActive(!isWin);

        // Важно: когда показываешь результат — лучше выключить кликабельность нижнего UI (если есть)
        // Это делается либо CanvasGroup, либо просто скрытием UI-панели вопросов.
    }

    private void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    private void OnNextClicked()
    {
        Debug.Log("[ResultView] Next clicked");
        AudioListener.pause = false;

        LevelManager.Instance?.ReloadGameScene();
        Hide();
    }

    private void OnRestartClicked()
    {
        Debug.Log("[ResultView] Restart clicked");
        AudioListener.pause = false;

        LevelManager.Instance?.RestartCurrentLevel();
        Hide();
    }

    private void OnMenuClicked()
    {
        Debug.Log("[ResultView] Menu clicked");
        AudioListener.pause = false;

        LevelManager.Instance?.GoToMenu();
        Hide();
    }
}*/
using UnityEngine;
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

        TryShowInterstitial();
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

        TryShowInterstitial();
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

    private void OnNextClicked()
    {
        //Debug.Log("[ResultView] Next clicked");
        AudioListener.pause = false;

        LevelManager.Instance?.ReloadGameScene();
        Hide();
    }

    private void OnRestartClicked()
    {
        //Debug.Log("[ResultView] Restart clicked");
        AudioListener.pause = false;

        LevelManager.Instance?.RestartCurrentLevel();
        Hide();
    }

    private void OnMenuClicked()
    {
        //Debug.Log("[ResultView] Menu clicked");
        AudioListener.pause = false;

        LevelManager.Instance?.GoToMenu();
        Hide();
    }

    private void TryShowInterstitial()
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
    }
}

