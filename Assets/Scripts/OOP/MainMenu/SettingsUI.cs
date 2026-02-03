using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Optional: preview sound in MenuScene (music volume only)")]
    [SerializeField] private AudioSource menuPreviewSource;

    private void OnEnable()
    {
        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(AudioSettingsModel.GetMusic());
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(AudioSettingsModel.GetSfx());
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        }

        ApplyMenuPreviewVolume();
    }

    private void OnDisable()
    {
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
    }

    private void OnMusicChanged(float v)
    {
        AudioSettingsModel.SetMusic(v);   // <- тут триггерится событие, AudioController обновится сам
        ApplyMenuPreviewVolume();
        LevelManager.Instance?.RequestCloudSave();
    }

    private void OnSfxChanged(float v)
    {
        AudioSettingsModel.SetSfx(v);     // <- событие
        LevelManager.Instance?.RequestCloudSave();
    }

    private void ApplyMenuPreviewVolume()
    {
        if (menuPreviewSource == null) return;

        menuPreviewSource.volume = AudioSettingsModel.GetMusic();

        // если хочешь, чтобы в меню сразу было слышно (опционально):
        // if (!menuPreviewSource.isPlaying) menuPreviewSource.Play();
    }
}
