/*using UnityEngine;

public class ScreamerController : MonoBehaviour
{
    [Header("Transform Root")]
    [SerializeField] private Transform screamerRoot;


    [Header("Scene objects (already placed in Hierarchy)")]
    [SerializeField] private SpriteRenderer screamerRendererA;
    [SerializeField] private SpriteRenderer screamerRendererB;

    [Header("Background Renderers to tint")]
    [SerializeField] private SpriteRenderer[] backgroundRenderers;

    [SerializeField] private float blinkInterval = 0.08f;

    private bool _playing;
    private float _blinkTimer;
    private Color _bgColor = Color.red;
    private Vector3 _defaultPosition;
    private Vector3 _defaultScale;
    private string _defaultSortingLayer;
    private int _defaultOrderInLayer;
    private bool _defaultsSaved;
    [SerializeField] private Transform visualOffsetRoot;



    // Подставить картинки скримера (из LevelConfig)
    public void SetScreamerSprites(Sprite a, Sprite b)
    {
        if (screamerRendererA != null) screamerRendererA.sprite = a;
        if (screamerRendererB != null) screamerRendererB.sprite = b;

        // Выключаем скример до момента проигрыша
        if (screamerRendererA != null) screamerRendererA.gameObject.SetActive(false);
        if (screamerRendererB != null) screamerRendererB.gameObject.SetActive(false);
    }

    public void SetBackgroundColor(Color c)
    {
        _bgColor = c;
    }

    public void Play()
    {
        _playing = true;
        _blinkTimer = 0f;

        TintBackground(_bgColor);

        if (screamerRendererA != null) screamerRendererA.gameObject.SetActive(true);
        if (screamerRendererB != null) screamerRendererB.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_playing)
            return;

        _blinkTimer += Time.deltaTime;
        if (_blinkTimer >= blinkInterval)
        {
            _blinkTimer = 0f;

            bool aActive = screamerRendererA != null && screamerRendererA.gameObject.activeSelf;

            if (screamerRendererA != null) screamerRendererA.gameObject.SetActive(!aActive);
            if (screamerRendererB != null) screamerRendererB.gameObject.SetActive(aActive);
        }
    }

    private void TintBackground(Color c)
    {
        if (backgroundRenderers == null)
            return;

        foreach (var r in backgroundRenderers)
        {
            if (r != null)
                r.color = c;
        }
    }

    public void ApplyOverrides(ScreamerSetup setup)
    {
        if (screamerRoot == null || screamerRendererA == null)
            return;

        // Сохраняем дефолты один раз (как стоит в сцене / Level 1)
        if (!_defaultsSaved)
        {
            _defaultPosition = screamerRoot.position;
            _defaultScale = screamerRoot.localScale;
            _defaultSortingLayer = screamerRendererA.sortingLayerName;
            _defaultOrderInLayer = screamerRendererA.sortingOrder;
            _defaultsSaved = true;

        }


        // Сброс к дефолту
        screamerRoot.position = _defaultPosition;
        screamerRoot.localScale = _defaultScale;

        screamerRendererA.sortingLayerName = _defaultSortingLayer;
        screamerRendererB.sortingLayerName = _defaultSortingLayer;

        screamerRendererA.sortingOrder = _defaultOrderInLayer;
        screamerRendererB.sortingOrder = _defaultOrderInLayer;

        if (setup == null)
            return;

        // ---------- POSITION ----------
        if (setup.overridePosition)
        {
            Vector3 pos = setup.position;
            pos.z = screamerRoot.position.z;
            screamerRoot.position = pos;
        }

        // ---------- SCALE ----------
        if (setup.overrideScale)
        {
            screamerRoot.localScale = setup.scale;
        }

        // ---------- SORTING LAYER ----------
        if (setup.overrideSortingLayer && !string.IsNullOrEmpty(setup.sortingLayerName))
        {
            screamerRendererA.sortingLayerName = setup.sortingLayerName;
            screamerRendererB.sortingLayerName = setup.sortingLayerName;
        }

        // ---------- ORDER IN LAYER ----------
        if (setup.overrideOrderInLayer)
        {
            screamerRendererA.sortingOrder = setup.orderInLayer;
            screamerRendererB.sortingOrder = setup.orderInLayer;
        }
    }

}
*/
using UnityEngine;

public class ScreamerController : MonoBehaviour
{
    // -------------------------------------------------
    // ROOT'ы
    // -------------------------------------------------

    [Header("Transform Roots")]
    // Главный root скримера (двигаем / масштабируем)
    [SerializeField] private Transform screamerRoot;

    // Root для визуального смещения (разные композиции: голова / полный рост)
    [SerializeField] private Transform visualOffsetRoot;

    // -------------------------------------------------
    // RENDERERS
    // -------------------------------------------------

    [Header("Scene objects (already placed in Hierarchy)")]
    [SerializeField] private SpriteRenderer screamerRendererA;
    [SerializeField] private SpriteRenderer screamerRendererB;

    // -------------------------------------------------
    // BACKGROUND
    // -------------------------------------------------

