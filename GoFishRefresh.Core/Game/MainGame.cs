#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
#endregion
public class MainGame
{
    // Dealing animation settings
    private Vector2 deckPosition = new Vector2(960, 540); // virtual center deck location
    private float dealDuration = 0.35f;
    private float dealDelayBetween = 0.08f; // seconds between each dealt card
    private Deck deck;
    private List<PlayerCard> playerHand;
    private List<AiCard> aiHand;
    private List<Card> selectedCards;
    private List<Card> playedCards; 
    private List<AnimatedCard> animatingCards = new List<AnimatedCard>();
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
    public event EventHandler<HandPlayedEventArgs> onHandPlayed;
    
    // Event args for when a hand is played
    public class HandPlayedEventArgs : EventArgs
    {
        public List<Card> PlayedCards { get; }
        public HandMatcher.HandType HandType { get; }
        
        public HandPlayedEventArgs(List<Card> playedCards, HandMatcher.HandType handType)
        {
            PlayedCards = new List<Card>(playedCards); // Create a copy
            HandType = handType;
        }
    } 
    public MainGame(ContentManager Content)
    {
        deck = new Deck();
        deck.Shuffle();
        playerHand = new List<PlayerCard>();
        aiHand = new List<AiCard>();
        selectedCards = new List<Card>();
        playedCards = new List<Card>();
        cardSelector = new CardSelector();
        playCardButton = new PlayCardButton(new Vector2(HandStartX, PlayButtonY), "Play Selected Cards");
        playCardButton.onClick += OnPlayCardButtonClicked;
        Deal(Content);
    }

