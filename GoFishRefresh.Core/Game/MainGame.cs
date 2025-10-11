#region Using Statments
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
#endregion
public class MainGame
{
    private Deck deck;
    private List<PlayerCard> playerHand;
    private List<AiCard> aiHand;
    private const int HandSize = 7;
    public MainGame()
    {
        deck = new Deck();
        deck.Shuffle();
        playerHand = new List<PlayerCard>();
        aiHand = new List<AiCard>();
        Deal();
    }
    // I'm using composition here rather than inheritance for PlayerCard and AiCard
    private void Deal()
    {
        for (int i = 0; i < HandSize; i++)
        {
            Card playerCard = deck.DrawCard();
            PlayerCard pCard = new PlayerCard(playerCard, new Vector2(50 + i * 240, 400));
            playerHand.Add(pCard);
            Card aiCard = deck.DrawCard();
            AiCard aCard = new AiCard(aiCard, new Vector2(50 + i * 240, 50));
            aiHand.Add(aCard);
        }
    }
    public void LoadContent(ContentManager Content)
    {
        foreach (var pCard in playerHand)
        {
            pCard.LoadContent(Content);
        }
        AiCard.Texture = Content.Load<Texture2D>("cardback");
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var pCard in playerHand)
        {
            pCard.DrawCard(spriteBatch);
        }
        foreach (var aCard in aiHand)
        {
            aCard.DrawCard(spriteBatch);
        }
    }
}