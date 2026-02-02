using UnityEngine;
using YG;

public static class StickyBannerController
{
    public static void Set(bool enabled)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!YG2.isSDKEnabled) return;

        // В PluginYG2 баннер включается/выключается вот так
        YG2.StickyAdActivity(enabled);
#endif
    }
}
