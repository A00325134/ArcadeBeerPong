
using UnityEngine;
using TMPro;

public class ScoreboardUI : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Status Display")]
    [SerializeField] private TMP_Text statusText;

    public void RefreshScore(
        int currentScore,
        int targetScore,
        string message)
    {
        if (scoreText != null)
        {
            scoreText.text =
                "SCORE: " + currentScore +
                " / " + targetScore;
        }

        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
