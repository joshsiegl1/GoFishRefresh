using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class AnimatedCard
{
    private Card card;
    private Vector2 position;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float elapsed = 0f;
    private float duration = 0.5f;
    private float delay = 0f;
    private bool started = false;
    public bool IsFinished { get; private set; } = false;
    private Texture2D texture;

    public event EventHandler onAnimationComplete;

    public AnimatedCard(Card card, Vector2 start, Vector2 target, float duration = 0.5f, float delay = 0f)
    {
        this.card = card;
        this.startPosition = start;
        this.targetPosition = target;
        this.position = start;
        this.duration = Math.Max(0.01f, duration);
        this.delay = Math.Max(0f, delay);
    }

    public void LoadContent(ContentManager Content)
    {
        if (Content == null) throw new ArgumentNullException(nameof(Content));
        texture = Content.Load<Texture2D>(card.LoadString());
    }

    public void Update(GameTime gameTime)
    {
        if (IsFinished) return;
        elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (elapsed < delay) return;
        float t = MathHelper.Clamp((elapsed - delay) / duration, 0f, 1f);
        float eased = 1f - (1f - t) * (1f - t);
        position = Vector2.Lerp(startPosition, targetPosition, eased);
        if (t >= 1f)
        {
            IsFinished = true;
            onAnimationComplete?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (texture == null || IsFinished) return;
        spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, Global.CardScale, SpriteEffects.None, Global.DisplayCardLayerDepth + 0.02f);
    }
}
