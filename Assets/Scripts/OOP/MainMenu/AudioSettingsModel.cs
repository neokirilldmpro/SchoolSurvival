/*using UnityEngine;

public static class AudioSettingsModel
{
    private const string KEY_MUSIC = "MusicVolume";
    private const string KEY_SFX = "SfxVolume";

    public static float GetMusic() => PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
    public static float GetSfx() => PlayerPrefs.GetFloat(KEY_SFX, 1f);

    public static void SetMusic(float v)
    {
        PlayerPrefs.SetFloat(KEY_MUSIC, Mathf.Clamp01(v));
        PlayerPrefs.Save();
    }

    public static void SetSfx(float v)
    {
        PlayerPrefs.SetFloat(KEY_SFX, Mathf.Clamp01(v));
        PlayerPrefs.Save();
    }
}
*/
using System;
using UnityEngine;

public static class AudioSettingsModel
{
    private const string KEY_MUSIC = "MusicVolume";
    private const string KEY_SFX = "SfxVolume";

    public static event Action<float, float> VolumesChanged;

    public static float GetMusic() => PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
    public static float GetSfx() => PlayerPrefs.GetFloat(KEY_SFX, 1f);

    public static void SetMusic(float v)
    {
        v = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(KEY_MUSIC, v);
        PlayerPrefs.Save();
        RaiseChanged();
    }

    public static void SetSfx(float v)
    {
        v = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(KEY_SFX, v);
        PlayerPrefs.Save();
        RaiseChanged();
    }

    public static void RaiseChanged()
    {
        VolumesChanged?.Invoke(GetMusic(), GetSfx());
    }

    public static void ResetToDefault()
    {
        PlayerPrefs.SetFloat(KEY_MUSIC, 1f);
        PlayerPrefs.SetFloat(KEY_SFX, 1f);
        PlayerPrefs.Save();
        RaiseChanged();
    }

    public static void ClearKeys()
    {
        PlayerPrefs.DeleteKey(KEY_MUSIC);
        PlayerPrefs.DeleteKey(KEY_SFX);
        PlayerPrefs.Save();
        RaiseChanged();
    }
}
