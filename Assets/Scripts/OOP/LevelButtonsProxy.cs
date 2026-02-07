using UnityEngine;

// Ётот скрипт вешаетс€ на ResultPanel (или любой UI-объект в сцене)
// и служит "прокладкой" дл€ кнопок.
//  нопки вызывают Ё“ќ“ скрипт, а он уже вызывает LevelManager.Instance.
public class LevelButtonsProxy : MonoBehaviour
{
    public void Restart()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelButtonsProxy: LevelManager.Instance is NULL");
            return;
        }

        //Debug.Log("LevelButtonsProxy: Restart clicked");
        LevelManager.Instance.RestartCurrentLevel();
    }

    public void NextLevel()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelButtonsProxy: LevelManager.Instance is NULL");
            return;
        }

        //Debug.Log("LevelButtonsProxy: NextLevel clicked");
        LevelManager.Instance.ReloadGameScene();
    }
}
