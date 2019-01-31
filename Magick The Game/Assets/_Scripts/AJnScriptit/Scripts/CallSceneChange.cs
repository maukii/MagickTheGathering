using UnityEngine;

public class CallSceneChange : MonoBehaviour
{
    public void ChangeLevel(string levelName)
    {
        GlobalVariables.loadLevel = levelName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
    }
}
