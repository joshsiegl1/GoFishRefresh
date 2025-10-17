#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion
public class SelectableCard : ISelectable
{
    private Card card;
    public Card Card { get { return card; } }
    public bool IsSelected { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;
    public Rectangle Bounds => new Rectangle((int)deckPosition.X, (int)deckPosition.Y, texture.Width, texture.Height); 
    private Texture2D texture;
    private Vector2 deckPosition;
    public SelectableCard(Card card, Vector2 deckPosition)
    {
        this.card = card;
        this.deckPosition = deckPosition;
    }
    public void LoadContent(ContentManager Content)
    {
        string rankString = card.Rank switch
        {
            Card.Ranks.Ace => "A",
            Card.Ranks.Two => "2",
            Card.Ranks.Three => "3",
            Card.Ranks.Four => "4",
            Card.Ranks.Five => "5",
            Card.Ranks.Six => "6",
            Card.Ranks.Seven => "7",
            Card.Ranks.Eight => "8",
            Card.Ranks.Nine => "9",
            Card.Ranks.Ten => "10",
            Card.Ranks.Jack => "J",
            Card.Ranks.Queen => "Q",
            Card.Ranks.King => "K",
            _ => throw new System.ArgumentOutOfRangeException()
        };
        string suitString = card.Suit switch
        {
            Card.Suits.Hearts => "H",
            Card.Suits.Diamonds => "D",
            Card.Suits.Clubs => "C",
            Card.Suits.Spades => "S",
            _ => throw new System.ArgumentOutOfRangeException()
        };
        string textureName = $"{rankString}{suitString}";
        texture = Content.Load<Texture2D>($"{textureName}");
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, deckPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, Global.CardSelectorLayerDepth);
    }
    public void Select()
    {
        IsSelected = true;
    }
    public void Deselect()
    {
        IsSelected = false; 
    }
    public void UpdateSelection(MouseState MS, GraphicsDeviceManager graphics)
    {
        if (Bounds.Contains(MS.Position))
        {
            IsHighlighted = true;
            Mouse.SetCursor(MouseCursor.Hand);
            if (MS.LeftButton == ButtonState.Pressed)
            {
                if (!IsSelected)
                    Select();
                else
                    Deselect();
            }
        }
        else if (IsHighlighted)
        {
            IsHighlighted = false;
            Mouse.SetCursor(MouseCursor.Arrow);
        }
    }
}