#region Using Statments
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
#endregion

public class PlayCardButton : ISelectable
{
    private string text;
    public bool IsActive { get; set; } = false;
    private int points = 0;
    private HandType handType; 
    private Texture2D texture; 
    public Texture2D Texture { get { return texture; } set { texture = value; } }
    private Vector2 position;
    public Rectangle Bounds { get; private set; }
    public event EventHandler onClick; 
    public PlayCardButton(Vector2 position, string text)
    {
        this.position = position;
        this.text = text;   
    }
    public void Draw(SpriteBatch spritebatch)
    {
        // Can be many seperate buttons here for different hand types
        if (IsActive)
        {
            spritebatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, Global.ShowCardButtonLayerDepth);
            spritebatch.DrawString(Fonts.MainFont, text, new Vector2(position.X + 10, position.Y + 10), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, Global.ShowCardButtonLayerDepth + 0.01f);
        }
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

    public void LoadContent(ContentManager Content)
    {
        texture = Content.Load<Texture2D>("button_background");
    }

    public void SetActive(string HandMatch)
    {
        switch (HandMatch)
        {
            case "No Match":
                handType = HandType.None;
                text = "No Valid Hand";
                IsActive = false;
                break;
            case "Pair":
                handType = HandType.Pair;
                text = "Play Pair";
                IsActive = true;
                break;
            case "Three of a Kind":
                handType = HandType.ThreeOfAKind;
                text = "Play Three of a Kind";
                IsActive = true;
                break;
            case "Four of a Kind":
                handType = HandType.FourOfAKind;
                text = "Play Four of a Kind";
                IsActive = true;
                break;
            case "Two Pair":
                handType = HandType.TwoPair;
                text = "Play Two Pair";
                IsActive = true;
                break;
            case "Full House":
                handType = HandType.FullHouse;
                text = "Play Full House";
                IsActive = true;
                break;
            case "Flush":
                handType = HandType.Flush;
                text = "Play Flush";
                IsActive = true;
                break;
            case "Straight":
                handType = HandType.Straight;
                text = "Play Straight";
                IsActive = true;
                break;
            case "Straight Flush":
                handType = HandType.StraightFlush;
                text = "Play Straight Flush";
                IsActive = true;
                break;
            case "Royal Flush":
                handType = HandType.RoyalFlush;
                text = "Play Royal Flush";
                IsActive = true;
                break;
        }
    }

    public enum HandType
    {
        None,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
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