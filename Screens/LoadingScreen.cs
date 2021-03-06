#region File Description
//-----------------------------------------------------------------------------
// LoadingScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The loading screen coordinates transitions between the menu system and the
    /// game itself. Normally one screen will transition off at the same time as
    /// the next screen is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    /// 
    /// - Tell all the existing screens to transition off.
    /// - Activate a loading screen, which will transition on at the same time.
    /// - The loading screen watches the state of the previous screens.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next screen, which may take a long time to load its data. The loading
    ///   screen will be the only thing displayed while this load is taking place.
    /// </summary>
    public class LoadingScreen : GameScreen
    {
        #region Fields

        readonly bool _loadingIsSlow;
        bool _otherScreensAreGone;

        readonly GameScreen[] _screensToLoad;

        bool _showText = true;
        string _backgroundTexture = "";
        Texture2D _background;

        float _fadeBackBufferToWhiteAlpha;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private LoadingScreen(bool loadingIsSlow,
                              GameScreen[] screensToLoad)
        {
            _loadingIsSlow = loadingIsSlow;
            _screensToLoad = screensToLoad;

            // we don't serialize loading screens. if the user exits while the
            // game is at a loading screen, the game will resume at the screen
            // before the loading screen.
            IsSerializable = false;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                bool closeOtherWindows,
                                params GameScreen[] screensToLoad)
        {
            Load(screenManager, loadingIsSlow, closeOtherWindows, "", true, 0.0f, screensToLoad);
        }

        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                bool closeOtherWindows, string backgroundTexture, bool showText, float fadeBackBufferToWhiteAlpha,
                                params GameScreen[] screensToLoad)
        {
            if (closeOtherWindows)
            {
                // Tell all the current screens to transition off.
                foreach (GameScreen screen in screenManager.GetScreens())
                    screen.ExitScreen();
            }
            //screenManager.Game.Content.Unload();

            // Create and activate the loading screen.
            var loadingScreen = new LoadingScreen(loadingIsSlow,
                                                            screensToLoad)
                {
                    _backgroundTexture = backgroundTexture,
                    _showText = showText,
                    _fadeBackBufferToWhiteAlpha = fadeBackBufferToWhiteAlpha
                };
            if (!closeOtherWindows)
                loadingScreen._otherScreensAreGone = true;

            screenManager.AddScreen(loadingScreen);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (!string.IsNullOrEmpty(_backgroundTexture))
                    _background = content.LoadLocalized<Texture2D>(_backgroundTexture);
            }
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (_otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in _screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                    }
                }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
#if !SILVERLIGHT
                ScreenManager.Game.ResetElapsedTime();
#endif
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                _otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (_loadingIsSlow)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

                spriteBatch.Begin();
#if IPHONE
				var screenSize = new Vector2(ScreenManager.Graphics.PreferredBackBufferWidth, ScreenManager.Graphics.PreferredBackBufferHeight);
#else
				var screenSize = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
#endif
                if (_background != null)
                    spriteBatch.Draw(_background, screenSize / 2, null, Color.White, 0, new Vector2(_background.Width, _background.Height) / 2, 1, SpriteEffects.None, 0);
                if (_showText)
                {
                    SpriteFont font = ScreenManager.Font;

                    const string message = "Loading...";

                    // Center the text in the screen.
                    Vector2 textSize = font.MeasureString(message);
                    Vector2 textPosition = (screenSize - textSize) / 2;

#if SILVERLIGHT
                    var color = new Color(Color.White, MathHelper.Clamp(TransitionAlpha, 0, 1));
#else
                    var color = Color.White * TransitionAlpha;
#endif

                    // Draw the text.
                    spriteBatch.DrawString(font, message, textPosition, color);
                }

                spriteBatch.End();

                // Fix for Silversprite
                spriteBatch.Begin();
                spriteBatch.End();

                if (_fadeBackBufferToWhiteAlpha > 0)
                    ScreenManager.FadeBackBufferToWhite(_fadeBackBufferToWhiteAlpha);
            }
        }


        #endregion
    }
}
