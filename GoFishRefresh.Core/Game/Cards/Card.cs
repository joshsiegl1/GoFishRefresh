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

    public string LoadString()
    {
        string rankString = Rank switch
        {
            Ranks.Ace => "A",
            Ranks.Two => "2",
            Ranks.Three => "3",
            Ranks.Four => "4",
            Ranks.Five => "5",
            Ranks.Six => "6",
            Ranks.Seven => "7",
            Ranks.Eight => "8",
            Ranks.Nine => "9",
            Ranks.Ten => "10",
            Ranks.Jack => "J",
            Ranks.Queen => "Q",
            Ranks.King => "K",
            _ => throw new ArgumentOutOfRangeException()
        };
        string suitString = Suit switch
        {
            Suits.Hearts => "H",
            Suits.Diamonds => "D",
            Suits.Clubs => "C",
            Suits.Spades => "S",
            _ => throw new ArgumentOutOfRangeException()
        };
        return $"{rankString}{suitString}";
    }
}