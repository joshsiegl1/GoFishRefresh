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

    private void DrawHand(SpriteBatch spriteBatch, string title, List<Card> hand, Vector2 position, float Fade)
    {
        // Draw the title of the hand
        //spriteBatch.DrawString(GameAssets.Font, title, position, Color.White);

        // Draw each card in the hand
        foreach (var card in hand)
        {
            Texture2D cardTexture = Textures.GetCardTexture(card);
            spriteBatch.Draw(cardTexture, position, null, Color.White * Fade, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, Global.HandsLayerDepth);
            position.X += (cardTexture.Width / 2) + 10; // Move right for the next card
        }

        position.X = 50; // Reset X position for the next hand
    }

    public void Draw(SpriteBatch spriteBatch, float Fade)
    {
        Vector2 position = new Vector2(50, 50);

        DrawHand(spriteBatch, "Two of a Kind", TwoOfAKind, position, Fade);
        position.Y += 200;
        DrawHand(spriteBatch, "Three of a Kind", ThreeOfAKind, position, Fade);
        position.Y += 200;
        DrawHand(spriteBatch, "Four of a Kind", FourOfAKind, position, Fade);
        position.Y += 200;
        DrawHand(spriteBatch, "Full House", FullHouse, position, Fade);
        position = new Vector2(1110, 50); 
        DrawHand(spriteBatch, "Straight", Straight, position, Fade);
        position.Y += 200;
        DrawHand(spriteBatch, "Flush", Flush, position, Fade);
        position.Y += 200;
        DrawHand(spriteBatch, "Straight Flush", StraightFlush, position, Fade);
        position.Y += 200;
        DrawHand(spriteBatch, "Royal Flush", RoyalFlush, position, Fade);
    }
}