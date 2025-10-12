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
        CardSelector cardSelector; 
        Hands hands; 
        public MainUI()
        {
            btnShowHands = new Button(Vector2.Zero);
            btnShowHands.onClick += onShowHandsClick;
            cardSelector = new CardSelector();
            hands = new Hands();
        }
        private void onShowHandsClick(object sender, EventArgs e)
        {
            showHands = !showHands;
        }

        public void Update(GameTime gameTime)
        {
            btnShowHands.UpdateSelection(MS);
            cardSelector.Update(gameTime);
        }
        public void LoadContent(ContentManager Content)
        {
            btnShowHands.Texture = Textures.ShowHandsButton;
            cardSelector.LoadContent(Content);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            btnShowHands.Draw(spriteBatch);
            cardSelector.Draw(spriteBatch);
            if (showHands)
            {
                hands.Draw(spriteBatch);
            }
        }
    }    
}