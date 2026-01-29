using UnityEngine;
using TMPro;

public class ResultView : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject panelRoot;   // <-- òâîå ïîëå

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
        Show("ÂÛ ÂÛÆÈËÈ");

        if (nextLevelButton != null)
            nextLevelButton.SetActive(true);

        if (restartButton != null)
            restartButton.SetActive(false);
    }

    public void ShowLose()
    {
        Show("ÂÛ ÏÐÎÈÃÐÀËÈ");

        if (restartButton != null)
            restartButton.SetActive(true);

        if (nextLevelButton != null)
            nextLevelButton.SetActive(false);
    }

    private void Show(string text)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (resultText != null)
            resultText.text = text;
    }
}
