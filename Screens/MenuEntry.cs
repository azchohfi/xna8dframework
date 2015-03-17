#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
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
	/// Helper class represents a single entry in a MenuScreen. By default this
	/// just draws the entry text string, but it can be customized to display menu
	/// entries in different ways. This also provides an event that will be raised
	/// when the menu entry is selected.
	/// </summary>
	public abstract class MenuEntry : IColorAnimatable, IAlphaAnimatable, IAngleAnimatable, IScaleAnimatable, IVector2Animatable
	{
		/// <summary>
		/// Tracks a fading selection effect on the entry.
		/// </summary>
		/// <remarks>
		/// The entries transition out of the selection effect when they are deselected.
		/// </remarks>
		protected float SelectionFade;

		public Color Color { get; set; }
		
		public double Alpha { get; set; }

		public float Scale { get; set; }

		public float Angle { get; set; }

		public Vector2 Origin { get; set; }

		public SpriteEffects Effect { get; set; }
		
		/// <summary>
		/// The position at which the entry is drawn. This is set by the MenuScreen
		/// each frame in Update.
		/// </summary>
		protected Vector2 position;

		/// <summary>
		/// Gets or sets the position at which to draw this menu entry.
		/// </summary>
		public virtual Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		protected Vector2 startPosition;
		public Vector2 StartPosition
		{
			get { return startPosition; }
			set { startPosition = value; }
		}

		protected MenuEntry()
		{
			Color = Color.White;
			Alpha = 1;
			Scale = 1;
		}

		protected MenuEntry(Vector2 startPosition)
			: this()
		{
			StartPosition = startPosition;
		}

		/// <summary>
		/// Event raised when the menu entry is selected.
		/// </summary>
		public event EventHandler<EventArgs> Selected;

		public object Tag { get; set; }

		public bool _Enabled = true;
		public virtual bool Enabled
		{
			get
			{
				return _Enabled;
			}
			set
			{
				_Enabled = value;
			}
		}
		public bool _Visible = true;
		public virtual bool Visible
		{
			get
			{
				return _Visible;
			}
			set
			{
				_Visible = value;
			}
		}

		/// <summary>
		/// Method for raising the Selected event.
		/// </summary>
		protected internal virtual void OnSelectEntry()
		{
#if SILVERLIGHT
			GameStateManagement.InputService.JustPressedMouse = 0;
#endif
			if (Selected != null)
				Selected(this, new EventArgs());
		}

		/// <summary>
		/// Updates the menu entry.
		/// </summary>
		public virtual void Update(MenuScreen screen, GameTime gameTime)
		{
			// When the menu selection changes, entries gradually fade between
			// their selected and deselected appearance, rather than instantly
			// popping to the new state.
			float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
			SelectionFade = Math.Max(SelectionFade - fadeSpeed, 0);
		}

		/// <summary>
		/// Draws the menu entry. This can be overridden to customize the appearance.
		/// </summary>
		public abstract void Draw(MenuScreen screen, GameTime gameTime);

		/// <summary>
		/// Queries how much space this menu entry requires.
		/// </summary>
		public abstract int GetHeight(MenuScreen screen);

		/// <summary>
		/// Queries how wide the entry is, used for centering on the screen.
		/// </summary>
		public abstract int GetWidth(MenuScreen screen);
	}
}
