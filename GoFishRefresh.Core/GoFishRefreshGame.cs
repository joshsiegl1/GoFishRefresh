using System;
using GoFishRefresh.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GoFishRefresh.Core.UI;
namespace GoFishRefresh.Core
{
    /// <summary>
    /// The main class for the game, responsible for managing game components, settings, 
    /// and platform-specific configurations.
    /// </summary>
    public class GoFishRefreshGame : Game
    {
        SpriteBatch _spriteBatch;
        // Resources for drawing.
        MainGame mainGame; 
        MainUI mainUI;
            MainMenu mainMenu;
            CardSelectionPrompt selectionPrompt;
            enum ScreenState { Menu, Selecting, Playing, Quitting }
            ScreenState currentScreen = ScreenState.Menu;
        private GraphicsDeviceManager graphicsDeviceManager;
        /// <summary>
        /// Indicates if the game is running on a mobile platform.
        /// </summary>
        public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();
        /// <summary>
        /// Indicates if the game is running on a desktop platform.
        /// </summary>
        public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();
        /// <summary>
        /// Initializes a new instance of the game. Configures platform-specific settings, 
        /// initializes services like settings and leaderboard managers, and sets up the 
        /// screen manager for screen transitions.
        /// </summary>
        public GoFishRefreshGame()
        {
            IsMouseVisible = true;
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            mainGame = new MainGame(Content);
            mainUI = new MainUI();
            mainMenu = new MainMenu();
            selectionPrompt = null;
            mainMenu.OnSelected += (s, e) =>
            {
                if (e.Selected == "Play")
                {
                    currentScreen = ScreenState.Playing;
                }
                else if (e.Selected == "Test")
                {
                    // Enter selecting state using CardSelectionPrompt
                    selectionPrompt = new CardSelectionPrompt(7);
                    // Ensure prompt has content loaded (LoadContent already ran by now)
                    selectionPrompt.LoadContent(Content);
                    currentScreen = ScreenState.Selecting;
                    selectionPrompt.OnConfirm += (sender2, args2) =>
                    {
                        // Start game with selected cards
                        var cards = new List<Card>(selectionPrompt.SelectedCards);
                        mainGame = new MainGame(Content, cards);
                        AttachMainGameHandlers();
                        currentScreen = ScreenState.Playing;
                    };
                    selectionPrompt.OnBack += (sender2, args2) =>
                    {
                        // Return to main menu
                        selectionPrompt = null;
                        currentScreen = ScreenState.Menu;
                    };
                }
                else if (e.Selected == "Quit")
                {
                    currentScreen = ScreenState.Quitting;
                }
            };
            // Share GraphicsDeviceManager as a service.
            Services.AddService(typeof(GraphicsDeviceManager), graphicsDeviceManager);
            Content.RootDirectory = "Content";
            // Configure screen orientations.
            graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;


        }
        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            // Load supported languages and set the default language.
            List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
            var languages = new List<CultureInfo>();
            for (int i = 0; i < cultures.Count; i++)
            {
                languages.Add(cultures[i]);
            }
            // TODO You should load this from a settings file or similar,
            // based on what the user or operating system selected.
            var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
            LocalizationManager.SetCulture(selectedLanguage);
            // Set up full screen mode for desktop platforms.
            if (IsDesktop)
            {
                int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                graphicsDeviceManager.IsFullScreen = true;
                graphicsDeviceManager.PreferredBackBufferWidth = screenWidth;
                graphicsDeviceManager.PreferredBackBufferHeight = screenHeight;
                Window.IsBorderless = true;
                graphicsDeviceManager.ApplyChanges();
            }
        }
        /// <summary>
        /// Loads game content, such as textures and particle systems.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Textures.LoadContent(Content);
            Fonts.LoadFonts(Content); 
            mainMenu.LoadContent(Content);
            mainGame.LoadContent(Content); 
            mainUI.LoadContent(Content);
            if (selectionPrompt != null)
            {
                selectionPrompt.LoadContent(Content);
            }
            
            // Connect MainGame's onHandPlayed event to MainUI's PlayedCards
            AttachMainGameHandlers();
            
            base.LoadContent();
        }

        private void AttachMainGameHandlers()
        {
            if (mainGame == null) return;
            // Detach existing handlers to prevent duplicates
            // Note: C# events cannot be iterated; reassign by creating a new delegate subscription
            mainGame.onHandPlayed += (s, e) =>
            {
                if (e != null && e.PlayedCards != null && e.PlayedCards.Count > 0)
                {
                    if (e.IsPlayerHand)
                    {
                        mainUI.GetPlayerPlayedCards().AddNewHandPlayed(e.PlayedCards, e.HandType);
                    }
                    else
                    {
                        mainUI.GetAiPlayedCards().AddNewHandPlayed(e.PlayedCards, e.HandType);
                    }
                }
            };
            mainGame.onGameEnd += (s, e) =>
            {
                // Return to main menu after game ends
                currentScreen = ScreenState.Menu;
            };
        }
        /// <summary>
        /// Updates the game's logic, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for game updates.
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (currentScreen == ScreenState.Playing)
            {
                mainGame.Update(gameTime, graphicsDeviceManager);
                mainUI.Update(gameTime, graphicsDeviceManager);
            }
            else if (currentScreen == ScreenState.Menu)
            {
                mainMenu.Update(gameTime, graphicsDeviceManager);
            }
            else if (currentScreen == ScreenState.Selecting)
            {
                // Ensure prompt is ready
                if (selectionPrompt == null)
                {
                    selectionPrompt = new CardSelectionPrompt(7);
                    selectionPrompt.LoadContent(Content);
                    selectionPrompt.OnConfirm += (sender2, args2) =>
                    {
                        var cards = new List<Card>(selectionPrompt.SelectedCards);
                        mainGame = new MainGame(Content, cards);
                        currentScreen = ScreenState.Playing;
                    };
                }
                selectionPrompt.Update(gameTime, graphicsDeviceManager);
            }
            else if (currentScreen == ScreenState.Quitting)
            {
                Exit();
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// Draws the game's graphics, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for rendering.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the MonoGame orange color before drawing.
            GraphicsDevice.Clear(new Color(53, 101, 77));
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: Global.createTransformMatrix(graphicsDeviceManager));
            if (currentScreen == ScreenState.Playing)
            {
                mainGame.Draw(_spriteBatch);
                mainUI.Draw(_spriteBatch);
            }
            else if (currentScreen == ScreenState.Menu)
            {
                mainMenu.Draw(_spriteBatch);
            }
            else if (currentScreen == ScreenState.Selecting)
            {
                selectionPrompt?.Draw(_spriteBatch);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}