#region Using Statments
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
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

    private PlayedCards playerPlayedCards;
    private PlayedCards aiPlayedCards;
    private bool showingPlayerCards = true;
    private Button toggleButton;
    private MouseState previousMS;

    public Hands(PlayedCards playerCards, PlayedCards aiCards)
    {
        playerPlayedCards = playerCards;
        aiPlayedCards = aiCards;
        toggleButton = new Button(new Vector2(1500, 50));
        toggleButton.onClick += OnToggleClick;

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

    private void OnToggleClick(object sender, EventArgs e)
    {
        showingPlayerCards = !showingPlayerCards;
    }

    public void LoadContent(ContentManager Content)
    {
        if (toggleButton.Texture == null && Textures.ButtonBackground != null)
        {
            toggleButton.Texture = Textures.ButtonBackground;
        }
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        MouseState currentMS = Mouse.GetState();
        toggleButton.UpdateSelection(currentMS, graphics);
        previousMS = currentMS;
    }

    private void DrawHand(SpriteBatch spriteBatch, string title, List<Card> hand, Vector2 position, float Fade, int pointValue)
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
        float rightMostX = cardPosition.X;
        foreach (var card in hand)
        {
            Texture2D cardTexture = Textures.GetCardTexture(card);
            if (cardTexture != null)
            {
                spriteBatch.Draw(cardTexture, cardPosition, null, Color.White * Fade, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, Global.HandsLayerDepth);
                cardPosition.X += (cardTexture.Width * 0.5f) + 10; // Move right for the next card
                rightMostX = cardPosition.X;
            }
        }

        // Draw point value to the right of cards in larger font
        if (Fonts.MainFont != null && pointValue > 0)
        {
            string pointText = $"+({pointValue})";
            Vector2 pointPosition = new Vector2(rightMostX + 20, position.Y + 80);
            spriteBatch.DrawString(Fonts.MainFont, pointText, pointPosition, Color.Gold * Fade, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, Global.HandsLayerDepth);
        }
    }

    public void Draw(SpriteBatch spriteBatch, float Fade)
    {
        // Draw title showing which cards are being displayed
        if (Fonts.MainFont != null)
        {
            string titleText = showingPlayerCards ? "Player's Played Hands" : "AI's Played Hands";
            Vector2 titlePos = new Vector2(50, 100);
            spriteBatch.DrawString(Fonts.MainFont, titleText, titlePos, Color.Gold * Fade, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, Global.HandsLayerDepth);
        }

        // Draw toggle button in top right corner
        if (toggleButton.Texture != null)
        {
            // Create a rectangle for the button background with fade
            Vector2 buttonPos = new Vector2(1500, 50);
            Rectangle buttonRect = new Rectangle((int)buttonPos.X, (int)buttonPos.Y, 350, 80);
            spriteBatch.Draw(toggleButton.Texture, buttonRect, null, Color.White * Fade, 0f, Vector2.Zero, SpriteEffects.None, Global.HandsLayerDepth);
            
            // Draw toggle button label centered on button
            if (Fonts.MainFont != null)
            {
                string toggleText = showingPlayerCards ? "Show AI Hands" : "Show Player Hands";
                Vector2 textSize = Fonts.MainFont.MeasureString(toggleText) * 1.2f;
                Vector2 labelPos = new Vector2(buttonPos.X + (350 - textSize.X) / 2, buttonPos.Y + (80 - textSize.Y) / 2);
                spriteBatch.DrawString(Fonts.MainFont, toggleText, labelPos, Color.Black * Fade, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, Global.HandsLayerDepth + 0.01f);
            }
        }

        // Get the appropriate PlayedCards instance
        PlayedCards cardsToShow = showingPlayerCards ? playerPlayedCards : aiPlayedCards;
        
        // Draw played cards if any exist
        if (cardsToShow != null && cardsToShow.CardsPlayed != null && cardsToShow.CardsPlayed.Count > 0)
        {
            DrawPlayedHands(spriteBatch, cardsToShow, Fade);
        }
        else
        {
            // Draw example hands reference if no played cards
            DrawExampleHands(spriteBatch, Fade);
        }
    }

    private void DrawPlayedHands(SpriteBatch spriteBatch, PlayedCards playedCards, float Fade)
    {
        Vector2 start = new Vector2(50, 200);
        const float verticalSpacing = 200f;
        const float maxY = 1000f;

        Vector2 currentPosition = start;
        foreach (var playedHand in playedCards.CardsPlayed)
        {
            if (currentPosition.Y > maxY)
                break;

            if (playedHand != null && playedHand.Cards != null && playedHand.Cards.Count > 0)
            {
                string handTypeText = HandMatcher.ToString(playedHand.HandType);
                int pointValue = GetPointValue(playedHand.HandType);
                DrawHand(spriteBatch, handTypeText, playedHand.Cards, currentPosition, Fade, pointValue);
                currentPosition.Y += verticalSpacing;
            }
        }
    }

    private void DrawExampleHands(SpriteBatch spriteBatch, float Fade)
    {
        // Layout categories into columns with up to N rows per column so extra categories flow to the right.
        Vector2 start = new Vector2(50, 150);
        const int maxRows = 4;
        const float verticalSpacing = 220f;
        const float columnSpacing = 700f; // distance between columns

        var categories = new List<(string title, List<Card> hand, int points)>
        {
            ("Two of a Kind", TwoOfAKind, 1),
            ("Three of a Kind", ThreeOfAKind, 3),
            ("Two Pair", TwoPair, 2),
            ("Four of a Kind", FourOfAKind, 10),
            ("Full House", FullHouse, 8),
            ("Straight", Straight, 5),
            ("Flush", Flush, 6),
            ("Straight Flush", StraightFlush, 15),
            ("Royal Flush", RoyalFlush, 25),
        };

        for (int i = 0; i < categories.Count; i++)
        {
            int col = i / maxRows;
            int row = i % maxRows;
            Vector2 pos = new Vector2(start.X + col * columnSpacing, start.Y + row * verticalSpacing);
            DrawHand(spriteBatch, categories[i].title, categories[i].hand, pos, Fade, categories[i].points);
        }
    }

    private int GetPointValue(HandMatcher.HandType handType)
    {
        return handType switch
        {
            HandMatcher.HandType.Pair => 1,
            HandMatcher.HandType.TwoPair => 2,
            HandMatcher.HandType.ThreeOfAKind => 3,
            HandMatcher.HandType.Straight => 5,
            HandMatcher.HandType.Flush => 6,
            HandMatcher.HandType.FullHouse => 8,
            HandMatcher.HandType.FourOfAKind => 10,
            HandMatcher.HandType.StraightFlush => 15,
            HandMatcher.HandType.RoyalFlush => 25,
            _ => 0
        };
    }
}