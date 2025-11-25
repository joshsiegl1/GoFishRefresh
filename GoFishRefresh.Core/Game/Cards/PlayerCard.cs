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
    // Per-card scale (allows shrinking when many cards are present)
    public float Scale { get; set; } = Global.CardScale;
    private Texture2D texture;
    public Texture2D Texture { get { return texture; } set { texture = value; } }
    // Per-card layer depth so selected cards can be drawn above others
    private float layerDepth = Global.DisplayCardLayerDepth;
    public float LayerDepth { get { return layerDepth; } set { layerDepth = value; } }
    public bool IsSelected { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;
    public Rectangle Bounds { get; set; } 
    public event EventHandler onSelect, onDeselect;
    public PlayerCard(Card card, Vector2 position)
    {
        this.card = card;
        this.position = position;
    }
    // Dealing animation state
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float dealElapsed = 0f;
    private float dealDuration = 0.5f;
    private float dealDelay = 0f;
    public bool IsDealing { get; private set; } = false;

    public void StartDeal(Vector2 start, Vector2 target, float duration, float delay = 0f)
    {
        startPosition = start;
        targetPosition = target;
        dealDuration = Math.Max(0.01f, duration);
        dealDelay = Math.Max(0f, delay);
        dealElapsed = 0f;
        position = startPosition;
        IsDealing = true;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsDealing)
            return;

        dealElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (dealElapsed < dealDelay)
            return;

        float t = MathHelper.Clamp((dealElapsed - dealDelay) / dealDuration, 0f, 1f);
        // Simple ease-out interpolation
        float eased = 1f - (1f - t) * (1f - t);
        position = Vector2.Lerp(startPosition, targetPosition, eased);

        if (t >= 1f)
        {
            position = targetPosition;
            IsDealing = false;
        }
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, layerDepth);
        if (IsSelected)
        {
            spriteBatch.Draw(Textures.CardSelected, position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, layerDepth + 0.01f);
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
        // Bring selected card to the top of the display stack
        layerDepth = Global.DisplayCardLayerDepth + 0.05f;
        onSelect?.Invoke(this, EventArgs.Empty);
    }
    public void Deselect()
    {
        IsSelected = false;
        // Reset layer depth when deselected
        layerDepth = Global.DisplayCardLayerDepth;
        onDeselect?.Invoke(this, EventArgs.Empty);
    }
    private MouseState previousMS;

    public bool ContainsPoint(Vector2 worldPoint)
    {
        if (texture == null) return false;
        var rect = new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width * Scale), (int)(texture.Height * Scale));
        return rect.Contains(worldPoint);
    }

    public void UpdateSelection(MouseState MS, GraphicsDeviceManager graphics, bool allowClick)
    {
        if (IsDealing)
            return; // disable selection while dealing
        Matrix invMatrix = Matrix.Invert(Global.createTransformMatrix(graphics));
        MS = Mouse.GetState();
        Bounds = new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width * Scale), (int)(texture.Height * Scale));
        Vector2 mouseWorld = Vector2.Transform(new Vector2(MS.X, MS.Y), invMatrix);
        if (Bounds.Contains(mouseWorld))
        {
            IsHighlighted = true;
            Mouse.SetCursor(MouseCursor.Hand);
            if (allowClick && MS.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released)
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

    // Interface-compatible overload that allows clicks (default behavior)
    public void UpdateSelection(MouseState MS, GraphicsDeviceManager graphics)
    {
        UpdateSelection(MS, graphics, true);
    }

}
