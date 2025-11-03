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
            btnShowHands.UpdateSelection(MS, graphics);
            if (showHands && Fade < 1f)
            {
                Fade += 0.05f;
            }
            else if (!showHands && Fade > 0f)
            {
                Fade -= 0.05f;
            }

        }
        public void LoadContent(ContentManager Content)
        {
            btnShowHands.Texture = Textures.ShowHandsButton;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            btnShowHands.Draw(spriteBatch);
            spriteBatch.Draw(Textures.background, new Rectangle(0, 0, 1920, 1080), null, Color.Black * 0.5f * Fade, 0f,
                Vector2.Zero, SpriteEffects.None, Global.BackgroundLayerDepth);
            hands.Draw(spriteBatch, Fade);
        }
    }    
}