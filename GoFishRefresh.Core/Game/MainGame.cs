#region Using Statements
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
    private List<Card> playedCards; 
    private const int HandSize = 7;
    private const int HandSpacing = 180;
    private const int PlayerHandY = 700;
    private const int AiHandY = 50;
    private const int HandStartX = 50;
    private const int PlayButtonY = 600;
    private CardSelector cardSelector;
    private PlayCardButton playCardButton;
    private MouseState MS;
    private HandMatcher.HandType currentHandType = HandMatcher.HandType.None;
    public event EventHandler onHandPlayed; 
    public MainGame(ContentManager Content)
    {
        deck = new Deck();
        deck.Shuffle();
        playerHand = new List<PlayerCard>();
        aiHand = new List<AiCard>();
        selectedCards = new List<Card>();
        playedCards = new List<Card>(); // Fixed: Initialize playedCards
        cardSelector = new CardSelector();
        playCardButton = new PlayCardButton(new Vector2(HandStartX, PlayButtonY), "Play Selected Cards");
        playCardButton.onClick += OnPlayCardButtonClicked;
        Deal(Content);
    }

    private void OnPlayCardButtonClicked(object sender, EventArgs e)
    {
        if (selectedCards.Count == 0 || currentHandType == HandMatcher.HandType.None)
            return;

        // Create a copy of selected cards for the played cards list
        var cardsToPlay = new List<Card>(selectedCards);
        playedCards.AddRange(cardsToPlay);

        // Efficiently remove cards from player hand using HashSet
        var cardsToRemove = new HashSet<Card>(selectedCards, CardEqualityComparer.Instance);
        playerHand.RemoveAll(pCard => cardsToRemove.Contains(pCard.Card));

        Console.WriteLine($"Player played a {HandMatcher.ToString(currentHandType)} with {selectedCards.Count} cards.");
        Console.WriteLine("Cards played:");
        foreach (var card in selectedCards)
        {
            Console.WriteLine($"- {card.Rank} of {card.Suit}");
        }

        onHandPlayed?.Invoke(this, EventArgs.Empty);
        ReGroupCards();
        selectedCards.Clear();
        currentHandType = HandMatcher.HandType.None;
        playCardButton.SetActive(HandMatcher.HandType.None);
    }

    // Improved: Made static for better performance (no instance allocation needed)
    private class CardEqualityComparer : IEqualityComparer<Card>
    {
        public static readonly CardEqualityComparer Instance = new CardEqualityComparer();
        
        public bool Equals(Card x, Card y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Rank == y.Rank && x.Suit == y.Suit;
        }

        public int GetHashCode(Card obj)
        {
            if (obj == null) return 0;
            return HashCode.Combine(obj.Rank, obj.Suit);
        }
    }

    private void ReGroupCards()
    {
        for (int i = 0; i < playerHand.Count; i++)
        {
            playerHand[i].Position = new Vector2(HandStartX + i * HandSpacing, PlayerHandY);
        }
        for (int i = 0; i < aiHand.Count; i++)
        {
            aiHand[i].Position = new Vector2(HandStartX + i * HandSpacing, AiHandY);
        }
    }
    
    private void CheckSelection(Card card, ContentManager Content)
    {
        if (card == null || Content == null)
            return;
            
        // Use FindIndex for better performance and safety
        int aiCardIndex = aiHand.FindIndex(aiCard => 
            aiCard?.Card != null && 
            aiCard.Card.Rank == card.Rank && 
            aiCard.Card.Suit == card.Suit);
        
        if (aiCardIndex >= 0)
        {
            Card newCard = aiHand[aiCardIndex].Card;
            aiHand.RemoveAt(aiCardIndex);
            ReGroupCards(); 
            
            PlayerCard playerCard = new PlayerCard(newCard, 
                new Vector2(HandStartX + playerHand.Count * HandSpacing, PlayerHandY));
            AttachCardEventHandlers(playerCard);
            playerCard.LoadContent(Content); 
            playerHand.Add(playerCard);
        }
    }
    // I'm using composition here rather than inheritance for PlayerCard and AiCard
    private void Deal(ContentManager Content)
    {
        if (Content == null)
            throw new ArgumentNullException(nameof(Content));
        if (deck == null)
            throw new InvalidOperationException("Deck is not initialized");
            
        cardSelector.onCardSelected += (s, e) =>
        {
            Card selected = cardSelector.SelectedCard;
            if (selected != null)
            {
                CheckSelection(selected, Content);
            }
        };
        
        // Improved: Add validation for deck size
        int cardsNeeded = HandSize * 2; // Player + AI hands
        if (deck.Cards.Count < cardsNeeded)
        {
            throw new InvalidOperationException($"Not enough cards in deck. Need {cardsNeeded}, have {deck.Cards.Count}");
        }
        
        for (int i = 0; i < HandSize; i++)
        {
            Card playerCard = deck.DrawCard();
            PlayerCard pCard = new PlayerCard(playerCard, 
                new Vector2(HandStartX + i * HandSpacing, PlayerHandY));
            AttachCardEventHandlers(pCard);
            playerHand.Add(pCard);
            
            Card aiCard = deck.DrawCard();
            AiCard aCard = new AiCard(aiCard, 
                new Vector2(HandStartX + i * HandSpacing, AiHandY));
            aiHand.Add(aCard);
        }
    }

    // Refactored: Extract duplicate event handler code into reusable method
    private void AttachCardEventHandlers(PlayerCard playerCard)
    {
        playerCard.onSelect += (s, e) =>
        {
            selectedCards.Add(playerCard.Card);
            UpdateHandMatch();
        };
        playerCard.onDeselect += (s, e) =>
        {
            selectedCards.Remove(playerCard.Card);
            UpdateHandMatch();
        };
    }

    private void UpdateHandMatch()
    {
        currentHandType = HandMatcher.IsMatch(selectedCards);
        playCardButton.SetActive(currentHandType);
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        MS = Mouse.GetState(); // Fixed: Update mouse state
        cardSelector.Update(gameTime, graphics);
        playCardButton.UpdateSelection(MS, graphics);
        foreach (PlayerCard pCard in playerHand)
        {
            pCard.UpdateSelection(MS, graphics); 
        }
    }
    public void LoadContent(ContentManager Content)
    {
        cardSelector.LoadContent(Content);
        playCardButton.LoadContent(Content);
        foreach (var pCard in playerHand)
        {
            pCard.LoadContent(Content);
        }
        AiCard.Texture = Content.Load<Texture2D>("cardback");
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        cardSelector.Draw(spriteBatch);
        playCardButton.Draw(spriteBatch);
        //spriteBatch.DrawString(Fonts.MainFont, "Selected Hand Type: " + handMatch, new Vector2(50, 600), Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, Global.HandsLayerDepth);
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