#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
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
using Microsoft.Xna.Framework.Input;
using XNA8DFramework.Animations;
#endregion

namespace GameStateManagement
{
	/// <summary>
	/// A popup message box screen, used to display "are you sure?"
	/// confirmation messages.
	/// </summary>
	public class MessageBoxScreen : MenuScreen, IScaleAnimatable
	{
		#region Fields

		protected string Message;
		protected Texture2D GradientTexture;

		public float Scale { get; set; }

		protected Vector2 TextSize;
		protected Vector2 TextPosition;

		protected string CancelText;

		protected MenuTextEntry OkMenuEntry;
		protected MenuTextEntry CancelMenuEntry;

		protected bool DrawContorno;

		// The background includes a border somewhat larger than the text itself.
		protected const int HPad = 32;
		protected const int VPad = 16;

		#endregion

		#region Events

		public event EventHandler<EventArgs> Ok;
		public event EventHandler<EventArgs> Cancel;

		#endregion

		#region Initialization

		public MessageBoxScreen(string title, string message, string cancelText)
			: this(title, message)
		{
			CancelText = cancelText;
		}

		public MessageBoxScreen(string title, string message)
			: base(title, false)
		{
			Message = message;
			CancelText = null;

			DrawContorno = true;

			IsPopup = true;

			Scale = 1;
			Color = Color.Black;

			TransitionOnTime = TimeSpan.FromSeconds(0.2);
			TransitionOffTime = TimeSpan.FromSeconds(0.2);
		}

		void OK_Selected(object sender, EventArgs e)
		{
			// Raise the accepted event, then exit the message box.
			if (Ok != null)
				Ok(this, EventArgs.Empty);

			ExitScreen();
		}

		void Cancel_Selected(object sender, EventArgs e)
		{
			if (Cancel != null)
				Cancel(this, EventArgs.Empty);

			ExitScreen();
		}

		/// <summary>
		/// Loads graphics content for this screen. This uses the shared ContentManager
		/// provided by the Game class, so the content will remain loaded forever.
		/// Whenever a subsequent MessageBoxScreen tries to load this same content,
		/// it will just get back another reference to the already loaded data.
		/// </summary>
		public override void Activate(bool instancePreserved)
		{
			GradientTexture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
			GradientTexture.SetData(new[] { Color.White });

			string s;
			TextSize = ScreenManager.Font.MeasureString(Message, Scale, ScreenManager.GraphicsDevice.Viewport.Width, -1, -1, out s);
			TextPosition = new Vector2((ScreenManager.Graphics.PreferredBackBufferWidth - TextSize.X) / 2, 75);

			OkMenuEntry = new MenuTextEntry("OK")
			{
				Scale = Scale, 
				Color = Color.Black
			};
			OkMenuEntry.StartPosition = new Vector2((ScreenManager.Graphics.PreferredBackBufferWidth / 2) - OkMenuEntry.GetWidth(this) / 2, TextPosition.Y + TextSize.Y * 2);
			OkMenuEntry.Selected += OK_Selected;
			MenuEntries.Add(OkMenuEntry);

			if (!string.IsNullOrEmpty(CancelText))
			{
				CancelMenuEntry = new MenuTextEntry(CancelText)
				{
					Scale = Scale,
					Color = Color.Black,
					StartPosition =
						new Vector2((ScreenManager.Graphics.PreferredBackBufferWidth/2) + HPad, TextPosition.Y + TextSize.Y*2)
				};
				CancelMenuEntry.Selected += Cancel_Selected;
				MenuEntries.Add(CancelMenuEntry);

				OkMenuEntry.StartPosition = new Vector2((ScreenManager.Graphics.PreferredBackBufferWidth / 2) - OkMenuEntry.GetWidth(this) - HPad, TextPosition.Y + TextSize.Y * 2);
			}
		}

		protected override Rectangle GetMenuEntryHitBounds(MenuEntry entry)
		{
			return new Rectangle((int)entry.Position.X, (int)entry.Position.Y,
				entry.GetWidth(this), entry.GetHeight(this));
		}

		public override void HandleInput(GameTime gameTime, InputService input)
		{
			if (ScrollableGame.IsKeyPressed(Keys.Enter))
			{
				OK_Selected(this, EventArgs.Empty);
			}
			if (ScrollableGame.IsKeyPressed(Keys.Escape))
			{
				Cancel_Selected(this, EventArgs.Empty);
			}

			base.HandleInput(gameTime, input);
		}

		#endregion

		#region Draw


		/// <summary>
		/// Draws the message box.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			if (DrawContorno)
			{
				SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

				// Fix for Silversprite
				spriteBatch.Begin();
				spriteBatch.End();

				// Darken down any other screens that were drawn beneath the popup.
				ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

				spriteBatch.Begin();

				var font = ScreenManager.Font;

#if SILVERLIGHT
				Color colorBack = new Color(Color.White, TransitionAlpha);
#else
				Color colorBack = Color.White * TransitionAlpha;
#endif

#if SILVERLIGHT
				Color colorText = new Color(Color.Black, TransitionAlpha);
#else
				Color colorText = Color.Black * TransitionAlpha;
#endif

				if (!string.IsNullOrEmpty(Message))
				{
					var backgroundRectangle = new Rectangle((int)TextPosition.X - HPad,
																	(int)TextPosition.Y - VPad,
																	(int)TextSize.X + HPad * 2,
																	(int)TextSize.Y + VPad * 2);
					// Draw the background rectangle.
					spriteBatch.Draw(GradientTexture, backgroundRectangle, colorBack);


					// Draw the message box text.
					spriteBatch.DrawWrappedStringWidth(font, Message, TextPosition, colorText, 0, Vector2.Zero, Scale, SpriteEffects.None, 0, ScreenManager.GraphicsDevice.Viewport.Width, out TextSize);
				}

				const int hPad = HPad / 2;
				const int vPad = VPad / 2;

				// Draw the message box text background rectangle.
				var rect = new Rectangle((int)OkMenuEntry.Position.X - hPad,
															  (int)OkMenuEntry.Position.Y - vPad,
															  OkMenuEntry.GetWidth(this) + hPad * 2,
															  OkMenuEntry.GetHeight(this) + vPad * 2);
				spriteBatch.Draw(GradientTexture, rect, colorBack);

				if (CancelMenuEntry != null)
				{
					// Draw the message box text background rectangle.
					rect = new Rectangle((int)CancelMenuEntry.Position.X - hPad,
																  (int)CancelMenuEntry.Position.Y - vPad,
																  CancelMenuEntry.GetWidth(this) + hPad * 2,
																  CancelMenuEntry.GetHeight(this) + vPad * 2);
					spriteBatch.Draw(GradientTexture, rect, colorBack);
				}

				spriteBatch.End();
			}

			base.Draw(gameTime);
		}


		#endregion
	}
}
