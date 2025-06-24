using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public float score = 0f;
    public Text scoreText;
    public Text highestScoreText;

    void Start()
    {
        float highestScore = PlayerPrefs.GetFloat("HighestScore");
        string formatted = highestScore.ToString("N0");
        highestScoreText.text = "highest score\n" + formatted;
    }

    public void changeNowScore(float addScore)
    {
        score += addScore;
        string formatted = score.ToString("N0");
        scoreText.text = "score\n" + formatted;
        updateScore(score);
    }

    public void hiddenScore() {
        scoreText.enabled = false;
        highestScoreText.enabled = false;
    }

    public void updateScore(float finScore) {
        float highestScore = PlayerPrefs.GetFloat("HighestScore");
        if (finScore > highestScore) PlayerPrefs.SetFloat("HighestScore", finScore);
    }
}
