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
    #region Constants
    private const int HandSize = 7;
    private const int HandSpacing = 180;
    private const int PlayerHandY = 700;
    private const int AiHandY = 50;
    private const int HandStartX = 50;
    private const int PlayButtonY = 600;
    private const float VirtualWidth = 1920f;
    private const float LeftMargin = HandStartX;
    private const float RightMargin = 50f;
    private const float BaseCardWidth = 240f;
    #endregion

    #region Animation Settings
    private readonly Vector2 deckPosition = new Vector2(960, 540);
    private readonly float dealDuration = 0.35f;
    private readonly float dealDelayBetween = 0.08f;
    #endregion

    #region Game State
    private readonly Deck deck;
    private readonly List<PlayerCard> playerHand;
    private readonly List<AiCard> aiHand;
    private readonly List<Card> selectedCards;
    private readonly List<Card> playedCards;
    private readonly List<AnimatedCard> animatingCards;
    private readonly CardSelector cardSelector;
    private readonly PlayCardButton playCardButton;
    private MouseState currentMouseState;
    private HandMatcher.HandType currentHandType = HandMatcher.HandType.None;
    private int playerPoints = 0;
    private int aiPoints = 0;
    private bool showGoFishMessage = false;
    private float goFishMessageTimer = 0f;
    private const float GoFishMessageDuration = 3f;
    #endregion

    #region Events
    public event EventHandler<HandPlayedEventArgs> onHandPlayed;
    
    public class HandPlayedEventArgs : EventArgs
    {
        public List<Card> PlayedCards { get; }
        public HandMatcher.HandType HandType { get; }
        
        public HandPlayedEventArgs(List<Card> playedCards, HandMatcher.HandType handType)
        {
            PlayedCards = new List<Card>(playedCards);
            HandType = handType;
        }
    }

    private static int GetHandPoints(HandMatcher.HandType handType)
    {
        return handType switch
        {
            HandMatcher.HandType.Pair => 1,
            HandMatcher.HandType.TwoPair => 2,
            HandMatcher.HandType.ThreeOfAKind => 3,
            HandMatcher.HandType.Straight => 5,
            HandMatcher.HandType.Flush => 6,
            HandMatcher.HandType.FullHouse => 8,
            HandMatcher.HandType.FourOfAKind => 10,
            HandMatcher.HandType.StraightFlush => 15,
            HandMatcher.HandType.RoyalFlush => 25,
            _ => 0
        };
    }
    #endregion

    #region Constructors
    public MainGame(ContentManager Content)
    {
        deck = new Deck();
        deck.Shuffle();
        playerHand = new List<PlayerCard>();
        aiHand = new List<AiCard>();
        selectedCards = new List<Card>();
        playedCards = new List<Card>();
        animatingCards = new List<AnimatedCard>();
        cardSelector = new CardSelector();
        playCardButton = new PlayCardButton(new Vector2(HandStartX, PlayButtonY), "Play Selected Cards");
        playCardButton.onClick += OnPlayCardButtonClicked;
        Deal(Content);
    }

    /// <summary>
    /// Creates a new game with a custom starting hand (used for Test mode).
    /// </summary>
    public MainGame(ContentManager Content, IReadOnlyList<Card> startingCards)
    {
        deck = new Deck();
        deck.Shuffle();
        playerHand = new List<PlayerCard>();
        aiHand = new List<AiCard>();
        selectedCards = new List<Card>();
        playedCards = new List<Card>();
        animatingCards = new List<AnimatedCard>();
        cardSelector = new CardSelector();
        playCardButton = new PlayCardButton(new Vector2(HandStartX, PlayButtonY), "Play Selected Cards");
        playCardButton.onClick += OnPlayCardButtonClicked;

        AttachCardSelectorHandler(Content);
        InitializePlayerHand(startingCards, Content);
        InitializeAiHand();
        LoadContent(Content);
        ReGroupCards();
    }
    #endregion

    #region Initialization Helpers
    private void AttachCardSelectorHandler(ContentManager Content)
    {
        cardSelector.onCardSelected += (s, e) =>
        {
            Card selected = cardSelector.SelectedCard;
            if (selected != null)
            {
                TransferCardFromAiToPlayer(selected, Content);
            }
        };
    }

    private void InitializePlayerHand(IReadOnlyList<Card> startingCards, ContentManager Content)
    {
        if (startingCards == null || startingCards.Count == 0)
            return;

        float x = HandStartX;
        foreach (var card in startingCards)
        {
            var playerCard = new PlayerCard(card, new Vector2(x, PlayerHandY));
            playerHand.Add(playerCard);
            AttachCardEventHandlers(playerCard);
            deck.Cards.RemoveAll(c => c.Rank == card.Rank && c.Suit == card.Suit);
            x += HandSpacing;
        }
    }

    private void InitializeAiHand()
    {
        for (int i = 0; i < HandSize && deck.Cards.Count > 0; i++)
        {
            var card = deck.DrawCard();
            var aiCard = new AiCard(card, new Vector2(HandStartX + i * HandSpacing, AiHandY));
            aiHand.Add(aiCard);
        }
    }
    #endregion

    #region Event Handlers

    private void OnPlayCardButtonClicked(object sender, EventArgs e)
    {
        if (!CanPlayCards())
            return;

        var cardsToPlay = new List<Card>(selectedCards);
        playedCards.AddRange(cardsToPlay);
        RemoveCardsFromPlayerHand(selectedCards);
        
        // Award points for the hand
        playerPoints += GetHandPoints(currentHandType);
        
        onHandPlayed?.Invoke(this, new HandPlayedEventArgs(cardsToPlay, currentHandType));
        ResetSelection();
    }

    private bool CanPlayCards()
    {
        return selectedCards.Count > 0 && currentHandType != HandMatcher.HandType.None;
    }

    private void RemoveCardsFromPlayerHand(List<Card> cards)
    {
        var cardsToRemove = new HashSet<Card>(cards, CardEqualityComparer.Instance);
        playerHand.RemoveAll(pCard => cardsToRemove.Contains(pCard.Card));
        ReGroupCards();
    }

    private void ResetSelection()
    {
        selectedCards.Clear();
        currentHandType = HandMatcher.HandType.None;
        playCardButton.SetActive(HandMatcher.HandType.None);
    }

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
    #endregion

    #region Card Management

    private void ReGroupCards()
    {
        RepositionPlayerHand();
        RepositionAiHand();
    }

    private void RepositionPlayerHand()
    {
        if (playerHand.Count == 0)
            return;

        float cardWidth = BaseCardWidth * Global.CardScale;
        float availableWidth = VirtualWidth - LeftMargin - RightMargin;
        float spacingUsed = CalculateCardSpacing(cardWidth, availableWidth);
        float startX = LeftMargin;

        for (int i = 0; i < playerHand.Count; i++)
        {
            playerHand[i].Position = new Vector2(startX + i * spacingUsed, PlayerHandY);
            playerHand[i].Scale = Global.CardScale;
            playerHand[i].LayerDepth = Global.DisplayCardLayerDepth + i * 0.0001f;
        }
    }

    private float CalculateCardSpacing(float cardWidth, float availableWidth)
    {
        if (playerHand.Count <= 1)
            return HandSpacing;

        float totalWidth = cardWidth + (playerHand.Count - 1) * HandSpacing;
        if (totalWidth <= availableWidth)
            return HandSpacing;

        float spacing = (availableWidth - cardWidth) / Math.Max(1, playerHand.Count - 1);
        float minSpacing = Math.Max(20f, cardWidth * 0.25f);
        return Math.Max(minSpacing, spacing);
    }

    private void RepositionAiHand()
    {
        for (int i = 0; i < aiHand.Count; i++)
        {
            aiHand[i].Position = new Vector2(HandStartX + i * HandSpacing, AiHandY);
        }
    }
    private void TransferCardFromAiToPlayer(Card card, ContentManager Content)
    {
        if (card == null || Content == null)
            return;

        int aiCardIndex = FindAiCardIndex(card);
        
        if (aiCardIndex >= 0)
        {
            // AI has the card - transfer it to player
            AiCard aiCard = aiHand[aiCardIndex];
            aiHand.RemoveAt(aiCardIndex);
            ReGroupCards();
            AnimateCardTransfer(aiCard, Content);
        }
        else
        {
            // AI doesn't have the card - Go Fish!
            ShowGoFishMessage();
            DrawCardFromDeck(Content);
        }
    }

    private void ShowGoFishMessage()
    {
        showGoFishMessage = true;
        goFishMessageTimer = GoFishMessageDuration;
    }

    private void DrawCardFromDeck(ContentManager Content)
    {
        if (deck.Cards.Count == 0)
            return;

        Card drawnCard = deck.DrawCard();
        Vector2 playerTarget = new Vector2(HandStartX + playerHand.Count * HandSpacing, PlayerHandY);
        
        // Animate card from deck to player hand
        AnimatedCard animation = new AnimatedCard(drawnCard, deckPosition, playerTarget, 0.45f, 0f);
        animation.LoadContent(Content);
        animation.onAnimationComplete += (s, e) => OnTransferAnimationComplete(drawnCard, playerTarget, Content);
        animatingCards.Add(animation);
    }

    private int FindAiCardIndex(Card card)
    {
        return aiHand.FindIndex(aiCard =>
            aiCard?.Card != null &&
            aiCard.Card.Rank == card.Rank &&
            aiCard.Card.Suit == card.Suit);
    }

    private void AnimateCardTransfer(AiCard aiCard, ContentManager Content)
    {
        Vector2 playerTarget = new Vector2(HandStartX + playerHand.Count * HandSpacing, PlayerHandY);
        AnimatedCard animation = new AnimatedCard(aiCard.Card, aiCard.Position, playerTarget, 0.45f, 0f);
        animation.LoadContent(Content);
        animation.onAnimationComplete += (s, e) => OnTransferAnimationComplete(aiCard.Card, playerTarget, Content);
        animatingCards.Add(animation);
    }

    private void OnTransferAnimationComplete(Card card, Vector2 position, ContentManager Content)
    {
        PlayerCard playerCard = new PlayerCard(card, position);
        AttachCardEventHandlers(playerCard);
        playerCard.LoadContent(Content);
        playerHand.Add(playerCard);
        ReGroupCards();
    }
    private void Deal(ContentManager Content)
    {
        ValidateDealPreconditions(Content);
        AttachCardSelectorHandler(Content);
        DealCardsToPlayers();
        ReGroupCards();
    }

    private void ValidateDealPreconditions(ContentManager Content)
    {
        if (Content == null)
            throw new ArgumentNullException(nameof(Content));
        if (deck == null)
            throw new InvalidOperationException("Deck is not initialized");

        int cardsNeeded = HandSize * 2;
        if (deck.Cards.Count < cardsNeeded)
            throw new InvalidOperationException($"Not enough cards in deck. Need {cardsNeeded}, have {deck.Cards.Count}");
    }

    private void DealCardsToPlayers()
    {
        for (int i = 0; i < HandSize; i++)
        {
            DealPlayerCard(i);
            DealAiCard(i);
        }
    }

    private void DealPlayerCard(int index)
    {
        Card card = deck.DrawCard();
        Vector2 targetPosition = new Vector2(HandStartX + index * HandSpacing, PlayerHandY);
        PlayerCard playerCard = new PlayerCard(card, deckPosition);
        AttachCardEventHandlers(playerCard);
        float delay = (index * 2) * dealDelayBetween;
        playerCard.StartDeal(deckPosition, targetPosition, dealDuration, delay);
        playerHand.Add(playerCard);
    }

    private void DealAiCard(int index)
    {
        Card card = deck.DrawCard();
        Vector2 targetPosition = new Vector2(HandStartX + index * HandSpacing, AiHandY);
        AiCard aiCard = new AiCard(card, deckPosition);
        float delay = (index * 2 + 1) * dealDelayBetween;
        aiCard.StartDeal(deckPosition, targetPosition, dealDuration, delay);
        aiHand.Add(aiCard);
    }

    #endregion

    #region Update Logic

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        currentMouseState = Mouse.GetState();
        UpdateGoFishMessage(gameTime);
        UpdateGameComponents(gameTime, graphics);
        UpdateCardAnimations(gameTime);
        UpdatePlayerCardSelection(graphics);
        UpdateCardVisualEffects();
    }

    private void UpdateGoFishMessage(GameTime gameTime)
    {
        if (showGoFishMessage)
        {
            goFishMessageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (goFishMessageTimer <= 0)
            {
                showGoFishMessage = false;
            }
        }
    }

    private void UpdateGameComponents(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        // Don't allow interactions while Go Fish message is displayed
        if (!showGoFishMessage)
        {
            cardSelector.Update(gameTime, graphics);
            playCardButton.UpdateSelection(currentMouseState, graphics);
        }
    }

    private void UpdateCardAnimations(GameTime gameTime)
    {
        foreach (PlayerCard card in playerHand)
            card.Update(gameTime);

        foreach (AiCard card in aiHand)
            card.Update(gameTime);

        for (int i = animatingCards.Count - 1; i >= 0; i--)
        {
            animatingCards[i].Update(gameTime);
            if (animatingCards[i].IsFinished)
                animatingCards.RemoveAt(i);
        }
    }

    private void UpdatePlayerCardSelection(GraphicsDeviceManager graphics)
    {
        // Don't allow card selection while Go Fish message is displayed
        if (showGoFishMessage)
            return;

        int topCardIndex = GetTopCardIndexUnderMouse(graphics);

        for (int i = 0; i < playerHand.Count; i++)
        {
            bool allowClick = (i == topCardIndex);
            playerHand[i].UpdateSelection(currentMouseState, graphics, allowClick);
        }
    }

    private int GetTopCardIndexUnderMouse(GraphicsDeviceManager graphics)
    {
        Matrix invMatrix = Matrix.Invert(Global.createTransformMatrix(graphics));
        Vector2 mouseWorld = Vector2.Transform(new Vector2(currentMouseState.X, currentMouseState.Y), invMatrix);

        for (int i = playerHand.Count - 1; i >= 0; i--)
        {
            if (playerHand[i].ContainsPoint(mouseWorld))
                return i;
        }

        return -1;
    }

    private void UpdateCardVisualEffects()
    {
        bool hasSelection = playerHand.Any(card => card.IsSelected);
        foreach (PlayerCard card in playerHand)
        {
            card.TargetTint = (hasSelection && !card.IsSelected) ? 0.5f : 0f;
        }
    }
    #endregion

    #region Content and Rendering
    public void LoadContent(ContentManager Content)
    {
        cardSelector.LoadContent(Content);
        playCardButton.LoadContent(Content);
        
        foreach (var card in playerHand)
            card.LoadContent(Content);

        AiCard.Texture = Content.Load<Texture2D>("cardback");
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        DrawGameUI(spriteBatch);
        DrawPlayerHand(spriteBatch);
        DrawAiHand(spriteBatch);
        DrawAnimatedCards(spriteBatch);
        DrawGoFishMessage(spriteBatch);
    }

    private void DrawGoFishMessage(SpriteBatch spriteBatch)
    {
        if (!showGoFishMessage || Fonts.MainFont == null)
            return;

        // Draw semi-transparent overlay to dim the game
        Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.Black });
        spriteBatch.Draw(pixel, new Rectangle(0, 0, (int)VirtualWidth, 1080), null, Color.Black * 0.5f, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);

        // Draw "Go Fish!" message in large text
        string message = "Go Fish!";
        Vector2 textSize = Fonts.MainFont.MeasureString(message) * 3.5f;
        Vector2 position = new Vector2((VirtualWidth - textSize.X) / 2f, 400f);
        
        // Draw shadow for better visibility
        spriteBatch.DrawString(Fonts.MainFont, message, position + new Vector2(4, 4), Color.Black, 0f, Vector2.Zero, 3.5f, SpriteEffects.None, 0.91f);
        // Draw main text
        spriteBatch.DrawString(Fonts.MainFont, message, position, Color.Yellow, 0f, Vector2.Zero, 3.5f, SpriteEffects.None, 0.92f);
    }

    private void DrawGameUI(SpriteBatch spriteBatch)
    {
        cardSelector.Draw(spriteBatch);
        playCardButton.Draw(spriteBatch);
        DrawScores(spriteBatch);
        DrawDeckCount(spriteBatch);
    }

    private void DrawScores(SpriteBatch spriteBatch)
    {
        if (Fonts.MainFont == null)
            return;

        string scoreText = $"Player: {playerPoints}    AI: {aiPoints}";
        Vector2 textSize = Fonts.MainFont.MeasureString(scoreText) * 1.2f;
        Vector2 position = new Vector2(VirtualWidth - textSize.X - 40, 30);
        spriteBatch.DrawString(Fonts.MainFont, scoreText, position, Color.Gold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, Global.HandsLayerDepth);
    }

    private void DrawDeckCount(SpriteBatch spriteBatch)
    {
        if (Fonts.MainFont == null)
            return;

        string deckText = $"Deck: {deck.Cards.Count}";
        Vector2 textSize = Fonts.MainFont.MeasureString(deckText) * 1.2f;
        Vector2 position = new Vector2(VirtualWidth - textSize.X - 40, 70);
        spriteBatch.DrawString(Fonts.MainFont, deckText, position, Color.LightBlue, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, Global.HandsLayerDepth);
    }

    private void DrawPlayerHand(SpriteBatch spriteBatch)
    {
        foreach (var card in playerHand)
        {
            if (!card.IsDealing)
                card.Draw(spriteBatch);
        }
    }

    private void DrawAiHand(SpriteBatch spriteBatch)
    {
        foreach (var card in aiHand)
        {
            if (!card.IsDealing)
                card.Draw(spriteBatch);
        }
    }

    private void DrawAnimatedCards(SpriteBatch spriteBatch)
    {
        foreach (var animation in animatingCards)
            animation.Draw(spriteBatch);
    }
    #endregion

    #region Helper Classes
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
    #endregion
}