using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GoFishRefresh.Core.UI
{
    public class MainMenu
    {
        private readonly List<string> entries = new() { "Play", "Test", "Quit" };
        private int selectedIndex = 0;
        private int hoverIndex = -1;

        private MouseState previousMS;
        private KeyboardState previousKS;

        // Events
        public event EventHandler<MenuSelectedEventArgs> OnSelected;

        public class MenuSelectedEventArgs : EventArgs
        {
            public string Selected { get; }
            public MenuSelectedEventArgs(string selected) => Selected = selected;
        }

        public MainMenu()
        {
        }

        public void LoadContent(ContentManager content)
        {
            // placeholder if you need to load menu-specific textures later
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            // Map mouse to world coordinates using the game's transform
            Matrix inv = Matrix.Invert(Global.createTransformMatrix(graphics));
            Vector2 mouseWorld = Vector2.Transform(new Vector2(ms.X, ms.Y), inv);

            // Determine layout: center screen vertically, center horizontally
            float virtualW = 1920f;
            float virtualH = 1080f;
            float scale = 1.5f;
            Vector2 basePos = new Vector2(virtualW / 2f, virtualH / 2f - 50f);

            hoverIndex = -1;
            for (int i = 0; i < entries.Count; i++)
            {
                Vector2 textSize = Fonts.MainFont?.MeasureString(entries[i]) ?? Vector2.Zero;
                Vector2 pos = new Vector2(basePos.X - (textSize.X * scale) / 2f, basePos.Y + i * (textSize.Y * 1.6f * scale));
                Rectangle bounds = new Rectangle((int)pos.X, (int)pos.Y, (int)(textSize.X * scale), (int)(textSize.Y * scale));
                if (bounds.Contains(mouseWorld))
                {
                    hoverIndex = i;
                }
            }

            // Keyboard navigation (detect key press transitions)
            if (ks.IsKeyDown(Keys.Down) && previousKS.IsKeyUp(Keys.Down))
            {
                selectedIndex = (selectedIndex + 1) % entries.Count;
            }
            else if (ks.IsKeyDown(Keys.Up) && previousKS.IsKeyUp(Keys.Up))
            {
                selectedIndex = (selectedIndex - 1 + entries.Count) % entries.Count;
            }
            else if (ks.IsKeyDown(Keys.Enter) && previousKS.IsKeyUp(Keys.Enter))
            {
                FireSelection(selectedIndex);
            }

            // Mouse click selects hovered entry
            if (ms.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released)
            {
                if (hoverIndex >= 0)
                {
                    selectedIndex = hoverIndex;
                    FireSelection(hoverIndex);
                }
            }

            // Mouse hover can also change selection highlight on hover
            if (hoverIndex >= 0)
                selectedIndex = hoverIndex;

            previousMS = ms;
            previousKS = ks;
        }

        private void FireSelection(int index)
        {
            if (index >= 0 && index < entries.Count)
            {
                OnSelected?.Invoke(this, new MenuSelectedEventArgs(entries[index]));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Fonts.MainFont == null)
                return; // nothing to draw without a font

            float scale = 1.5f;
            float virtualW = 1920f;
            float virtualH = 1080f;
            Vector2 basePos = new Vector2(virtualW / 2f, virtualH / 2f - 50f);

            for (int i = 0; i < entries.Count; i++)
            {
                string text = entries[i];
                Vector2 textSize = Fonts.MainFont.MeasureString(text);
                Vector2 pos = new Vector2(basePos.X - (textSize.X * scale) / 2f, basePos.Y + i * (textSize.Y * 1.6f * scale));
                Color color = (i == selectedIndex) ? Color.Yellow : Color.White;
                if (i == hoverIndex)
                    color = Color.LightGreen;

                spriteBatch.DrawString(Fonts.MainFont, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, Global.HandsLayerDepth);
            }
        }
    }
}
