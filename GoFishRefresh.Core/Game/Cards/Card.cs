#region Using Statments
using System;
#endregion
public class Card
{
    public enum Suits
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }
    public enum Ranks
    {
        Ace = 1,
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
        King = 13
    }
    public Suits Suit { get; private set; }
    public Ranks Rank { get; private set; }
    public Card(Suits suit, Ranks rank)
    {
        Suit = suit;
        Rank = rank;
    }
    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}