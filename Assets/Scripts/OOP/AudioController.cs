using UnityEngine;

// Этот класс хранит клипы и умеет их проигрывать через AudioSource-ы.
public class AudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioSource tickSource;

    private AudioClip _correct;
    private AudioClip _wrong;


    // Клипы уровня
    private AudioClip _music;
    private AudioClip _tick;
    private AudioClip _breathing;
    private AudioClip _warning;
    private AudioClip _screamer;

    // LevelLoader задаёт клипы отсюда
    public void SetAudioClips(
    AudioClip music,
    AudioClip tick,
    AudioClip breathing,
    AudioClip warning,
    AudioClip screamer,
    AudioClip correct,
    AudioClip wrong)
    {
        _music = music;
        _tick = tick;
        _breathing = breathing;
        _warning = warning;
        _screamer = screamer;
        _correct = correct;
        _wrong = wrong;
    }

    public void PlayCorrect()
    {
        PlaySfx(_correct);
    }

    public void PlayWrong()
    {
        PlaySfx(_wrong);
    }

    // Фоновая музыка
    public void PlayMusic()
    {
        // Сначала проверяем ссылки
        if (musicSource == null || _music == null)
        {
            Debug.LogWarning("PlayMusic: musicSource or _music is NULL");
            return;
        }

        // Потом запускаем
        musicSource.clip = _music;
        musicSource.loop = true;
        musicSource.Play();

        Debug.Log("PlayMusic OK. source=" + musicSource.name + " clip=" + _music.name);
    }

    // Один звук (SFX)
    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    //public void PlayTick() => PlaySfx(_tick);
    public void PlayBreathing() => PlaySfx(_breathing);
    public void PlayWarning() => PlaySfx(_warning);
    public void PlayScreamer() => PlaySfx(_screamer);

    // Остановить ВСЕ эффекты (SFX)
    public void StopSfx()
    {
        if (tickSource != null)
            tickSource.Stop();

        if (sfxSource != null)
            sfxSource.Stop();
    }


    // Остановить вообще всё: музыку и эффекты
    public void StopAll()
    {
        if (tickSource != null)
            tickSource.Stop();

        if (musicSource != null)
            musicSource.Stop();

        if (sfxSource != null)
            sfxSource.Stop();
    }

    public void StopTick()
    {
        if (tickSource != null)
            tickSource.Stop();
    }
    // Включить/выключить тиканье (loop)
    public void SetTicking(bool enabled)
    {
        if (tickSource == null || _tick == null)
            return;

        // Если хотим включить
        if (enabled)
        {
            // Если уже играет — ничего не делаем (важно против "двоения")
            if (tickSource.isPlaying)
                return;

            tickSource.clip = _tick;
            tickSource.loop = true;
            tickSource.Play();
        }
        else
        {
            // Выключаем тик сразу
            if (tickSource.isPlaying)
                tickSource.Stop();
        }
    }
    public void ApplyVolumes(float musicVol, float sfxVol)
    {
        
        if (musicSource != null) musicSource.volume = musicVol;
        if (sfxSource != null) sfxSource.volume = sfxVol;
        if (tickSource != null) tickSource.volume = sfxVol;
        Debug.Log($"ApplyVolumes music={musicVol} sfx={sfxVol}");

    }
    private void OnEnable()
    {
        AudioSettingsModel.VolumesChanged += ApplyVolumes;
    }

    private void OnDisable()
    {
        AudioSettingsModel.VolumesChanged -= ApplyVolumes;
    }

    private void Start()
    {
        // синхронизация при старте сцены (если слайдеры не трогали)
        ApplyVolumes(AudioSettingsModel.GetMusic(), AudioSettingsModel.GetSfx());
    }

}
