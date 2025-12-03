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
    private readonly Vector2 deckPosition = new Vector2(1600, 540);
    private readonly float dealDuration = 0.35f;
    private readonly float dealDelayBetween = 0.08f;
    private const int DeckStackVisualCount = 5;
    private const float DeckStackOffset = 2f;
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
    private ContentManager content;
    private MouseState currentMouseState;
    private HandMatcher.HandType currentHandType = HandMatcher.HandType.None;
    private int playerPoints = 0;
    private int aiPoints = 0;
    private bool showGoFishMessage = false;
    private float goFishMessageTimer = 0f;
    private const float GoFishMessageDuration = 3f;
    private bool isPlayerTurn = true;
    private float aiTurnDelay = 0f;
    private const float AiTurnDelayDuration = 1.5f;
    private Random random = new Random();
    private string goFishNextTurnText = "";
    private bool showGameEndMessage = false;
    private float gameEndMessageTimer = 0f;
    private const float GameEndMessageDuration = 3f;
    private string gameEndWinner = "";
    private float cardSelectorAlpha = 1f;
    private const float CardSelectorFadeSpeed = 5f;
    private float overlayAlpha = 0f;
    private const float OverlayFadeSpeed = 5f;
    #endregion

    #region Events
    public event EventHandler<HandPlayedEventArgs> onHandPlayed;
    public event EventHandler onGameEnd;
    
    public class HandPlayedEventArgs : EventArgs
    {
        public List<Card> PlayedCards { get; }
        public HandMatcher.HandType HandType { get; }
        public bool IsPlayerHand { get; }
        
        public HandPlayedEventArgs(List<Card> playedCards, HandMatcher.HandType handType, bool isPlayerHand)
        {
            PlayedCards = new List<Card>(playedCards);
            HandType = handType;
            IsPlayerHand = isPlayerHand;
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
        content = Content;
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
        content = Content;
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
        
        onHandPlayed?.Invoke(this, new HandPlayedEventArgs(cardsToPlay, currentHandType, true));
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
            // Player gets another turn if they got the card
        }
        else
        {
            // AI doesn't have the card - Go Fish!
            if (deck.Cards.Count == 0)
            {
                // Deck is empty - end the game
                EndGame();
            }
            else
            {
                // Next turn will be AI's
                ShowGoFishMessage(false);
                DrawCardFromDeckAndSwitchTurn(Content, true);
            }
        }
    }

    private void ShowGoFishMessage(bool nextIsPlayerTurn)
    {
        showGoFishMessage = true;
        goFishMessageTimer = GoFishMessageDuration;
        goFishNextTurnText = nextIsPlayerTurn ? "Next: Player's Turn" : "Next: AI's Turn";
    }

    private void EndGame()
    {
        showGameEndMessage = true;
        gameEndMessageTimer = GameEndMessageDuration;
        
        if (playerPoints > aiPoints)
            gameEndWinner = "Player Wins!";
        else if (aiPoints > playerPoints)
            gameEndWinner = "AI Wins!";
        else
            gameEndWinner = "It's a Tie!";
    }

    private void DrawCardFromDeckAndSwitchTurn(ContentManager Content, bool switchTurn)
    {
        if (deck.Cards.Count == 0)
        {
            if (switchTurn)
            {
                isPlayerTurn = false;
                aiTurnDelay = AiTurnDelayDuration;
            }
            return;
        }

        Card drawnCard = deck.DrawCard();
        Vector2 playerTarget = new Vector2(HandStartX + playerHand.Count * HandSpacing, PlayerHandY);
        
        // Animate card from deck to player hand
        AnimatedCard animation = new AnimatedCard(drawnCard, deckPosition, playerTarget, 0.45f, 0f);
        animation.LoadContent(Content);
        animation.onAnimationComplete += (s, e) =>
        {
            OnTransferAnimationComplete(drawnCard, playerTarget, Content);
            if (switchTurn)
            {
                isPlayerTurn = false;
                aiTurnDelay = AiTurnDelayDuration;
            }
        };
        animatingCards.Add(animation);
    }

    private void ExecuteAiTurn(ContentManager Content)
    {
        // First, check if AI can play any hands
        CheckAndPlayAiHands();

        if (aiHand.Count == 0)
        {
            isPlayerTurn = true;
            return;
        }

        // AI asks for a random card rank
        Card.Ranks requestedRank = (Card.Ranks)random.Next(1, 14); // 1-13 (Ace to King)
        int playerCardIndex = FindPlayerCardByRank(requestedRank);

        if (playerCardIndex >= 0)
        {
            // Player has the card - transfer to AI
            TransferCardFromPlayerToAi(playerCardIndex, Content);
            // AI gets another turn
            aiTurnDelay = AiTurnDelayDuration;
        }
        else
        {
            // Player doesn't have it - AI goes fishing
            if (deck.Cards.Count == 0)
            {
                // Deck is empty - end the game
                EndGame();
            }
            else
            {
                // Next turn will be Player's
                ShowGoFishMessage(true);
                DrawCardToAiFromDeck(Content);
                isPlayerTurn = true;
            }
        }
    }

    private void CheckAndPlayAiHands()
    {
        var aiCards = aiHand.Select(ai => ai.Card).ToList();
        
        while (true)
        {
            var (handType, cardsToPlay) = FindBestHandInCards(aiCards);
            
            if (handType == HandMatcher.HandType.None || cardsToPlay == null || cardsToPlay.Count == 0)
                break;
            
            // AI has a playable hand - remove cards and award points
            aiPoints += GetHandPoints(handType);
            
            // Trigger event for AI hand played
            onHandPlayed?.Invoke(this, new HandPlayedEventArgs(cardsToPlay, handType, false));
            
            // Remove the played cards from AI's hand
            foreach (var card in cardsToPlay)
            {
                int index = aiHand.FindIndex(ai => ai.Card != null && ai.Card.Rank == card.Rank && ai.Card.Suit == card.Suit);
                if (index >= 0)
                    aiHand.RemoveAt(index);
            }

            ReGroupCards();
            
            // Update card list for next iteration
            aiCards = aiHand.Select(ai => ai.Card).ToList();
        }
    }

    private (HandMatcher.HandType, List<Card>) FindBestHandInCards(List<Card> cards)
    {
        if (cards == null || cards.Count < 2)
            return (HandMatcher.HandType.None, null);

        // Check for 5-card hands first (highest priority)
        if (cards.Count >= 5)
        {
            // Try all combinations of 5 cards
            for (int i = 0; i < cards.Count - 4; i++)
            {
                for (int j = i + 1; j < cards.Count - 3; j++)
                {
                    for (int k = j + 1; k < cards.Count - 2; k++)
                    {
                        for (int l = k + 1; l < cards.Count - 1; l++)
                        {
                            for (int m = l + 1; m < cards.Count; m++)
                            {
                                var fiveCards = new List<Card> { cards[i], cards[j], cards[k], cards[l], cards[m] };
                                var handType = HandMatcher.IsMatch(fiveCards);
                                
                                // Royal Flush is the best, return immediately
                                if (handType == HandMatcher.HandType.RoyalFlush)
                                    return (handType, fiveCards);
                                // Straight Flush is second best
                                if (handType == HandMatcher.HandType.StraightFlush)
                                    return (handType, fiveCards);
                            }
                        }
                    }
                }
            }
            
            // Check for Full House and Flush
            for (int i = 0; i < cards.Count - 4; i++)
            {
                for (int j = i + 1; j < cards.Count - 3; j++)
                {
                    for (int k = j + 1; k < cards.Count - 2; k++)
                    {
                        for (int l = k + 1; l < cards.Count - 1; l++)
                        {
                            for (int m = l + 1; m < cards.Count; m++)
                            {
                                var fiveCards = new List<Card> { cards[i], cards[j], cards[k], cards[l], cards[m] };
                                var handType = HandMatcher.IsMatch(fiveCards);
                                
                                if (handType == HandMatcher.HandType.FullHouse || 
                                    handType == HandMatcher.HandType.Flush ||
                                    handType == HandMatcher.HandType.Straight)
                                    return (handType, fiveCards);
                            }
                        }
                    }
                }
            }
        }

        // Check for Four of a Kind
        var rankGroups = cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();
        if (rankGroups.Any(g => g.Count() >= 4))
        {
            var fourOfAKind = rankGroups.First(g => g.Count() >= 4).Take(4).ToList();
            return (HandMatcher.HandType.FourOfAKind, fourOfAKind);
        }

        // Check for Three of a Kind
        if (rankGroups.Any(g => g.Count() >= 3))
        {
            var threeOfAKind = rankGroups.First(g => g.Count() >= 3).Take(3).ToList();
            return (HandMatcher.HandType.ThreeOfAKind, threeOfAKind);
        }

        // Check for Two Pair
        if (rankGroups.Count(g => g.Count() >= 2) >= 2)
        {
            var pairs = rankGroups.Where(g => g.Count() >= 2).Take(2).SelectMany(g => g.Take(2)).ToList();
            if (pairs.Count == 4)
                return (HandMatcher.HandType.TwoPair, pairs);
        }

        // Check for Pair
        if (rankGroups.Any(g => g.Count() >= 2))
        {
            var pair = rankGroups.First(g => g.Count() >= 2).Take(2).ToList();
            return (HandMatcher.HandType.Pair, pair);
        }

        return (HandMatcher.HandType.None, null);
    }

    private int FindPlayerCardByRank(Card.Ranks rank)
    {
        return playerHand.FindIndex(pc => pc.Card.Rank == rank);
    }

    private void TransferCardFromPlayerToAi(int playerCardIndex, ContentManager Content)
    {
        PlayerCard playerCard = playerHand[playerCardIndex];
        Vector2 startPos = playerCard.Position;
        Card card = playerCard.Card;
        
        playerHand.RemoveAt(playerCardIndex);
        ReGroupCards();

        Vector2 aiTarget = new Vector2(HandStartX + aiHand.Count * HandSpacing, AiHandY);
        AnimatedCard animation = new AnimatedCard(card, startPos, aiTarget, 0.45f, 0f);
        animation.LoadContent(Content);
        animation.onAnimationComplete += (s, e) =>
        {
            AiCard aiCard = new AiCard(card, aiTarget);
            aiHand.Add(aiCard);
            ReGroupCards();
        };
        animatingCards.Add(animation);
    }

    private void DrawCardToAiFromDeck(ContentManager Content)
    {
        if (deck.Cards.Count == 0)
            return;

        Card drawnCard = deck.DrawCard();
        Vector2 aiTarget = new Vector2(HandStartX + aiHand.Count * HandSpacing, AiHandY);
        
        AnimatedCard animation = new AnimatedCard(drawnCard, deckPosition, aiTarget, 0.45f, 0f);
        animation.LoadContent(Content);
        animation.onAnimationComplete += (s, e) =>
        {
            AiCard aiCard = new AiCard(drawnCard, aiTarget);
            aiHand.Add(aiCard);
            ReGroupCards();
        };
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
        UpdateAiTurn(gameTime, graphics);
        UpdateGameComponents(gameTime, graphics);
        UpdateCardAnimations(gameTime);
        UpdatePlayerCardSelection(graphics);
        UpdateCardVisualEffects();
    }

    private void UpdateAiTurn(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        if (!isPlayerTurn && !showGoFishMessage && !showGameEndMessage)
        {
            aiTurnDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (aiTurnDelay <= 0)
            {
                ExecuteAiTurn(content);
            }
        }
    }

    private void UpdateGoFishMessage(GameTime gameTime)
    {
        // Fade overlay alpha based on whether any overlay is active
        bool overlayActive = showGoFishMessage || showGameEndMessage;
        float targetOverlayAlpha = overlayActive ? 1f : 0f;
        float dtOverlay = (float)gameTime.ElapsedGameTime.TotalSeconds;
        overlayAlpha = MathHelper.Lerp(overlayAlpha, targetOverlayAlpha, OverlayFadeSpeed * dtOverlay);

        if (showGoFishMessage)
        {
            goFishMessageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (goFishMessageTimer <= 0f)
            {
                showGoFishMessage = false;
            }
        }

        if (showGameEndMessage)
        {
            gameEndMessageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (gameEndMessageTimer <= 0f)
            {
                showGameEndMessage = false;
                onGameEnd?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void UpdateGameComponents(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        // Compute target visibility and fade alpha
        bool selectorShouldBeVisible = !showGoFishMessage && !showGameEndMessage && isPlayerTurn;
        float targetAlpha = selectorShouldBeVisible ? 1f : 0f;
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        cardSelectorAlpha = MathHelper.Lerp(cardSelectorAlpha, targetAlpha, CardSelectorFadeSpeed * dt);

        // Don't allow interactions while overlay is displayed or not player's turn
        if (selectorShouldBeVisible)
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
        DrawDeck(spriteBatch);
        DrawPlayerHand(spriteBatch);
        DrawAiHand(spriteBatch);
        DrawAnimatedCards(spriteBatch);
        DrawGoFishMessage(spriteBatch);
    }

    private void DrawGoFishMessage(SpriteBatch spriteBatch)
    {
        if ((overlayAlpha <= 0.01f) || Fonts.MainFont == null)
            return;

        // Draw semi-transparent overlay to dim the game
        Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.Black });
        spriteBatch.Draw(pixel, new Rectangle(0, 0, (int)VirtualWidth, 1080), null, Color.Black * (0.5f * overlayAlpha), 0f, Vector2.Zero, SpriteEffects.None, 0.9f);

        // Draw message in large text
        string message = showGameEndMessage ? gameEndWinner : "Go Fish!";
        Vector2 textSize = Fonts.MainFont.MeasureString(message) * 3.5f;
        Vector2 position = new Vector2((VirtualWidth - textSize.X) / 2f, 380f);
        
        // Draw shadow for better visibility
        spriteBatch.DrawString(Fonts.MainFont, message, position + new Vector2(4, 4), Color.Black * overlayAlpha, 0f, Vector2.Zero, 3.5f, SpriteEffects.None, 0.91f);
        // Draw main text
        spriteBatch.DrawString(Fonts.MainFont, message, position, Color.Yellow * overlayAlpha, 0f, Vector2.Zero, 3.5f, SpriteEffects.None, 0.92f);

        // Draw upcoming turn subtitle when showing Go Fish (not on game end)
        if (!showGameEndMessage && !string.IsNullOrEmpty(goFishNextTurnText))
        {
            Vector2 subSize = Fonts.MainFont.MeasureString(goFishNextTurnText) * 1.6f;
            Vector2 subPos = new Vector2((VirtualWidth - subSize.X) / 2f, position.Y + textSize.Y + 20f);
            // Shadow
            spriteBatch.DrawString(Fonts.MainFont, goFishNextTurnText, subPos + new Vector2(3, 3), Color.Black * overlayAlpha, 0f, Vector2.Zero, 1.6f, SpriteEffects.None, 0.91f);
            // Text
            spriteBatch.DrawString(Fonts.MainFont, goFishNextTurnText, subPos, Color.Yellow * overlayAlpha, 0f, Vector2.Zero, 1.6f, SpriteEffects.None, 0.92f);
        }
    }

    private void DrawDeck(SpriteBatch spriteBatch)
    {
        if (deck.Cards.Count == 0 || AiCard.Texture == null)
            return;

        // Draw multiple cardbacks with slight offset to show stack effect
        int cardsToShow = Math.Min(deck.Cards.Count, DeckStackVisualCount);
        for (int i = 0; i < cardsToShow; i++)
        {
            Vector2 offset = new Vector2(i * DeckStackOffset, i * DeckStackOffset);
            Vector2 drawPos = deckPosition + offset;
            spriteBatch.Draw(AiCard.Texture, drawPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, Global.DeckLayerDepth - (i * 0.001f));
        }
    }

    private void DrawGameUI(SpriteBatch spriteBatch)
    {
        // Draw card selector with fade when appropriate
        if (cardSelectorAlpha > 0.01f && isPlayerTurn && !showGoFishMessage && !showGameEndMessage)
            cardSelector.Draw(spriteBatch, cardSelectorAlpha);
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