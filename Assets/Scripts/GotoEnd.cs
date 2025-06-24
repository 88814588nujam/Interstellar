using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GotoEnd : MonoBehaviour
{
    public string nextSceneName;
    public bool isWin = false;
    public GameObject winPrefab;
    public GameObject failedPrefab;

    public void NextScene(bool flag)
    {
        isWin = flag;
        StartCoroutine(ExecuteMethods());
    }

    IEnumerator ExecuteMethods()
    {
        yield return new WaitForSeconds(2f);
        DisplayFailed();
        yield return new WaitForSeconds(4f);
        GameOver();
    }

    void DisplayFailed()
    {
        Camera mainCamera = Camera.main;
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
        Vector3 worldCenter = mainCamera.ViewportToWorldPoint(screenCenter);
        worldCenter.z = 0;
        Instantiate(isWin ? winPrefab : failedPrefab, worldCenter, Quaternion.identity);
    }

    void GameOver()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
