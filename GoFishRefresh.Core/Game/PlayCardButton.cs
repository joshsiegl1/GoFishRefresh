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
    private HandMatcher.HandType handType; 
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

    // Improved: Use enum instead of string for type safety
    public void SetActive(HandMatcher.HandType handType)
    {
        this.handType = handType;
        
        switch (handType)
        {
            case HandMatcher.HandType.None:
                text = "No Valid Hand";
                IsActive = false;
                break;
            case HandMatcher.HandType.Pair:
                text = "Play Pair";
                IsActive = true;
                break;
            case HandMatcher.HandType.ThreeOfAKind:
                text = "Play Three of a Kind";
                IsActive = true;
                break;
            case HandMatcher.HandType.FourOfAKind:
                text = "Play Four of a Kind";
                IsActive = true;
                break;
            case HandMatcher.HandType.TwoPair:
                text = "Play Two Pair";
                IsActive = true;
                break;
            case HandMatcher.HandType.FullHouse:
                text = "Play Full House";
                IsActive = true;
                break;
            case HandMatcher.HandType.Flush:
                text = "Play Flush";
                IsActive = true;
                break;
            case HandMatcher.HandType.Straight:
                text = "Play Straight";
                IsActive = true;
                break;
            case HandMatcher.HandType.StraightFlush:
                text = "Play Straight Flush";
                IsActive = true;
                break;
            case HandMatcher.HandType.RoyalFlush:
                text = "Play Royal Flush";
                IsActive = true;
                break;
            default:
                text = "No Valid Hand";
                IsActive = false;
                break;
        }
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