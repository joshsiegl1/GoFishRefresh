#region Using Statments
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

public class Hands
{
    public List<Card> TwoOfAKind = new List<Card>();
    public List<Card> ThreeOfAKind = new List<Card>();
    public List<Card> TwoPair = new List<Card>(); 
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

        TwoPair.Add(new Card(Card.Suits.Hearts, Card.Ranks.Two));
        TwoPair.Add(new Card(Card.Suits.Spades, Card.Ranks.Two));
        TwoPair.Add(new Card(Card.Suits.Hearts, Card.Ranks.Three));
        TwoPair.Add(new Card(Card.Suits.Spades, Card.Ranks.Three));

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

    private void DrawHand(SpriteBatch spriteBatch, string title, List<Card> hand, Vector2 position, float Fade)
    {
        if (hand == null || hand.Count == 0)
            return;

        // Draw the title/label of the hand (similar to PlayedCards.cs)
        if (Fonts.MainFont != null && !string.IsNullOrEmpty(title))
        {
            spriteBatch.DrawString(Fonts.MainFont, title, position, Color.White * Fade, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, Global.HandsLayerDepth);
        }

        // Draw each card in the hand (offset down from label, similar to PlayedCards.cs)
        Vector2 cardPosition = new Vector2(position.X, position.Y + 40);
        foreach (var card in hand)
        {
            Texture2D cardTexture = Textures.GetCardTexture(card);
            if (cardTexture != null)
            {
                spriteBatch.Draw(cardTexture, cardPosition, null, Color.White * Fade, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, Global.HandsLayerDepth);
                cardPosition.X += (cardTexture.Width * 0.5f) + 10; // Move right for the next card
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, float Fade)
    {
        // Layout categories into columns with up to N rows per column so extra categories flow to the right.
        Vector2 start = new Vector2(50, 150);
        const int maxRows = 4;
        const float verticalSpacing = 220f;
        const float columnSpacing = 700f; // distance between columns

        var categories = new List<(string title, List<Card> hand)>
        {
            ("Two of a Kind", TwoOfAKind),
            ("Three of a Kind", ThreeOfAKind),
            ("Two Pair", TwoPair),
            ("Four of a Kind", FourOfAKind),
            ("Full House", FullHouse),
            ("Straight", Straight),
            ("Flush", Flush),
            ("Straight Flush", StraightFlush),
            ("Royal Flush", RoyalFlush),
        };

        for (int i = 0; i < categories.Count; i++)
        {
            int col = i / maxRows;
            int row = i % maxRows;
            Vector2 pos = new Vector2(start.X + col * columnSpacing, start.Y + row * verticalSpacing);
            DrawHand(spriteBatch, categories[i].title, categories[i].hand, pos, Fade);
        }
    }
}