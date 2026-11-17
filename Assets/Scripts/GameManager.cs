
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        WaitingToStart,
        WaitingForCard,
        WaitingForChoice,
        ResolvingRound,
        GameOver
    }

    public enum PlayerChoice
    {
        None,
        Higher,
        Lower
    }

    [Header("Game State")]
    [SerializeField]
    private GameState currentState =
        GameState.WaitingToStart;

    [Header("Player Stats")]
    [SerializeField] private int score = 0;
    [SerializeField] private int streak = 0;
    [SerializeField] private int lives = 3;

    [Header("Current Card")]
    [SerializeField] private PlayingCard currentCard;

    [Header("Deck Reference")]
    [SerializeField] private DeckManager deckManager;

    [Header("Player Prediction")]
    [SerializeField]
    private PlayerChoice playerChoice =
        PlayerChoice.None;

    // Public access to game information
    public GameState CurrentState => currentState;
    public PlayingCard CurrentCard => currentCard;
    public PlayerChoice CurrentChoice => playerChoice;

    public int Score => score;
    public int Streak => streak;
    public int Lives => lives;

    // Called when a new card is drawn
    public void RegisterDrawnCard(PlayingCard card)
    {
        if (card == null)
        {
            Debug.LogError("GameManager: Invalid card!");
            return;
        }

        currentCard = card;
        playerChoice = PlayerChoice.None;
        currentState = GameState.WaitingForChoice;

        Debug.Log("Current card: " + card.name +
                  " | Value: " + card.RankValue);
    }

    // Called when a card enters Higher or Lower socket
    public void RegisterChoice(bool choseHigher, PlayingCard card)
    {
        if (currentState != GameState.WaitingForChoice)
        {
            Debug.LogWarning("Game is not waiting for a choice!");
            return;
        }

        if (card == null || card != currentCard)
        {
            Debug.LogWarning("This isn't the current card!");
            return;
        }

        playerChoice = choseHigher
            ? PlayerChoice.Higher
            : PlayerChoice.Lower;

        currentState = GameState.ResolvingRound;

        Debug.Log("PLAYER CHOSE: " + playerChoice);
    }
}
