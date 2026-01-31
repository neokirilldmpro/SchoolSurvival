using UnityEngine;

public class WebGLAudioUnlocker : MonoBehaviour
{
    public static WebGLAudioUnlocker Instance { get; private set; }

    [SerializeField] private AudioSource source;

    private bool _unlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (source == null) source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = false;
        source.volume = 0f; // важно: “беззвучный” запуск
    }

    // Вызывать строго из обработчика кнопки (клик игрока)
    public void Unlock()
    {
        if (_unlocked) return;

        // В WebGL сам факт Play() в клик — обычно “поднимает” AudioContext
        source.Play();
        source.Stop();

        _unlocked = true;
        Debug.Log("WebGL audio unlocked");
    }
}
