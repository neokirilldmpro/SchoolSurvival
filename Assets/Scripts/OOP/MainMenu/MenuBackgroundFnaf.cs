using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackgroundFnaf : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image baseImage;   // обычный фон (и "училка прямо")
    [SerializeField] private Image scareImage;  // "наклонённая" (страшная) поверх

    [Header("Sprites")]
    [SerializeField] private Sprite normalBackground;   // картинка 1 (фон без училки)
    [SerializeField] private Sprite teacherStraight;    // картинка 2 (училка прямо)
    [SerializeField] private Sprite teacherTilted;      // картинка 3 (училка наклонённая)

    [Header("Subtle flicker")]
    [SerializeField] private float flickerSpeed = 6f;   // 4..10
    [SerializeField] private float flickerAmount = 0.06f; // 0..0.15

    [Header("Straight flick (часто)")]
    [SerializeField] private Vector2 straightInterval = new Vector2(2.5f, 6.0f);
    [SerializeField] private Vector2 straightDuration = new Vector2(0.06f, 0.18f);

    [Header("Scare flash (редко)")]
    [SerializeField] private Vector2 scareInterval = new Vector2(8f, 20f);
    [SerializeField] private Vector2 scareDuration = new Vector2(0.03f, 0.10f);

    private Coroutine _loop;

    private void OnEnable()
    {
        if (baseImage != null)
        {
            baseImage.sprite = normalBackground;
            SetAlpha(baseImage, 1f);
        }

        if (scareImage != null)
        {
            scareImage.sprite = teacherTilted;
            scareImage.gameObject.SetActive(true);
            SetAlpha(scareImage, 0f);
        }

        _loop = StartCoroutine(Loop());
    }

    private void OnDisable()
    {
        if (_loop != null) StopCoroutine(_loop);
        _loop = null;

        if (scareImage != null) SetAlpha(scareImage, 0f);
        if (baseImage != null) baseImage.sprite = normalBackground;
    }

    private void Update()
    {
        // лёгкое мерцание (дёшево для WebGL)
        if (baseImage == null) return;

        float t = Mathf.Sin(Time.unscaledTime * flickerSpeed) * 0.5f + 0.5f; // 0..1
        float a = 1f - flickerAmount + (t * flickerAmount);
        var c = baseImage.color;
        baseImage.color = new Color(c.r, c.g, c.b, a);
    }

    private IEnumerator Loop()
    {
        float nextStraightAt = Time.unscaledTime + Random.Range(straightInterval.x, straightInterval.y);
        float nextScareAt = Time.unscaledTime + Random.Range(scareInterval.x, scareInterval.y);

        while (true)
        {
            float now = Time.unscaledTime;

            // "миг" на училку прямо (часто)
            if (now >= nextStraightAt)
            {
                yield return StraightFlash();
                nextStraightAt = Time.unscaledTime + Random.Range(straightInterval.x, straightInterval.y);
            }

            // страшный кадр (редко, очень коротко)
            if (now >= nextScareAt)
            {
                yield return ScareFlash();

                // иногда двойной удар как FNAF
                if (Random.value < 0.35f)
                {
                    yield return new WaitForSecondsRealtime(Random.Range(0.02f, 0.08f));
                    yield return ScareFlash();
                }

                nextScareAt = Time.unscaledTime + Random.Range(scareInterval.x, scareInterval.y);
            }

            yield return null;
        }
    }

    private IEnumerator StraightFlash()
    {
        if (baseImage == null || teacherStraight == null || normalBackground == null)
            yield break;

        baseImage.sprite = teacherStraight;
        yield return new WaitForSecondsRealtime(Random.Range(straightDuration.x, straightDuration.y));
        baseImage.sprite = normalBackground;
    }

    private IEnumerator ScareFlash()
    {
        if (scareImage == null) yield break;

        if (teacherTilted != null)
            scareImage.sprite = teacherTilted;

        SetAlpha(scareImage, 1f);
        yield return new WaitForSecondsRealtime(Random.Range(scareDuration.x, scareDuration.y));
        SetAlpha(scareImage, 0f);
    }

    private static void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        var c = img.color;
        img.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(a));
    }
}
