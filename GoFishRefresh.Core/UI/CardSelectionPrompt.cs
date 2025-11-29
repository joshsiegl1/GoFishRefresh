using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GoFishRefresh.Core.UI
{
    public class CardSelectionPrompt
    {
        private CardSelector cardSelector;
        private List<Card> selectedCards = new List<Card>();
        private class CardComparer : IEqualityComparer<Card>
        {
            public bool Equals(Card x, Card y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;
                return x.Rank == y.Rank && x.Suit == y.Suit;
            }
            public int GetHashCode(Card obj) => HashCode.Combine(obj?.Rank, obj?.Suit);
        }
        private static readonly CardComparer comparer = new CardComparer();
        private int maxCards;
        public bool IsDone { get; private set; } = false;
        public IReadOnlyList<Card> SelectedCards => selectedCards;
        public event EventHandler OnConfirm;
        public event EventHandler OnBack;
        private Rectangle backButtonBounds = new Rectangle(100, 40, 180, 60);

        public CardSelectionPrompt(int maxCards = 7)
        {
            this.maxCards = maxCards;
            cardSelector = new CardSelector();
            cardSelector.onCardSelected += (s, e) =>
            {
                var card = cardSelector.SelectedCard;
                if (card == null) return;
                int idx = selectedCards.FindIndex(c => comparer.Equals(c, card));
                if (idx >= 0)
                {
                    // Toggle off if already selected
                    selectedCards.RemoveAt(idx);
                }
                else if (selectedCards.Count < maxCards)
                {
                    selectedCards.Add(card);
                }
            };
        }

        public void LoadContent(ContentManager content)
        {
            cardSelector.LoadContent(content);
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            cardSelector.Update(gameTime, graphics);
            // Confirm with Enter key
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (selectedCards.Count > 0)
                {
                    IsDone = true;
                    OnConfirm?.Invoke(this, EventArgs.Empty);
                }
            }

            // Back button (Esc key or click)
            MouseState ms = Mouse.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                OnBack?.Invoke(this, EventArgs.Empty);
            }
            // Mouse click on back button
            Matrix inv = Matrix.Invert(Global.createTransformMatrix(graphics));
            Vector2 mouseWorld = Vector2.Transform(new Vector2(ms.X, ms.Y), inv);
            if (ms.LeftButton == ButtonState.Pressed && backButtonBounds.Contains(mouseWorld))
            {
                OnBack?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw prompt text
            if (Fonts.MainFont != null)
            {
                string prompt = $"Select up to {maxCards} cards for your starting hand. Scroll to browse. Press Enter to confirm.";
                spriteBatch.DrawString(Fonts.MainFont, prompt, new Vector2(100, 100), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, Global.HandsLayerDepth);
                string count = $"Selected: {selectedCards.Count}/{maxCards}";
                spriteBatch.DrawString(Fonts.MainFont, count, new Vector2(100, 150), Color.Yellow, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, Global.HandsLayerDepth);

                // Draw back button
                spriteBatch.DrawString(Fonts.MainFont, "< Back", new Vector2(backButtonBounds.X, backButtonBounds.Y), Color.LightBlue, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, Global.HandsLayerDepth);
            }
            cardSelector.Draw(spriteBatch);
            // Optionally, draw selected cards somewhere
            float x = 100, y = 220;
            foreach (var card in selectedCards)
            {
                var tex = Textures.GetCardTexture(card);
                if (tex != null)
                {
                    spriteBatch.Draw(tex, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, Global.DisplayCardLayerDepth);
                    x += tex.Width * 0.7f + 10;
                }
            }
        }
    }
}
