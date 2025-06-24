using UnityEngine;
using UnityEngine.SceneManagement;

public class Common : MonoBehaviour
{
    public string nextSceneName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Quit();
        else
            if (Input.anyKeyDown)
                SceneManager.LoadScene(nextSceneName);
    }

    private void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}