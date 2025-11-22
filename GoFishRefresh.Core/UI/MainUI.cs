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
        Button btnShowHands;
        bool showHands = false;
        Hands hands;
        Button btnShowPlayedCards;
        bool showPlayedHands = false; 
        PlayedCards playedCards; 
        float Fade = 0f;
        float PlayedCardsFade = 0f;
        private const float FadeSpeed = 0.05f;
        
        public MainUI()
        {
            btnShowHands = new Button(new Vector2(50, 50));
            btnShowHands.onClick += onShowHandsClick;
            hands = new Hands();

            btnShowPlayedCards = new Button(new Vector2(250, 50));
            btnShowPlayedCards.onClick += onShowPlayedCardsClick;
            playedCards = new PlayedCards();
        }

        private void onShowPlayedCardsClick(object sender, EventArgs e)
        {
            showPlayedHands = !showPlayedHands;
        }

        private void onShowHandsClick(object sender, EventArgs e)
        {
            showHands = !showHands;
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            MS = Mouse.GetState();
            btnShowHands.UpdateSelection(MS, graphics);
            btnShowPlayedCards.UpdateSelection(MS, graphics);
            
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
        }
        
        public void LoadContent(ContentManager Content)
        {
            btnShowHands.Texture = Textures.ShowHandsButton;
            btnShowPlayedCards.Texture = Textures.ButtonBackground;
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
                playedCards.Draw(spriteBatch, PlayedCardsFade);
            }
        }
        
        /// <summary>
        /// Get the PlayedCards instance to connect to MainGame events
        /// </summary>
        public PlayedCards GetPlayedCards()
        {
            return playedCards;
        }
    }    
}