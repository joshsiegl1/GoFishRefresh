#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion 

public class PlayedCards
{
    public List<PlayedHand> CardsPlayed { get; private set; }

    public PlayedCards()
    {
        CardsPlayed = new List<PlayedHand>();
    }

    public void AddNewHandPlayed(List<Card> hand, HandMatcher.HandType handType = HandMatcher.HandType.None)
    {
        if (hand != null && hand.Count > 0)
        {
            CardsPlayed.Add(new PlayedHand
            {
                Cards = new List<Card>(hand), // Create a copy to avoid reference issues
                HandType = handType
            });
        }
    }
    
    // Helper class to store a played hand with its type
    public class PlayedHand
    {
        public List<Card> Cards { get; set; }
        public HandMatcher.HandType HandType { get; set; }
    }

    /// <summary>
    /// Draw a single played hand with its cards and hand type label
    /// </summary>
    private void DrawHand(SpriteBatch spriteBatch, PlayedHand playedHand, Vector2 position, float fade)
    {
        if (playedHand == null || playedHand.Cards == null || playedHand.Cards.Count == 0)
            return;

        // Draw hand type label if font is available
        if (Fonts.MainFont != null)
        {
            string handTypeText = HandMatcher.ToString(playedHand.HandType);
            if (!string.IsNullOrEmpty(handTypeText) && playedHand.HandType != HandMatcher.HandType.None)
            {
                spriteBatch.DrawString(Fonts.MainFont, handTypeText, position, Color.White * fade, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, Global.HandsLayerDepth);
            }
        }

        // Draw each card in the hand (offset down from label)
        Vector2 cardPosition = new Vector2(position.X, position.Y + 40);
        foreach (var card in playedHand.Cards)
        {
            Texture2D cardTexture = Textures.GetCardTexture(card);
            if (cardTexture != null)
            {
                spriteBatch.Draw(cardTexture, cardPosition, null, Color.White * fade, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, Global.HandsLayerDepth);
                cardPosition.X += (cardTexture.Width * 0.5f) + 10; // Move right for the next card
            }
        }
    }

    /// <summary>
    /// Draw all played hands on the screen
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, float fade)
    {
        if (CardsPlayed == null || CardsPlayed.Count == 0)
            return;

        Vector2 startPosition = new Vector2(50, 200);
        const float verticalSpacing = 200;
        const float rowWidth = 1920 - 100; // Screen width minus margins
        const float screenHeight = 1080; // Virtual screen height
        const float maxY = screenHeight - 100; // Stop drawing before bottom margin

        Vector2 currentPosition = startPosition;
        int handsInCurrentRow = 0;

        foreach (var playedHand in CardsPlayed)
        {
            if (playedHand == null || playedHand.Cards == null || playedHand.Cards.Count == 0)
                continue;

            // Check if we need to wrap to a new row
            float estimatedWidth = playedHand.Cards.Count * 80; // Rough estimate: ~80 pixels per card at 0.5 scale
            if (handsInCurrentRow > 0 && currentPosition.X + estimatedWidth > rowWidth)
            {
                // Move to next row
                currentPosition.X = startPosition.X;
                currentPosition.Y += verticalSpacing;
                handsInCurrentRow = 0;
            }

            // Check if the current position exceeds the screen height - stop drawing if it does
            if (currentPosition.Y > maxY)
            {
                break; // Stop drawing when we exceed the visible area
            }

            // Draw the hand
            DrawHand(spriteBatch, playedHand, currentPosition, fade);

            // Move position for next hand (cards are drawn horizontally, so move down)
            currentPosition.Y += verticalSpacing;
            handsInCurrentRow++;
        }
    }
}
