using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void Start()
    {
        if (Time.timeScale != 1.0f)
        {
            Time.timeScale = 1.0f;
        }

        if (!string.IsNullOrWhiteSpace(GlobalVariables.loadLevel))
        {
            StartCoroutine(LoadLevelAsync(GlobalVariables.loadLevel));
        }
        else
        {
            Debug.LogError("Empty scene name detected! Loading main menu instead...");
            StartCoroutine(LoadLevelAsync("MainMenu"));
        }
    }
    
    IEnumerator LoadLevelAsync(string levelName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
