#region Using Statments
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;
#endregion
public class MainGame
{
    private Deck deck;
    private List<PlayerCard> playerHand;
    private List<AiCard> aiHand;
    private List<Card> selectedCards;
    private const int HandSize = 7;
    private MouseState MS; 
    public MainGame()
    {
        deck = new Deck();
        deck.Shuffle();
        playerHand = new List<PlayerCard>();
        aiHand = new List<AiCard>();
        selectedCards = new List<Card>();
        Deal();
    }
    // I'm using composition here rather than inheritance for PlayerCard and AiCard
    private void Deal()
    {
        for (int i = 0; i < HandSize; i++)
        {
            Card playerCard = deck.DrawCard();
            PlayerCard pCard = new PlayerCard(playerCard, new Vector2(50 + i * 240, 700));
            pCard.onSelect += (s, e) => {
                selectedCards.Add(pCard.Card);
                Console.WriteLine("Selected a card: " + pCard.Card + " Count: " + selectedCards.Count);
                Console.WriteLine(HandMatcher.IsMatch(selectedCards)); 
            };
            pCard.onDeselect += (s, e) => {
                selectedCards.Remove(pCard.Card);
                Console.WriteLine("Deselected a card: " + pCard.Card + " Count: " + selectedCards.Count); 
                Console.WriteLine(HandMatcher.IsMatch(selectedCards)); 
            };
            playerHand.Add(pCard);
            Card aiCard = deck.DrawCard();
            AiCard aCard = new AiCard(aiCard, new Vector2(50 + i * 240, 50));
            aiHand.Add(aCard);
        }
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        foreach (PlayerCard pCard in playerHand)
        {
            pCard.UpdateSelection(MS, graphics); 
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
            pCard.Draw(spriteBatch);
        }
        foreach (var aCard in aiHand)
        {
            aCard.Draw(spriteBatch);
        }
    }
}