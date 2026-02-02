using UnityEngine;
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
