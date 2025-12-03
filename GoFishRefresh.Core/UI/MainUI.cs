#region Using Statments
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
#endregion
namespace GoFishRefresh.Core.UI
{
    public class MainUI
    {
        MouseState MS; 
            MouseState previousMS;
        Button btnShowHands;
        bool showHands = false;
        Hands hands;
        Button btnShowPlayedCards;
        bool showPlayedHands = false; 
        PlayedCards playerPlayedCards;
        PlayedCards aiPlayedCards;
        float Fade = 0f;
        float PlayedCardsFade = 0f;
        private const float FadeSpeed = 0.05f;
        
        public MainUI()
        {
            btnShowHands = new Button(new Vector2(50, 50));
            btnShowHands.onClick += onShowHandsClick;
            playerPlayedCards = new PlayedCards();
            aiPlayedCards = new PlayedCards();
            hands = new Hands(playerPlayedCards, aiPlayedCards);

            btnShowPlayedCards = new Button(new Vector2(250, 50));
            btnShowPlayedCards.onClick += onShowPlayedCardsClick;
        }

        private void onShowPlayedCardsClick(object sender, EventArgs e)
        {
            // If clicking to show played cards, hide hands screen
            if (!showPlayedHands)
            {
                showHands = false; // Hide hands screen
            }
            showPlayedHands = !showPlayedHands;
        }

        private void onShowHandsClick(object sender, EventArgs e)
        {
            // If clicking to show hands, hide played cards screen
            if (!showHands)
            {
                showPlayedHands = false; // Hide played cards screen
            }
            showHands = !showHands;
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            MS = Mouse.GetState();

            // Compute whether the user clicked the overlay areas so we can dismiss them.
            // Use the inverse transform matrix to convert mouse screen coords to virtual world coords.
            Matrix invMatrix = Matrix.Invert(Global.createTransformMatrix(graphics));
            Vector2 mouseWorld = Vector2.Transform(new Vector2(MS.X, MS.Y), invMatrix);

            bool overlayHandled = false;
            // If the played-cards overlay is visible and the user clicked, hide it.
            if (PlayedCardsFade > 0f && MS.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released)
            {
                showPlayedHands = false;
                overlayHandled = true;
            }
            // If the hands overlay is visible and the user clicked, hide it.
            if (!overlayHandled && Fade > 0f && MS.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released)
            {
                showHands = false;
                overlayHandled = true;
            }

            // Only allow the top-level buttons to receive clicks when an overlay didn't just absorb the click.
            if (!overlayHandled)
            {
                btnShowHands.UpdateSelection(MS, graphics);
                btnShowPlayedCards.UpdateSelection(MS, graphics);
            }

            // Update hands screen (handles its own toggle button)
            if (showHands)
            {
                hands.Update(gameTime, graphics);
            }

            // Update fade for hands screen
            if (showHands && Fade < 1f)
            {
                Fade += FadeSpeed;
            }
            else if (!showHands && Fade > 0f)
            {
                Fade -= FadeSpeed;
            }

            // Update fade for played cards screen
            if (showPlayedHands && PlayedCardsFade < 1f)
            {
                PlayedCardsFade += FadeSpeed;
            }
            else if (!showPlayedHands && PlayedCardsFade > 0f)
            {
                PlayedCardsFade -= FadeSpeed;
            }

            previousMS = MS;
        }
        
        public void LoadContent(ContentManager Content)
        {
            btnShowHands.Texture = Textures.ShowHandsButton;
            btnShowPlayedCards.Texture = Textures.ButtonBackground;
            hands.LoadContent(Content);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            btnShowHands.Draw(spriteBatch);
            btnShowPlayedCards.Draw(spriteBatch);
            
            // Draw hands screen
            if (Fade > 0f)
            {
                spriteBatch.Draw(Textures.background, new Rectangle(0, 0, 1920, 1080), null, Color.Black * 0.5f * Fade, 0f,
                    Vector2.Zero, SpriteEffects.None, Global.BackgroundLayerDepth);
                hands.Draw(spriteBatch, Fade);
            }
            
            // Draw played cards screen
            if (PlayedCardsFade > 0f)
            {
                spriteBatch.Draw(Textures.background, new Rectangle(0, 0, 1920, 1080), null, Color.Black * 0.5f * PlayedCardsFade, 0f,
                    Vector2.Zero, SpriteEffects.None, Global.BackgroundLayerDepth);
                playerPlayedCards.Draw(spriteBatch, PlayedCardsFade);
            }
        }
        
        /// <summary>
        /// Get the player's PlayedCards instance to connect to MainGame events
        /// </summary>
        public PlayedCards GetPlayerPlayedCards()
        {
            return playerPlayedCards;
        }

        /// <summary>
        /// Get the AI's PlayedCards instance to connect to MainGame events
        /// </summary>
        public PlayedCards GetAiPlayedCards()
        {
            return aiPlayedCards;
        }
    }    
}