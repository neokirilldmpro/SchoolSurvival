using System;
using UnityEngine;
using YG;

public class YG2InterstitialRunner : MonoBehaviour
{
    public static YG2InterstitialRunner Instance { get; private set; }

    
    [SerializeField] private float fallbackTimeout = 4f;
    

    private bool _waiting;
    private Action _after;
    private float _startedAt;

    // Автосоздание объекта до загрузки первой сцены
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;

        var go = new GameObject("YG2InterstitialRunner");
        DontDestroyOnLoad(go);
        go.AddComponent<YG2InterstitialRunner>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        YG2.onCloseInterAdvWasShow += OnCloseInter;
#endif
    }

    private void OnDisable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        YG2.onCloseInterAdvWasShow -= OnCloseInter;
#endif
    }

    public void Show(Action after)
    {
        if (_waiting) return;



#if UNITY_WEBGL && !UNITY_EDITOR
Debug.Log($"[ADS] Show interstitial: sdk={YG2.isSDKEnabled}");
        if (!YG2.isSDKEnabled)
        {
            after?.Invoke();
            return;
        }
        
        _waiting = true;
        _after = after;
        _startedAt = Time.unscaledTime;

        // ВАЖНО: не трогаем AudioListener.pause вручную.
        // Если в настройках PluginYG2 включен Pause Game — он сам поставит/снимет паузу.
        YG2.InterstitialAdvShow();
        return;
#else
        after?.Invoke();
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private void OnCloseInter(bool wasShown)
    {
        Finish();
    }

#endif

    private void Finish()
    {
        if (!_waiting) return;

        _waiting = false;
        var a = _after;
        _after = null;
        a?.Invoke();
    }

    private void Update()
    {
        // Если реклама не показалась/колбэк не пришел — всё равно продолжаем
        if (_waiting && (Time.unscaledTime - _startedAt) > fallbackTimeout)
            Finish();
    }
}
