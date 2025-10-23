#region Using Statments
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
#endregion

public class PlayCardButton : ISelectable
{
    private Texture2D texture; 
    public Texture2D Texture { get { return texture; } set { texture = value; } }
    private Vector2 position;
    public Rectangle Bounds { get; private set; }
    public event EventHandler onClick; 
    public PlayCardButton(Vector2 position)
    {
        this.position = position;
    }
    public void Draw(SpriteBatch spritebatch)
    {
        spritebatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, Global.ShowCardButtonLayerDepth);
    }
    public bool IsSelected { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;
    public void Deselect()
    {
        IsSelected = false;
    }
    public void Select()
    {
        IsSelected = true;
    }
    private MouseState previousMS; 
    public void UpdateSelection(MouseState MS, GraphicsDeviceManager graphics)
    {
        Matrix invMatrix = Matrix.Invert(Global.createTransformMatrix(graphics));
        MS = Mouse.GetState();
        Bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        Vector2 mouseWorld = Vector2.Transform(new Vector2(MS.X, MS.Y), invMatrix);
        if (Bounds.Contains(mouseWorld))
        {
            Mouse.SetCursor(MouseCursor.Hand);
            IsHighlighted = true;
            if (MS.LeftButton == ButtonState.Pressed 
                && previousMS.LeftButton == ButtonState.Released)
            {
                onClick?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            if (IsHighlighted)
                Mouse.SetCursor(MouseCursor.Arrow);
            IsHighlighted = false;
        }
        previousMS = MS;    
    }
}