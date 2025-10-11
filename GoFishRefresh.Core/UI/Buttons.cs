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
    public Rectangle Bounds => throw new NotImplementedException();
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
    public void UpdateSelection(MouseState MS)
    {
        throw new NotImplementedException();
    }
}