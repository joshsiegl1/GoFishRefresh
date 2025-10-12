#region Using Statments
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion
public class Button : ISelectable
{
    private Texture2D texture;
    public Texture2D Texture { get { return texture; } set { texture = value; } }
    public bool IsSelected { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;
    public Rectangle Bounds { get; private set; }
    private Vector2 position;
    public event EventHandler onClick; 
    public Button(Vector2 position)
    {
        this.position = position;
    }
    public void Draw(SpriteBatch spritebatch)
    {
        spritebatch.Draw(texture, position, Color.White);
    }
    public void Select()
    {
        IsSelected = true;
    }
    public void Deselect()
    {
        IsSelected = false;
    }
    private MouseState previousMS; 
    public void UpdateSelection(MouseState MS)
    {
        MS = Mouse.GetState();
        Bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        if (Bounds.Contains(MS.Position))
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