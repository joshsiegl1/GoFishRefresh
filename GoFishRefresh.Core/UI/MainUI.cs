#region Using Statments
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion
namespace GoFishRefresh.Core.UI
{
    public class MainUI
    {
        Button btnShowHands;
        bool showHands = false;
        CardSelector cardSelector; 
        public MainUI()
        {
            btnShowHands = new Button(Vector2.Zero);
            btnShowHands.onClick += onShowHandsClick;
            cardSelector = new CardSelector();
        }
        private void onShowHandsClick(object sender, EventArgs e)
        {
            showHands = !showHands;
        }

        public void Update(GameTime gameTime)
        {
            //btnShowHands.Update(gameTime);
            cardSelector.Update(gameTime);
        }
        public void LoadContent(ContentManager Content)
        {
            //btnShowHands.Texture = Content.Load<Texture2D>("showHandsButton");
            cardSelector.LoadContent(Content);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //btnShowHands.Draw(spriteBatch);
            cardSelector.Draw(spriteBatch);
        }
    }    
}