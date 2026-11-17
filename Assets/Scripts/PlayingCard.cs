using UnityEngine;

public enum CardRank
{
    Joker = 0,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14
}

public class PlayingCard : MonoBehaviour
{
    [Header("Card Value")]
    [SerializeField] private CardRank rank;

    public CardRank Rank => rank;
    public int RankValue => (int)rank;
    public bool IsJoker => rank == CardRank.Joker;
}