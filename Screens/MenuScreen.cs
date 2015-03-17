#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#if WINDOWS_PHONE || WINDOWS || WINDOWS8 || IPHONE || ANDROID || SILVERLIGHT
#define GAMEPAD
#endif

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNA8DFramework;
using XNA8DFramework.Animations;
#endregion

namespace GameStateManagement
{
	/// <summary>
	/// Base class for screens that contain a menu of options. The user can
	/// move up and down to select an entry, or cancel to back out of the screen.
	/// </summary>
	public abstract class MenuScreen : GameScreen, IColorAnimatable
	{
		#region Fields

		// the number of pixels to pad above and below menu entries for touch input
		protected const int MenuEntryPadding = 10;

		protected List<MenuEntry> menuEntries = new List<MenuEntry>();
		protected string MenuTitle;

		protected MenuEntry backMenuEntry = null;
		protected bool showBackMenuEntry = true;
		
		public bool Portrait = false;
		
		public Color Color { get; set; }

		public static event EventHandler ButtonClicked;

		#endregion

		#region Properties


		/// <summary>
		/// Gets the list of menu entries, so derived classes can add
		/// or change the menu contents.
		/// </summary>
		public IList<MenuEntry> MenuEntries
		{
			get { return menuEntries; }
		}

		protected MenuEntry BackMenuEntry
		{
			get { return backMenuEntry; }
		}

		protected bool ShowBackMenuEntry
		{
			get { return showBackMenuEntry; }
		}

		#endregion

		#region Initialization

		protected MenuScreen(string menuTitle)
			: this(menuTitle, true)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected MenuScreen(string menuTitle, bool showBackMenuEntry)
		{
			Color = Color.White;
			MenuTitle = menuTitle;

			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);

			this.showBackMenuEntry = showBackMenuEntry;

			if (showBackMenuEntry)
				backMenuEntry = new MenuTextEntry("Voltar");
		}

		#endregion

		#region Handle Input

		/// <summary>
		/// Allows the screen to create the hit bounds for a particular menu entry.
		/// </summary>
		protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
		{
#if IPHONE
			int scale = 1;
			if(ScreenManager.FixScaleIPhone)
				scale = (int)MonoTouch.UIKit.UIScreen.MainScreen.Scale;
#else
			const int scale = 1;
#endif
			// the hit bounds are the entire width of the screen, and the height of the entry
			// with some additional padding above and below.
			if(Portrait)
			{
				return new Rectangle(
					0,
					(int)entry.Position.Y - MenuEntryPadding * scale,
					ScreenManager.Graphics.PreferredBackBufferHeight,
					entry.GetHeight(this) + (MenuEntryPadding * 2 * scale));
			}
			return new Rectangle(
				0,
				(int)entry.Position.Y - MenuEntryPadding * scale,
				ScreenManager.Graphics.PreferredBackBufferWidth,
				entry.GetHeight(this) + (MenuEntryPadding * 2 * scale));
		}

		/// <summary>
		/// Responds to user input, changing the selected entry and accepting
		/// or cancelling the menu.
		/// </summary>
		public override void HandleInput(GameTime gameTime, InputService input)
		{
			var pressed = ScrollableGame.CheckLeftPressed();
			if (pressed)
			{
				CheckMenuEntrySelection(ScrollableGame.MousePoint, input);
			}

			if ((ShowBackMenuEntry && backMenuEntry.Enabled &&
				pressed && GetMenuEntryHitBounds(backMenuEntry).Contains(ScrollableGame.MousePoint))
#if GAMEPAD
				|| (input.CurrentGamePadState.IsButtonDown(Buttons.Back) &&
					input.LastGamePadState.IsButtonUp(Buttons.Back))
#endif
)
			{
				if (ButtonClicked != null)
					ButtonClicked(backMenuEntry, EventArgs.Empty);
				OnCancel();
			}
		}

		protected void CheckMenuEntrySelection(Point tapLocation, InputService input)
		{
			// iterate the entries to see if any were tapped
			for (int i = 0; i < menuEntries.Count; i++)
			{
				MenuEntry menuEntry = menuEntries[i];

				if (GetMenuEntryHitBounds(menuEntry).Contains(tapLocation) && menuEntry.Enabled)
				{
					OnSelectEntry(i);
				}
			}
		}

		/// <summary>
		/// Handler for when the user has chosen a menu entry.
		/// </summary>
		protected virtual void OnSelectEntry(int entryIndex)
		{
			if(ButtonClicked != null)
				ButtonClicked(menuEntries[entryIndex], null);
			menuEntries[entryIndex].OnSelectEntry();
		}


		/// <summary>
		/// Handler for when the user has cancelled the menu.
		/// </summary>
		protected virtual void OnCancel()
		{
			ExitScreen();
		}

		#endregion

		#region Update and Draw


