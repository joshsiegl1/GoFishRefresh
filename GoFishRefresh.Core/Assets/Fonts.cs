#region Using Statements
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

public static class Fonts
{
    public static SpriteFont MainFont;
    public static void LoadFonts(ContentManager Content)
    {
        MainFont = Content.Load<SpriteFont>("Fonts/Hud");
    }
}