    // New: allow starting with a custom player hand (e.g., Test mode)
    public MainGame(ContentManager Content, IReadOnlyList<Card> startingCards)
    {
        deck = new Deck();
        deck.Shuffle();
        playerHand = new List<PlayerCard>();
        aiHand = new List<AiCard>();
        selectedCards = new List<Card>();
        playedCards = new List<Card>();
        cardSelector = new CardSelector();
        playCardButton = new PlayCardButton(new Vector2(HandStartX, PlayButtonY), "Play Selected Cards");
        playCardButton.onClick += OnPlayCardButtonClicked;

        // Build player's hand from provided cards (no initial deal animation)
        if (startingCards != null && startingCards.Count > 0)
        {
            float x = HandStartX;
            for (int i = 0; i < startingCards.Count; i++)
            {
                var card = startingCards[i];
                var p = new PlayerCard(card, new Vector2(x, PlayerHandY));
                playerHand.Add(p);
                // Remove this card from deck to avoid duplicates when creating AI hand
                deck.Cards.RemoveAll(c => c.Rank == card.Rank && c.Suit == card.Suit);
                x += HandSpacing;
            }
        }

        // Ensure AI gets a full hand immediately (no animation)
        int aiTargetCount = HandSize;
        for (int i = 0; i < aiTargetCount && deck.Cards.Count > 0; i++)
        {
            var c = deck.DrawCard();
            var ai = new AiCard(c, new Vector2(HandStartX + i * HandSpacing, AiHandY));
            aiHand.Add(ai);
        }

        // Load content for current objects
        LoadContent(Content);
        // Group/space the cards properly
        ReGroupCards();
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

        // Invoke event with the played cards
        onHandPlayed?.Invoke(this, new HandPlayedEventArgs(cardsToPlay, currentHandType));
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
        // Compute spacing compression so the player's hand fits within the virtual width,
        // and center the hand when it is narrower than the available area.
        const float virtualWidth = 1920f;
        const float leftMargin = HandStartX; // starting X used elsewhere
        const float rightMargin = 50f;
        const float baseCardWidth = 240f; // approximate texture width in pixels
        float cardWidth = baseCardWidth * Global.CardScale;

        float availableWidth = virtualWidth - leftMargin - rightMargin;

        float spacingUsed = HandSpacing;
        float totalWidth = cardWidth;
        if (playerHand.Count > 1)
        {
            totalWidth = cardWidth + (playerHand.Count - 1) * spacingUsed;
        }

        if (playerHand.Count > 1 && totalWidth > availableWidth)
        {
            // Compress spacing to fit
            spacingUsed = (availableWidth - cardWidth) / Math.Max(1, playerHand.Count - 1);
            // Clamp spacing so cards don't overlap too much
            float minSpacing = Math.Max(20f, cardWidth * 0.25f);
            spacingUsed = Math.Max(minSpacing, spacingUsed);
            totalWidth = cardWidth + (playerHand.Count - 1) * spacingUsed;
            // Start at leftMargin when compressed
        }

        // Left-align the hand (start at the left margin)
        float startX = leftMargin;

        for (int i = 0; i < playerHand.Count; i++)
        {
            float x = startX + i * spacingUsed;
            playerHand[i].Position = new Vector2(x, PlayerHandY);
            playerHand[i].Scale = Global.CardScale; // keep default scale; spacing handles fit
            // Ensure left-most cards are drawn under the cards to their right by assigning
            // a small incremental layer depth per index (higher index draws later/on-top).
            playerHand[i].LayerDepth = Global.DisplayCardLayerDepth + i * 0.0001f;
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
            // Animate the AI card moving to the player's hand, then add it to the player's hand when done.
            AiCard ai = aiHand[aiCardIndex];
            Vector2 aiPos = ai.Position;
            // Remove AI card from AI hand immediately so it's no longer treated as an AI-held card
            aiHand.RemoveAt(aiCardIndex);
            ReGroupCards();

            Vector2 playerTarget = new Vector2(HandStartX + playerHand.Count * HandSpacing, PlayerHandY);
            AnimatedCard anim = new AnimatedCard(ai.Card, aiPos, playerTarget, 0.45f, 0f);
            anim.LoadContent(Content);
            anim.onAnimationComplete += (s, e) =>
            {
                // When animation completes, create a PlayerCard at the final position
                PlayerCard playerCard = new PlayerCard(ai.Card, playerTarget);
                AttachCardEventHandlers(playerCard);
                playerCard.LoadContent(Content);
                playerHand.Add(playerCard);
                // Recalculate grouping so the new card has correct position and layer
                ReGroupCards();
            };
            animatingCards.Add(anim);
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
        
        // Create cards and start them at the deck position, with staggered delays
        for (int i = 0; i < HandSize; i++)
        {
            // Player card (dealt on even steps)
            Card playerCard = deck.DrawCard();
            Vector2 playerTarget = new Vector2(HandStartX + i * HandSpacing, PlayerHandY);
            PlayerCard pCard = new PlayerCard(playerCard, deckPosition);
            AttachCardEventHandlers(pCard);
            // delay so dealing alternates: player at step (i*2)
            float playerDelay = (i * 2) * dealDelayBetween;
            pCard.StartDeal(deckPosition, playerTarget, dealDuration, playerDelay);
            playerHand.Add(pCard);

            // AI card (dealt on odd steps)
            Card aiCard = deck.DrawCard();
            Vector2 aiTarget = new Vector2(HandStartX + i * HandSpacing, AiHandY);
            AiCard aCard = new AiCard(aiCard, deckPosition);
            float aiDelay = (i * 2 + 1) * dealDelayBetween;
            aCard.StartDeal(deckPosition, aiTarget, dealDuration, aiDelay);
            aiHand.Add(aCard);
        }
        // Ensure initial grouping/layers are correct after dealing
        ReGroupCards();
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

        // Update any dealing animations first (so positions are current for selection)
        foreach (PlayerCard pCard in playerHand)
        {
            pCard.Update(gameTime);
        }
        foreach (AiCard aCard in aiHand)
        {
            aCard.Update(gameTime);
        }
        // Update animated transfers (AI -> player)
        for (int i = animatingCards.Count - 1; i >= 0; i--)
        {
            var a = animatingCards[i];
            a.Update(gameTime);
            if (a.IsFinished)
                animatingCards.RemoveAt(i);
        }

        // Determine the top-most player card under the cursor (iterate from top draw order)
        Matrix invMatrix = Matrix.Invert(Global.createTransformMatrix(graphics));
        Vector2 mouseWorld = Vector2.Transform(new Vector2(MS.X, MS.Y), invMatrix);
        int topIndex = -1;
        for (int i = playerHand.Count - 1; i >= 0; i--)
        {
            if (playerHand[i].ContainsPoint(mouseWorld))
            {
                topIndex = i;
                break;
            }
        }

        // Update selection: only the top-most card (if any) should accept click toggles
        for (int i = 0; i < playerHand.Count; i++)
        {
            bool allowClick = (i == topIndex);
            playerHand[i].UpdateSelection(MS, graphics, allowClick);
        }

        // Animate darkening of non-selected cards when any card is selected
        bool hasSelection = playerHand.Any(p => p.IsSelected);
        foreach (PlayerCard pCard in playerHand)
        {
            pCard.TargetTint = (hasSelection && !pCard.IsSelected) ? 0.5f : 0f;
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
        // Draw any animated transfer cards on top
        foreach (var anim in animatingCards)
        {
            anim.Draw(spriteBatch);
        }
    }
}