		/// <summary>
		/// Allows the screen the chance to position the menu entries. By default
		/// all menu entries are lined up in a vertical list, centered on the screen.
		/// </summary>
		protected virtual void UpdateMenuEntryLocations()
		{
			// Make the menu slide into place during transitions, using a
			// power curve to make things look more interesting (this makes
			// the movement slow down as it nears the end).
			var transitionOffset = (float)Math.Pow(TransitionPosition, 2);

#if IPHONE
			float xDif = 1;
			if(ScreenManager.FixScaleIPhone)
				xDif = MonoTouch.UIKit.UIScreen.MainScreen.Scale;
#else
			const float xDif = 1;
#endif

			// start at Y = 175; each X value is generated per entry
			var position = new Vector2(0f, 87f * xDif);

			// update each menu entry's location in turn
			foreach (var menuEntry in menuEntries)
			{
				if (menuEntry.StartPosition.X == 0 && menuEntry.StartPosition.Y == 0)
				{
					// each entry is to be centered horizontally
					if(Portrait)
						position.X = ScreenManager.Graphics.PreferredBackBufferHeight / 2 - menuEntry.GetWidth(this) / 2;
					else
						position.X = ScreenManager.Graphics.PreferredBackBufferWidth / 2 - menuEntry.GetWidth(this) / 2;

					if (ScreenState == ScreenState.TransitionOn)
						position.X -= xDif * transitionOffset * 256;
					else
						position.X += xDif * transitionOffset * 512;

					// set the entry's position
					menuEntry.Position = position;
					
#if IPHONE
					int scale = 1;
					if (ScreenManager.FixScaleIPhone)
						scale = (int) MonoTouch.UIKit.UIScreen.MainScreen.Scale;
#else
					const int scale = 1;
#endif
					// move down for the next entry the size of this entry plus our padding
					position.Y += menuEntry.GetHeight(this) + (MenuEntryPadding * 2 * scale);
				}
				else
				{
					Vector2 pos = menuEntry.StartPosition;

					if (ScreenState == ScreenState.TransitionOn)
						pos.X -= xDif * transitionOffset * 256;
					else
						pos.X += xDif * transitionOffset * 512;

					menuEntry.Position = pos;
				}
			}

			if (ShowBackMenuEntry)
			{
				if (backMenuEntry.StartPosition.X == 0)
				{
					// each entry is to be centered horizontally
					if(Portrait)
						position.X = ScreenManager.Graphics.PreferredBackBufferHeight / 2 - backMenuEntry.GetWidth(this) / 2;
					else
						position.X = ScreenManager.Graphics.PreferredBackBufferWidth / 2 - backMenuEntry.GetWidth(this) / 2;

					if (ScreenState == ScreenState.TransitionOn)
						position.X -= xDif * transitionOffset * 256;
					else
						position.X += xDif * transitionOffset * 512;

					// set the entry's position
					backMenuEntry.Position = position;
				}
				else
				{
					Vector2 pos = backMenuEntry.StartPosition;

					if (ScreenState == ScreenState.TransitionOn)
						pos.X -= xDif * transitionOffset * 256;
					else
						pos.X += xDif * transitionOffset * 512;

					// set the entry's position
					backMenuEntry.Position = pos;
				}
			}
		}

		/// <summary>
		/// Updates the menu.
		/// </summary>
		public override void Update(GameTime gameTime, bool otherScreenHasFocus,
													   bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

			// Update each nested MenuEntry object.
			for (int i = 0; i < menuEntries.Count; i++)
			{
				if(menuEntries[i].Enabled)
					menuEntries[i].Update(this, gameTime);
			}

			if (ShowBackMenuEntry && backMenuEntry.Enabled)
				backMenuEntry.Update(this, gameTime);
		}


		/// <summary>
		/// Draws the menu.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			// make sure our entries are in the right place before we draw them
			UpdateMenuEntryLocations();

			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

			spriteBatch.Begin();

			// Draw each menu entry in turn.
			for (var i = 0; i < menuEntries.Count; i++)
			{
				var menuEntry = menuEntries[i];
				if (menuEntry.Visible)
					menuEntry.Draw(this, gameTime);
			}

			if (ShowBackMenuEntry && backMenuEntry.Visible)
				backMenuEntry.Draw(this, gameTime);

			DrawTitle();

			spriteBatch.End();
		}

		protected virtual void DrawTitle()
		{
			if (string.IsNullOrEmpty(MenuTitle))
				return;

			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			SpriteFont font = ScreenManager.Font;
			// Make the menu slide into place during transitions, using a
			// power curve to make things look more interesting (this makes
			// the movement slow down as it nears the end).
			var transitionOffset = (float)Math.Pow(TransitionPosition, 2);

			// Draw the menu title centered on the screen
			Vector2 titlePosition;
#if IPHONE
			float yTitle = 40;
		    if (ScreenManager.FixScaleIPhone)
		        yTitle *= MonoTouch.UIKit.UIScreen.MainScreen.Scale;
#else
			const float yTitle = 40;
#endif
			if (Portrait)
				titlePosition = new Vector2(ScreenManager.Graphics.PreferredBackBufferHeight / 2, yTitle);
			else
				titlePosition = new Vector2(ScreenManager.Graphics.PreferredBackBufferWidth / 2, yTitle);
			Vector2 titleOrigin = font.MeasureString(MenuTitle) / 2;
#if SILVERLIGHT
			var titleColor = new Color(Color, MathHelper.Clamp((Color.A / 255.0f) * TransitionAlpha, 0, 1));
#else
			var titleColor = Color * TransitionAlpha;
#endif

			const float titleScale = 1.25f;

#if IPHONE
			float titleYDif = 100;
		    if (ScreenManager.FixScaleIPhone)
		        titleYDif *= MonoTouch.UIKit.UIScreen.MainScreen.Scale;
#else
			const float titleYDif = 100;
#endif

			titlePosition.Y -= transitionOffset * titleYDif;

			spriteBatch.DrawString(font, MenuTitle, titlePosition, titleColor, 0,
								   titleOrigin, titleScale, SpriteEffects.None, 0);
		}
		
		/*
		public override void UnloadContent ()
		{
			base.UnloadContent ();
			
			foreach (MenuEntry menuEntry in menuEntries) {
				menuEntry.Dispose();
			}
			menuEntries.Clear();
			
			if(backMenuEntry != null)
				backMenuEntry.Dispose();
		}
		*/

		#endregion
	}
}
