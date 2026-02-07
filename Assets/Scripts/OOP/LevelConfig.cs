using UnityEngine;
using System.Collections.Generic;

// ------------------------------
//  НАСТРОЙКА ПОЗИЦИЙ ДЛЯ СОСТОЯНИЙ УЧИЛКИ
// ------------------------------

// Делаем отдельный тип, который Unity будет красиво показывать в Inspector
[System.Serializable]

public class TeacherStateSetup
{
    [Header("State index")]
    [Tooltip("Индекс состояния (0,1,2,3...)")]
    public int stateIndex;

    // ---------- POSITION ----------
    [Header("Position")]
    public bool overridePosition;
    public Vector3 position;

    // ---------- SCALE ----------
    [Header("Scale")]
    [Tooltip("Если включено — переопределяем масштаб этого состояния")]
    public bool overrideScale;
    public Vector3 scale;

    // ---------- SORTING ----------
    [Header("Sorting")]
    [Tooltip("Если включено — переопределяем sorting layer")]
    public bool overrideSortingLayer;
    public string sortingLayerName;

    [Tooltip("Если включено — переопределяем order in layer")]
    public bool overrideOrderInLayer;
    public int orderInLayer;

    
}

[System.Serializable]

public class ScreamerSetup
{
    [Header("Root Transform")]
    public bool overridePosition;
    public Vector3 position;

    public bool overrideScale;
    public Vector3 scale;

    [Header("Visual Offset (for different compositions)")]
    [Tooltip("Смещение картинки скримера относительно ScreamerRoot")]
    public Vector3 visualOffset;

    [Header("Sorting")]
    public bool overrideSortingLayer;
    public string sortingLayerName;

    public bool overrideOrderInLayer;
    public int orderInLayer;

    [Header("Timing")]
    public float losePanelDelaySeconds = 3f;
}



// ------------------------------
//  LEVEL CONFIG (ScriptableObject)
// ------------------------------

// Это создаёт пункт в Unity:
// Create -> SchoolSurvival -> LevelConfig
[CreateAssetMenu(menuName = "SchoolSurvival/LevelConfig", fileName = "LevelConfig")]
public class LevelConfig : ScriptableObject
{
    // ------------------------------
    //  BACKGROUND
    // ------------------------------
    [Header("Background")]
    [Tooltip("Фон класса для этого уровня. Если null — фон останется как в сцене.")]
    public Sprite backgroundSprite;



    // ------------------------------
    //  TIMER
    // ------------------------------
    [Header("Timer")]
    [Tooltip("Сколько секунд даётся на один вопрос")]
    public float timePerQuestion = 10f;


    // ------------------------------
    //  QUESTIONS
    // ------------------------------
    [Header("Questions")]
    [Tooltip("Список вопросов для уровня (твои QuestionData)")]
    public List<QuestionData> questions = new List<QuestionData>();


    // ------------------------------
    //  TEACHER (SPRITES)
    // ------------------------------
    [Header("Teacher Sprites (0 calm, 1 breathing, 2 shouting)")]
    [Tooltip("Спрайты для состояний училки.\nНапример: 0 = спокойная, 1 = ближе/дышит, 2 = орёт.\nЕсли у тебя 4 состояния — можешь сделать размер 4.")]
    public Sprite[] teacherStateSprites;


    // ------------------------------
    //  TEACHER (PER-STATE POSITIONS)
    // ------------------------------
    [Header("Teacher Positions per State (optional overrides)")]
    [Tooltip(
        "Здесь ты можешь ПЕРЕОПРЕДЕЛЯТЬ позиции отдельных состояний училки.\n" +
        "Если список пустой — все позиции берутся из сцены (как ты расставил в Level 1).\n" +
        "Если добавишь элемент с overridePosition=true — только это состояние переедет в новую позицию."
    )]
    public TeacherStateSetup[] teacherStateSetups;


    // ------------------------------
    //  SCREAMER
    // ------------------------------
    [Header("Screamer Setup")]
    public ScreamerSetup screamerSetup;


    [Header("Screamer Sprites (A/B)")]
    [Tooltip("Первая картинка скримера")]
    public Sprite screamerSpriteA;

    [Tooltip("Вторая картинка скримера (мигание)")]
    public Sprite screamerSpriteB;

    [Header("Screamer Background")]
    [Tooltip("Цвет фона во время скримера (например красный)")]
    public Color screamerBackgroundColor = Color.red;


    // ------------------------------
    //  AUDIO
    // ------------------------------
    [Header("Audio - Level")]
    [Tooltip("Фоновая музыка уровня")]
    public AudioClip backgroundMusic;

    [Tooltip("Звук тиканья таймера (loop в AudioController)")]
    public AudioClip tickSound;

    [Tooltip("Дыхание/напряжение (может играть на 1-й ошибке, если ты так решил)")]
    public AudioClip breathingSound;

    [Tooltip("Крик/предупреждение (например на 2-й ошибке)")]
    public AudioClip warningShoutSound;

    [Tooltip("Звук скримера")]
    public AudioClip screamerSound;

    [Header("Audio - Answer Feedback")]
    [Tooltip("Звук при правильном ответе")]
    public AudioClip correctAnswerSound;

    [Tooltip("Звук при неправильном ответе")]
    public AudioClip wrongAnswerSound;


    [Header("Rules")]
    [Tooltip("Сколько ошибок можно допустить на этом уровне до проигрыша")]
    public int allowedMistakes = 3;
}
