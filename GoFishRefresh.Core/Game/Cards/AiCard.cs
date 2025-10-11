#region Using Statments
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
    public void DrawCard(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, position, Color.White);
    }
}