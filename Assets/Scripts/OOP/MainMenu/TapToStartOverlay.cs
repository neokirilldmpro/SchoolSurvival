using UnityEngine;
using UnityEngine.UI;

public class TapToStartOverlay : MonoBehaviour
{
    private const string KEY_TAP_DONE = "TapToStartDone";

    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private Button tapButton;
    [SerializeField] private AudioClip menuMusic;

    private bool _clicked;
    private static bool _tapDoneThisSession;
    private void Awake()
    {
        if (overlayRoot == null) overlayRoot = gameObject;

        if (_tapDoneThisSession)
        {
            overlayRoot.SetActive(false);
            return;
        }

        overlayRoot.SetActive(true);
        transform.SetAsLastSibling();
    }

    private void OnEnable()
    {
        if (tapButton != null)
        {
            tapButton.onClick.RemoveListener(OnTap);
            tapButton.onClick.AddListener(OnTap);
        }
    }

    private void OnDisable()
    {
        if (tapButton != null)
            tapButton.onClick.RemoveListener(OnTap);
    }

    private void OnTap()
    {
        if (_clicked) return;
        _clicked = true;

        _tapDoneThisSession = true;

        WebGLAudioUnlocker.Instance?.Unlock();
        if (menuMusic != null)
            AudioController.Instance?.PlayMenuMusic(menuMusic);

        overlayRoot.SetActive(false);
    }
}
