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
    private const int HandSpacing = 180; 
    private CardSelector cardSelector;
    private MouseState MS;
    string handMatch = "";
    public MainGame(ContentManager Content)
    {
        deck = new Deck();
        deck.Shuffle();
        playerHand = new List<PlayerCard>();
        aiHand = new List<AiCard>();
        selectedCards = new List<Card>();
        cardSelector = new CardSelector();
        Deal(Content);
    }
    
    private void CheckSelection(Card card, ContentManager Content)
    {
        foreach (var aiCard in aiHand)
        {
            if (aiCard.Card.Rank == card.Rank && aiCard.Card.Suit == card.Suit)
            {
                Console.WriteLine($"Match found: Player's {card.Rank} matches AI's {aiCard.Card.Rank}");
                Card newCard = aiCard.Card;
                aiHand.Remove(aiCard);
                PlayerCard playerCard = new PlayerCard(newCard, new Vector2(50 + (playerHand.Count) * HandSpacing, 700));
                playerCard.onSelect += (s, e) =>
                {
                    selectedCards.Add(playerCard.Card);
                    Console.WriteLine("Selected a card: " + playerCard.Card + " Count: " + selectedCards.Count);
                    Console.WriteLine(HandMatcher.IsMatch(selectedCards));
                    handMatch = HandMatcher.ToString(HandMatcher.IsMatch(selectedCards));
                };
                playerCard.onDeselect += (s, e) =>
                {
                    selectedCards.Remove(playerCard.Card);
                    Console.WriteLine("Deselected a card: " + playerCard.Card + " Count: " + selectedCards.Count);
                    Console.WriteLine(HandMatcher.IsMatch(selectedCards));
                    handMatch = HandMatcher.ToString(HandMatcher.IsMatch(selectedCards));
                };
                // Load content for the new player card
                playerCard.LoadContent(Content); 
                playerHand.Add(playerCard);
                return;
            }
        }
    }
    // I'm using composition here rather than inheritance for PlayerCard and AiCard
    private void Deal(ContentManager Content)
    {
        cardSelector.onCardSelected += (s, e) =>
        {
            Card selected = cardSelector.SelectedCard;
            Console.WriteLine($"MainGame detected selected card: {selected.Rank} of {selected.Suit}");
            CheckSelection(selected, Content);
        };
        for (int i = 0; i < HandSize; i++)
        {
            Card playerCard = deck.DrawCard();
            PlayerCard pCard = new PlayerCard(playerCard, new Vector2(50 + i * HandSpacing, 700));
            pCard.onSelect += (s, e) => {
                selectedCards.Add(pCard.Card);
                Console.WriteLine("Selected a card: " + pCard.Card + " Count: " + selectedCards.Count);
                Console.WriteLine(HandMatcher.IsMatch(selectedCards));
                handMatch = HandMatcher.ToString(HandMatcher.IsMatch(selectedCards)); 
            };
            pCard.onDeselect += (s, e) => {
                selectedCards.Remove(pCard.Card);
                Console.WriteLine("Deselected a card: " + pCard.Card + " Count: " + selectedCards.Count);
                Console.WriteLine(HandMatcher.IsMatch(selectedCards)); 
                handMatch = HandMatcher.ToString(HandMatcher.IsMatch(selectedCards));
            };
            playerHand.Add(pCard);
            Card aiCard = deck.DrawCard();
            AiCard aCard = new AiCard(aiCard, new Vector2(50 + i * HandSpacing, 50));
            aiHand.Add(aCard);
        }
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        cardSelector.Update(gameTime, graphics);
        foreach (PlayerCard pCard in playerHand)
        {
            pCard.UpdateSelection(MS, graphics); 
        }
    }
    public void LoadContent(ContentManager Content)
    {
        cardSelector.LoadContent(Content);
        foreach (var pCard in playerHand)
        {
            pCard.LoadContent(Content);
        }
        AiCard.Texture = Content.Load<Texture2D>("cardback");
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        cardSelector.Draw(spriteBatch);
        spriteBatch.DrawString(Fonts.MainFont, "Selected Hand Type: " + handMatch, new Vector2(50, 600), Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, Global.HandsLayerDepth);
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