    [Header("Background Renderers to tint")]
    [SerializeField] private SpriteRenderer[] backgroundRenderers;

    // -------------------------------------------------
    // BLINK
    // -------------------------------------------------

    [Header("Blink")]
    [SerializeField] private float blinkInterval = 0.08f;

    private bool _playing;
    private float _blinkTimer;
    private Color _bgColor = Color.red;

    // -------------------------------------------------
    // DEFAULT VALUES (from scene / Level 1)
    // -------------------------------------------------

    private Vector3 _defaultRootPosition;
    private Vector3 _defaultRootScale;
    private Vector3 _defaultVisualOffset;

    private string _defaultSortingLayer;
    private int _defaultOrderInLayer;

    private bool _defaultsSaved;

    // -------------------------------------------------
    // SETUP
    // -------------------------------------------------

    // Подставить картинки скримера (из LevelConfig)
    public void SetScreamerSprites(Sprite a, Sprite b)
    {
        if (screamerRendererA != null) screamerRendererA.sprite = a;
        if (screamerRendererB != null) screamerRendererB.sprite = b;

        // Скример выключен до проигрыша
        if (screamerRendererA != null) screamerRendererA.gameObject.SetActive(false);
        if (screamerRendererB != null) screamerRendererB.gameObject.SetActive(false);
    }

    public void SetBackgroundColor(Color c)
    {
        _bgColor = c;
    }

    // -------------------------------------------------
    // PLAY
    // -------------------------------------------------

    public void Play()
    {
        _playing = true;
        _blinkTimer = 0f;

        TintBackground(_bgColor);

        if (screamerRendererA != null) screamerRendererA.gameObject.SetActive(true);
        if (screamerRendererB != null) screamerRendererB.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_playing)
            return;

        _blinkTimer += Time.deltaTime;
        if (_blinkTimer >= blinkInterval)
        {
            _blinkTimer = 0f;

            bool aActive = screamerRendererA != null && screamerRendererA.gameObject.activeSelf;

            if (screamerRendererA != null) screamerRendererA.gameObject.SetActive(!aActive);
            if (screamerRendererB != null) screamerRendererB.gameObject.SetActive(aActive);
        }
    }

    // -------------------------------------------------
    // BACKGROUND TINT
    // -------------------------------------------------

    private void TintBackground(Color c)
    {
        if (backgroundRenderers == null)
            return;

        foreach (var r in backgroundRenderers)
        {
            if (r != null)
                r.color = c;
        }
    }

    // -------------------------------------------------
    // OVERRIDES FROM LEVEL CONFIG
    // -------------------------------------------------

    public void ApplyOverrides(ScreamerSetup setup)
    {
        if (screamerRoot == null || screamerRendererA == null || visualOffsetRoot == null)
            return;

        // 1) Сохраняем дефолты ОДИН РАЗ (как стоит в сцене / Level 1)
        if (!_defaultsSaved)
        {
            _defaultRootPosition = screamerRoot.position;
            _defaultRootScale = screamerRoot.localScale;
            _defaultVisualOffset = visualOffsetRoot.localPosition;

            _defaultSortingLayer = screamerRendererA.sortingLayerName;
            _defaultOrderInLayer = screamerRendererA.sortingOrder;

            _defaultsSaved = true;
        }

        // 2) Сброс к дефолту (чтобы уровни не "залипали")
        screamerRoot.position = _defaultRootPosition;
        screamerRoot.localScale = _defaultRootScale;
        visualOffsetRoot.localPosition = _defaultVisualOffset;

        screamerRendererA.sortingLayerName = _defaultSortingLayer;
        screamerRendererB.sortingLayerName = _defaultSortingLayer;

        screamerRendererA.sortingOrder = _defaultOrderInLayer;
        screamerRendererB.sortingOrder = _defaultOrderInLayer;

        if (setup == null)
            return;

        // ---------- ROOT POSITION ----------
        if (setup.overridePosition)
        {
            Vector3 pos = setup.position;
            pos.z = screamerRoot.position.z;
            screamerRoot.position = pos;
        }

        // ---------- ROOT SCALE ----------
        if (setup.overrideScale)
        {
            screamerRoot.localScale = setup.scale;
        }

        // ---------- VISUAL OFFSET (KEY POINT) ----------
        if (setup.visualOffset != Vector3.zero)
        {
            visualOffsetRoot.localPosition = setup.visualOffset;
        }

        // ---------- SORTING LAYER ----------
        if (setup.overrideSortingLayer && !string.IsNullOrEmpty(setup.sortingLayerName))
        {
            screamerRendererA.sortingLayerName = setup.sortingLayerName;
            screamerRendererB.sortingLayerName = setup.sortingLayerName;
        }

        // ---------- ORDER IN LAYER ----------
        if (setup.overrideOrderInLayer)
        {
            screamerRendererA.sortingOrder = setup.orderInLayer;
            screamerRendererB.sortingOrder = setup.orderInLayer;
        }
    }
}
