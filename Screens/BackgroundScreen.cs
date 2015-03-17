#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.Animations;
#endregion

namespace GameStateManagement
{
	/// <summary>
	/// The background screen sits behind all the other menu screens.
	/// It draws a background image that remains fixed in place regardless
	/// of whatever transitions the screens on top of it may be doing.
	/// </summary>
	public class BackgroundScreen : GameScreen, IColorAnimatable
	{
		#region Fields

		Texture2D _backgroundTexture;

		#endregion

		#region Initialization
		
		public Color Color { get; set; }
		
		public bool Rotate = false;

#if IPHONE
		bool _background = false;
#endif

#if IPHONE
		public BackgroundScreen()
			: this(false)
		{
		}
#endif

		public BackgroundScreen(
#if IPHONE
			bool background
#endif
			)
		{
#if IPHONE
			_background = background;
#endif
			Color = Color.White;
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		/// <summary>
		/// Loads graphics content for this screen. The background texture is quite
		/// big, so we use our own local ContentManager to load it. This allows us
		/// to unload before going from the menus into the game itself, wheras if we
		/// used the shared ContentManager provided by the Game class, the content
		/// would remain loaded forever.
		/// </summary>
		public override void Activate(bool instancePreserved)
		{
			if (!instancePreserved)
			{
#if IPHONE
				if (this._background)
				{
					_backgroundTexture = content.Load<Texture2D>(@"Textures\background");
				}
				else
				{
					_backgroundTexture = content.Load<Texture2D>(@"Textures\Default");
				}
#else
				_backgroundTexture = content.Load<Texture2D>(@"Textures\Default");
#endif
			}
		}

		#endregion

		#region Update and Draw


		/// <summary>
		/// Updates the background screen. Unlike most screens, this should not
		/// transition off even if it has been covered by another screen: it is
		/// supposed to be covered, after all! This overload forces the
		/// coveredByOtherScreen parameter to false in order to stop the base
		/// Update method wanting to transition off.
		/// </summary>
		public override void Update(GameTime gameTime, bool otherScreenHasFocus,
													   bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);
		}


		/// <summary>
		/// Draws the background screen.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

			spriteBatch.Begin();

#if SILVERLIGHT
			var color = new Color(Color, MathHelper.Clamp((Color.A / 255.0f) * TransitionAlpha, 0, 1));
#else
			var color = Color * TransitionAlpha;
#endif

			if (Rotate)
			{
				var origin =
					new Vector2(ScreenManager.Graphics.PreferredBackBufferWidth,
								ScreenManager.Graphics.PreferredBackBufferHeight)/2;
				spriteBatch.Draw(_backgroundTexture, origin, null, color, -MathHelper.PiOver2,
								 new Vector2(_backgroundTexture.Width, _backgroundTexture.Height)/2, 1, SpriteEffects.None, 0);
			}
			else
			{
				spriteBatch.Draw(_backgroundTexture, Vector2.Zero, color);
			}

			spriteBatch.End();
		}

		#endregion
	}
}
