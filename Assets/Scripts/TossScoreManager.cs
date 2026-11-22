
using UnityEngine;

public class TossScoreManager : MonoBehaviour
{
    [Header("Game Rules")]
    [SerializeField] private int winningScore = 6;

    [SerializeField] private int score = 0;

    private bool gameWon = false;

    [Header("TV Scoreboard")]
    [SerializeField] private ScoreboardUI scoreboardUI;

    private void Start()
    {
        UpdateScoreboard("THROW TO START!");
    }

    public void AddPoints(int points, string cupName)
    {
        if (gameWon || points <= 0)
            return;

        score += points;

        Debug.Log("Score: " + score);

        if (score >= winningScore)
        {
            score = winningScore;
            gameWon = true;

            UpdateScoreboard("YOU WIN!");
        }
        else
        {
            UpdateScoreboard(cupName + " HIT! +" + points);
        }
    }

    private void UpdateScoreboard(string message)
    {
        if (scoreboardUI != null)
        {
            scoreboardUI.RefreshScore(
                score,
                winningScore,
                message
            );
        }
        else
        {
            Debug.LogWarning("Scoreboard UI not assigned!");
        }
    }

    public int Score => score;
}
