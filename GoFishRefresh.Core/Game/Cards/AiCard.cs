#region Using Statments
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion
public class AiCard
{
    private Card card;
    public Card Card { get { return card; } }
    private Vector2 position;
    public Vector2 Position { get { return position; } set { position = value; } }
    public static Texture2D Texture { get; set; }
    public AiCard(Card card, Vector2 position)
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
        spriteBatch.Draw(Texture, position, null, Color.White, 0f, Vector2.Zero, Global.CardScale, SpriteEffects.None, Global.DisplayCardLayerDepth);
    }
}