#region Using Statments
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion
public class PlayerCard : ISelectable
{
    private Card card;
    public Card Card { get { return card; } }
    private Vector2 position;
    public Vector2 Position { get { return position; } set { position = value; } }
    private Texture2D texture;
    public Texture2D Texture { get { return texture; } set { texture = value; } }
    public bool IsSelected { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;
    public Rectangle Bounds { get; set; } 
    public event EventHandler onSelect, onDeselect;
    public PlayerCard(Card card, Vector2 position)
    {
        this.card = card;
        this.position = position;
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, Global.CardScale, SpriteEffects.None, Global.DisplayCardLayerDepth);
        if (IsSelected)
        {
            spriteBatch.Draw(Textures.CardSelected, position, null, Color.White, 0f, Vector2.Zero, Global.CardScale, SpriteEffects.None, Global.DisplayCardLayerDepth + 0.01f);
        }
    }
    public void LoadContent(ContentManager Content)
    {
        // Improved: Use Card.LoadString() instead of duplicating conversion logic
        if (Content == null)
            throw new ArgumentNullException(nameof(Content));
        if (card == null)
            throw new InvalidOperationException("Card is null");
            
        string textureName = card.LoadString();
        texture = Content.Load<Texture2D>(textureName);
    }
    public void Select()
    {
        IsSelected = true;
        onSelect?.Invoke(this, EventArgs.Empty);
    }
    public void Deselect()
    {
        IsSelected = false;
        onDeselect?.Invoke(this, EventArgs.Empty);
    }
    private MouseState previousMS;
    public void UpdateSelection(MouseState MS, GraphicsDeviceManager graphics)
    {
        Matrix invMatrix = Matrix.Invert(Global.createTransformMatrix(graphics));
        MS = Mouse.GetState();
        Bounds = new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width * Global.CardScale), (int)(texture.Height * Global.CardScale));
        Vector2 mouseWorld = Vector2.Transform(new Vector2(MS.X, MS.Y), invMatrix);
        if (Bounds.Contains(mouseWorld))
        {
            IsHighlighted = true;
            Mouse.SetCursor(MouseCursor.Hand);
            if (MS.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released)
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

        previousMS = MS;
    }
}
