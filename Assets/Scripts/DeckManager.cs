
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Deck References")]
    [SerializeField] private Transform cardsParent;
    [SerializeField] private Transform cardSpawnPoint;
    [SerializeField] private GameManager gameManager;

    private List<GameObject> availableCards =
        new List<GameObject>();

    private void Awake()
    {
        if (cardsParent == null || cardSpawnPoint == null)
        {
            Debug.LogError("DeckManager: Missing references!");
            enabled = false;
            return;
        }

        availableCards.Clear();

        // Collect all 53 cards, including inactive ones.
        for (int i = 0; i < cardsParent.childCount; i++)
        {
            GameObject card = cardsParent.GetChild(i).gameObject;

            card.SetActive(false);

            PlayingCard cardData = card.GetComponent<PlayingCard>();

            if (cardData == null)
            {
                Debug.LogWarning(card.name +
                    " is missing PlayingCard!");
                continue;
            }

            // Include Joker (value 0).
            availableCards.Add(card);
        }

        Debug.Log("Deck loaded: " +
            availableCards.Count + " cards.");
    }

    // Initial draw, triggered by the player selecting the deck.
    public void DrawRandomCard()
    {
        if (gameManager == null)
        {
            Debug.LogError("DeckManager: GameManager is missing!");
            return;
        }

        PlayingCard card = DrawNextCard();

        if (card != null)
        {
            gameManager.RegisterDrawnCard(card);
        }
    }

    // Draws the next random card and returns it to GameManager.
    public PlayingCard DrawNextCard()
    {
        if (availableCards.Count == 0)
        {
            Debug.Log("The deck is empty!");
            return null;
        }

        int randomIndex = Random.Range(
            0, availableCards.Count
        );

        GameObject drawnCard = availableCards[randomIndex];

        // Remove card so it cannot be drawn again.
        availableCards.RemoveAt(randomIndex);

        // Move card to the spawn point.
        drawnCard.transform.SetPositionAndRotation(
            cardSpawnPoint.position,
            cardSpawnPoint.rotation
        );

        // Activate the card.
        drawnCard.SetActive(true);

        PlayingCard cardData =
            drawnCard.GetComponent<PlayingCard>();

        Debug.Log("Next card drawn: " + drawnCard.name +
            " | Value: " + cardData.RankValue +
            " | Remaining: " + availableCards.Count);

        return cardData;
    }
}
