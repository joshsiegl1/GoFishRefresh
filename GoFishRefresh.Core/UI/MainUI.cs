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
        public MainUI()
        {
            btnShowHands = new Button(Vector2.Zero);
            btnShowHands.onClick += onShowHandsClick;
        }
        private void onShowHandsClick(object sender, EventArgs e)
        {
            showHands = !showHands;
        }
        public void LoadContent(ContentManager Content)
        {
            btnShowHands.Texture = Content.Load<Texture2D>("showHandsButton");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            btnShowHands.Draw(spriteBatch);
        }
    }    
}