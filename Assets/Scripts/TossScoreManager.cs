
using UnityEngine;

public class TossScoreManager : MonoBehaviour
{
    public enum GameState
    {
        Ready,
        Playing,
        Won
    }

    [Header("Game Rules")]
    [SerializeField] private int winningScore = 6;
    [SerializeField] private int score = 0;

    [Header("Game State")]
    [SerializeField]
    private GameState currentState =
        GameState.Ready;

    [Header("Game Objects")]
    [SerializeField] private CupTarget[] cups;
    [SerializeField] private ThrowableBall ball;

    [Header("TV Scoreboard")]
    [SerializeField] private ScoreboardUI scoreboardUI;

    public int Score => score;
    public GameState CurrentState => currentState;
    public bool IsPlaying => currentState == GameState.Playing;

    private void Start()
    {
        score = 0;
        currentState = GameState.Ready;

        UpdateScoreboard("PRESS START TO PLAY!");
    }

    public void StartGame()
    {
        if (currentState != GameState.Ready)
            return;

        currentState = GameState.Playing;

        UpdateScoreboard("GAME STARTED - THROW!");

        Debug.Log("Game Started!");
    }

    public void RestartGame()
    {
        score = 0;

        // Restore all six cups.
        foreach (CupTarget cup in cups)
        {
            if (cup != null)
                cup.ResetCup();
        }

        // Return ball to its starting position.
        if (ball != null)
            ball.ResetToStart();

        currentState = GameState.Playing;

        UpdateScoreboard("NEW GAME - THROW!");

        Debug.Log("Game Restarted!");
    }

    public bool AddPoints(int points, string cupName)
    {
        if (!IsPlaying || points <= 0)
            return false;

        score += points;

        Debug.Log("Score updated: " + score);

        if (score >= winningScore)
        {
            score = winningScore;
            currentState = GameState.Won;

            UpdateScoreboard("YOU WIN!");

            Debug.Log("ALL SIX CUPS CLEARED!");
        }
        else
        {
            UpdateScoreboard(cupName + " HIT! +" + points);
        }

        return true;
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
    }
}
