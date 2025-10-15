#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

public class DrawableCard
{
    private Card card;
    public Card Card { get { return card; } }
    private Vector2 position;
    public Vector2 Position { get { return position; } set { position = value; } }
    private Texture2D texture;
    public Texture2D Texture { get { return texture; } set { texture = value; } }
    public DrawableCard(Card card, Vector2 position)
    {
        this.card = card;
        this.position = position;
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
            _ => throw new ArgumentOutOfRangeException()
        };
        string suitString = card.Suit switch
        {
            Card.Suits.Hearts => "H",
            Card.Suits.Diamonds => "D",
            Card.Suits.Clubs => "C",
            Card.Suits.Spades => "S",
            _ => throw new ArgumentOutOfRangeException()
        };
        string textureName = $"{rankString}{suitString}";
        texture = Content.Load<Texture2D>(textureName);
    }
    public void DrawCard(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, Global.DisplayCardLayerDepth);
    }
}