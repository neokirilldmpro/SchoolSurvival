using UnityEngine;

// Управляет состояниями училки (включает/выключает объекты, меняет спрайты,
// и (новое) может менять позицию КАЖДОГО состояния отдельно по LevelConfig).
public class TeacherStateController : MonoBehaviour
{
    [Header("Scene objects (already placed in Hierarchy)")]
    // Массив SpriteRenderer-ов, которые уже стоят в сцене (каждый на своём GameObject).
    // ПОРЯДОК ВАЖЕН: индекс 0/1/2/3 должен совпадать с логикой твоих ошибок.
    [SerializeField] private SpriteRenderer[] stateRenderers;

    // Тут мы будем хранить "дефолтные" позиции состояний (как ты расставил их в сцене).
    // Это нужно, чтобы в новом уровне можно было менять только 1 состояние,
    // а остальные автоматически оставались "как в Level 1".
    private Vector3[] _defaultStatePositions;

    private Vector3[] _defaultPositions;
    private Vector3[] _defaultScales;
    private string[] _defaultSortingLayers;
    private int[] _defaultOrders;


    private void Awake()
    {
        if (stateRenderers == null || stateRenderers.Length == 0)
            return;

        int count = stateRenderers.Length;

        _defaultPositions = new Vector3[count];
        _defaultScales = new Vector3[count];
        _defaultSortingLayers = new string[count];
        _defaultOrders = new int[count];

        for (int i = 0; i < count; i++)
        {
            if (stateRenderers[i] == null)
                continue;

            Transform t = stateRenderers[i].transform;
            SpriteRenderer r = stateRenderers[i];

            _defaultPositions[i] = t.position;
            _defaultScales[i] = t.localScale;
            _defaultSortingLayers[i] = r.sortingLayerName;
            _defaultOrders[i] = r.sortingOrder;
        }
    }


    // Подставить картинки (из LevelConfig)
    public void SetStateSprites(Sprite[] sprites)
    {
        // Проверка: назначены ли renderers в инспекторе
        if (stateRenderers == null || stateRenderers.Length == 0)
        {
            Debug.LogError("TeacherStateController: stateRenderers is not assigned!");
            return;
        }

        // Проверка: пришли ли спрайты из LevelConfig
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("TeacherStateController: sprites array is empty!");
            return;
        }

        // Подставляем спрайты по индексам (сколько есть и там и там)
        int count = Mathf.Min(stateRenderers.Length, sprites.Length);

        for (int i = 0; i < count; i++)
        {
            if (stateRenderers[i] != null)
                stateRenderers[i].sprite = sprites[i];
        }

        // По умолчанию включаем спокойное состояние
        SetState(0);
    }

    // НОВОЕ: применить позиции состояний из LevelConfig.teacherStateSetups
    // setups может быть пустым или null — тогда просто оставляем всё как в сцене.
    /*public void ApplyStatePositions(TeacherStateSetup[] setups)
    {
        // Если renderers не назначены — выходим
        if (stateRenderers == null || stateRenderers.Length == 0)
            return;

        // Если дефолтные позиции ещё не были сохранены (на всякий случай),
        // то сохраним их прямо сейчас.
        if (_defaultStatePositions == null || _defaultStatePositions.Length != stateRenderers.Length)
        {
            _defaultStatePositions = new Vector3[stateRenderers.Length];
            for (int i = 0; i < stateRenderers.Length; i++)
            {
                if (stateRenderers[i] != null)
                    _defaultStatePositions[i] = stateRenderers[i].transform.position;
            }
        }

        // 1) Сначала возвращаем все состояния в дефолт (как в сцене / Level 1).
        // Это даёт тебе поведение "оставить как было", если overridePosition выключен.
        for (int i = 0; i < stateRenderers.Length; i++)
        {
            if (stateRenderers[i] != null)
                stateRenderers[i].transform.position = _defaultStatePositions[i];
        }

        // 2) Если setups не задан — значит ничего не переопределяем.
        if (setups == null || setups.Length == 0)
            return;

        // 3) Применяем только те элементы, где overridePosition = true.
        for (int i = 0; i < setups.Length; i++)
        {
            // Берём одну настройку
            TeacherStateSetup setup = setups[i];

            // На всякий случай защита от null
            if (setup == null)
                continue;

            // Если не хотим переопределять позицию — пропускаем
            if (!setup.overridePosition)
                continue;

            // Берём индекс состояния (0..N-1)
            int idx = setup.stateIndex;

            // Проверяем границы
            if (idx < 0 || idx >= stateRenderers.Length)
                continue;

            // Если renderer существует — задаём позицию
            if (stateRenderers[idx] != null)
            {
                Vector3 pos = setup.position;
                pos.z = stateRenderers[idx].transform.position.z; // сохраняем текущий Z (для 2D удобно)
                stateRenderers[idx].transform.position = pos;
            }
        }
    }*/
    public void ApplyStateOverrides(TeacherStateSetup[] setups)
    {
        if (stateRenderers == null || stateRenderers.Length == 0)
            return;

        // 1) Сброс ВСЕХ состояний к дефолту (как в сцене / Level 1)
        for (int i = 0; i < stateRenderers.Length; i++)
        {
            if (stateRenderers[i] == null)
                continue;

            Transform t = stateRenderers[i].transform;
            SpriteRenderer r = stateRenderers[i];

            t.position = _defaultPositions[i];
            t.localScale = _defaultScales[i];
            r.sortingLayerName = _defaultSortingLayers[i];
            r.sortingOrder = _defaultOrders[i];
        }

        // 2) Применяем overrides из LevelConfig
        if (setups == null)
            return;

        foreach (var s in setups)
        {
            if (s == null)
                continue;

            int idx = s.stateIndex;
            if (idx < 0 || idx >= stateRenderers.Length)
                continue;

            var renderer = stateRenderers[idx];
            if (renderer == null)
                continue;

            Transform t = renderer.transform;

            // Position
            if (s.overridePosition)
            {
                Vector3 pos = s.position;
                pos.z = t.position.z;
                t.position = pos;
            }

            // Scale
            if (s.overrideScale)
            {
                t.localScale = s.scale;
            }

            // Sorting Layer
            if (s.overrideSortingLayer && !string.IsNullOrEmpty(s.sortingLayerName))
            {
                renderer.sortingLayerName = s.sortingLayerName;
            }

            // Order in Layer
            if (s.overrideOrderInLayer)
            {
                renderer.sortingOrder = s.orderInLayer;
            }
        }
    }



    // Включить нужное состояние по индексу
    public void SetState(int index)
    {
        if (stateRenderers == null)
            return;

        for (int i = 0; i < stateRenderers.Length; i++)
        {
            if (stateRenderers[i] != null)
                stateRenderers[i].gameObject.SetActive(i == index);
        }
    }

    // Спрятать все состояния училки (например, при скримере)
    public void HideAll()
    {
        if (stateRenderers == null)
            return;

        for (int i = 0; i < stateRenderers.Length; i++)
        {
            if (stateRenderers[i] != null)
                stateRenderers[i].gameObject.SetActive(false);
        }
    }
}
