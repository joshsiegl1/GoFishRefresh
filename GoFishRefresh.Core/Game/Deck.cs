#region Using Statments
using System;
using System.Collections.Generic;
#endregion 
public class Deck
{
    private List<Card> cards;
    public List<Card> Cards { get { return cards; } }
    private Random random;
    public Deck()
    {
        cards = new List<Card>();
        random = new Random();
        foreach (Card.Suits suit in Enum.GetValues(typeof(Card.Suits)))
        {
            foreach (Card.Ranks rank in Enum.GetValues(typeof(Card.Ranks)))
            {
                cards.Add(new Card(suit, rank));
            }
        }
    }
    public void Shuffle()
    {
        int n = cards.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            Card temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }
    // Draw a card from the top of the deck
    // This would be a good spot to end the game if the deck is empty
    public Card DrawCard()
    {
        if (cards.Count == 0)
        {
            throw new InvalidOperationException("The deck is empty.");
        }
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}