#region Using Statments
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

public class Hands
{
    public List<Card> TwoOfAKind = new List<Card>();
    public List<Card> ThreeOfAKind = new List<Card>();
    public List<Card> FourOfAKind = new List<Card>();
    public List<Card> FullHouse = new List<Card>();
    public List<Card> Straight = new List<Card>();
    public List<Card> Flush = new List<Card>(); 
    public List<Card> StraightFlush = new List<Card>();
    public List<Card> RoyalFlush = new List<Card>();
    public Hands()
    {
        TwoOfAKind.Add(new Card(Card.Suits.Hearts, Card.Ranks.Ace));
        TwoOfAKind.Add(new Card(Card.Suits.Spades, Card.Ranks.Ace));

        ThreeOfAKind.Add(new Card(Card.Suits.Hearts, Card.Ranks.King));
        ThreeOfAKind.Add(new Card(Card.Suits.Spades, Card.Ranks.King));
        ThreeOfAKind.Add(new Card(Card.Suits.Diamonds, Card.Ranks.King));

        FourOfAKind.Add(new Card(Card.Suits.Hearts, Card.Ranks.Queen));
        FourOfAKind.Add(new Card(Card.Suits.Spades, Card.Ranks.Queen));
        FourOfAKind.Add(new Card(Card.Suits.Diamonds, Card.Ranks.Queen));
        FourOfAKind.Add(new Card(Card.Suits.Clubs, Card.Ranks.Queen));

        FullHouse.Add(new Card(Card.Suits.Hearts, Card.Ranks.Jack));
        FullHouse.Add(new Card(Card.Suits.Spades, Card.Ranks.Jack));
        FullHouse.Add(new Card(Card.Suits.Diamonds, Card.Ranks.Jack));
        FullHouse.Add(new Card(Card.Suits.Hearts, Card.Ranks.Ten));
        FullHouse.Add(new Card(Card.Suits.Spades, Card.Ranks.Ten));

        Straight.Add(new Card(Card.Suits.Hearts, Card.Ranks.Three));
        Straight.Add(new Card(Card.Suits.Spades, Card.Ranks.Four));
        Straight.Add(new Card(Card.Suits.Diamonds, Card.Ranks.Five));
        Straight.Add(new Card(Card.Suits.Clubs, Card.Ranks.Six));
        Straight.Add(new Card(Card.Suits.Hearts, Card.Ranks.Seven));

        Flush.Add(new Card(Card.Suits.Hearts, Card.Ranks.Two));
        Flush.Add(new Card(Card.Suits.Hearts, Card.Ranks.Four));
        Flush.Add(new Card(Card.Suits.Hearts, Card.Ranks.Six));
        Flush.Add(new Card(Card.Suits.Hearts, Card.Ranks.Eight));
        Flush.Add(new Card(Card.Suits.Hearts, Card.Ranks.Ten));

        StraightFlush.Add(new Card(Card.Suits.Spades, Card.Ranks.Five));
        StraightFlush.Add(new Card(Card.Suits.Spades, Card.Ranks.Six));
        StraightFlush.Add(new Card(Card.Suits.Spades, Card.Ranks.Seven));
        StraightFlush.Add(new Card(Card.Suits.Spades, Card.Ranks.Eight));
        StraightFlush.Add(new Card(Card.Suits.Spades, Card.Ranks.Nine));

        RoyalFlush.Add(new Card(Card.Suits.Diamonds, Card.Ranks.Ten));
        RoyalFlush.Add(new Card(Card.Suits.Diamonds, Card.Ranks.Jack));
        RoyalFlush.Add(new Card(Card.Suits.Diamonds, Card.Ranks.Queen));
        RoyalFlush.Add(new Card(Card.Suits.Diamonds, Card.Ranks.King));
        RoyalFlush.Add(new Card(Card.Suits.Diamonds, Card.Ranks.Ace));
    }
}