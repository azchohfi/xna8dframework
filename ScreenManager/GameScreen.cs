#region File Description
//-----------------------------------------------------------------------------
// GameScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;

namespace GameStateManagement
{
	/// <summary>
	/// Enum describes the screen transition state.
	/// </summary>
	public enum ScreenState
	{
		TransitionOn,
		Active,
		TransitionOff,
		Hidden,
	}


	/// <summary>
	/// A screen is a single layer that has update and draw logic, and which
	/// can be combined with other layers to build up a complex menu system.
	/// For instance the main menu, the options menu, the "are you sure you
	/// want to quit" message box, and the main game itself are all implemented
	/// as screens.
	/// </summary>
	public abstract class GameScreen
	{
		#region Properties

	    /// <summary>
	    /// Normally when one screen is brought up over the top of another,
	    /// the first screen will transition off to make room for the new
	    /// one. This property indicates whether the screen is only a small
	    /// popup, in which case screens underneath it do not need to bother
	    /// transitioning off.
	    /// </summary>
	    public bool IsPopup { get; protected set; }

	    /// <summary>
		/// Indicates how long the screen takes to
		/// transition on when it is activated.
		/// </summary>
		public TimeSpan TransitionOnTime
		{
			get { return _transitionOnTime; }
			protected set { _transitionOnTime = value; }
		}

		TimeSpan _transitionOnTime = TimeSpan.Zero;


		/// <summary>
		/// Indicates how long the screen takes to
		/// transition off when it is deactivated.
		/// </summary>
		public TimeSpan TransitionOffTime
		{
			get { return _transitionOffTime; }
			protected set { _transitionOffTime = value; }
		}

		TimeSpan _transitionOffTime = TimeSpan.Zero;


		/// <summary>
		/// Gets the current position of the screen transition, ranging
		/// from zero (fully active, no transition) to one (transitioned
		/// fully off to nothing).
		/// </summary>
		public virtual float TransitionPosition
		{
			get { return _transitionPosition; }
			protected set { _transitionPosition = value; }
		}

		float _transitionPosition = 1;


		/// <summary>
		/// Gets the current alpha of the screen transition, ranging
		/// from 1 (fully active, no transition) to 0 (transitioned
		/// fully off to nothing).
		/// </summary>
		public float TransitionAlpha
		{
			get { return 1f - TransitionPosition; }
		}


		/// <summary>
		/// Gets the current screen transition state.
		/// </summary>
		public ScreenState ScreenState
		{
			get { return _screenState; }
			protected set { _screenState = value; }
		}

		ScreenState _screenState = ScreenState.TransitionOn;


		/// <summary>
		/// There are two possible reasons why a screen might be transitioning
		/// off. It could be temporarily going away to make room for another
		/// screen that is on top of it, or it could be going away for good.
		/// This property indicates whether the screen is exiting for real:
		/// if set, the screen will automatically remove itself as soon as the
		/// transition finishes.
		/// </summary>
		public bool IsExiting
		{
			get { return _isExiting; }
			protected internal set { _isExiting = value; }
		}

		bool _isExiting;


		/// <summary>
		/// Checks whether this screen is active and can respond to user input.
		/// </summary>
		public bool IsActive
		{
			get
			{
				return !_otherScreenHasFocus &&
					   (_screenState == ScreenState.TransitionOn ||
						_screenState == ScreenState.Active);
			}
		}

		bool _otherScreenHasFocus;


	    /// <summary>
	    /// Gets the manager that this screen belongs to.
	    /// </summary>
	    public ScreenManager ScreenManager { get; internal set; }

#if WINDOWS_PHONE || WINDOWS || ANDROID
		/// <summary>
		/// Gets the gestures the screen is interested in. Screens should be as specific
		/// as possible with gestures to increase the accuracy of the gesture engine.
		/// For example, most menus only need Tap or perhaps Tap and VerticalDrag to operate.
		/// These gestures are handled by the ScreenManager when screens change and
		/// all gestures are placed in the InputState passed to the HandleInput method.
		/// </summary>
		public GestureType EnabledGestures
		{
			get { return _enabledGestures; }
			protected set
			{
				_enabledGestures = value;

				// the screen manager handles this during screen changes, but
				// if this screen is active and the gesture types are changing,
				// we have to update the TouchPanel ourself.
				if (ScreenState == ScreenState.Active)
				{
					TouchPanel.EnabledGestures = value;
				}
			}
		}

		GestureType _enabledGestures = GestureType.None;
#endif

		/// <summary>
		/// Gets whether or not this screen is serializable. If this is true,
		/// the screen will be recorded into the screen manager's state and
		/// its Serialize and Deserialize methods will be called as appropriate.
		/// If this is false, the screen will be ignored during serialization.
		/// By default, all screens are assumed to be serializable.
		/// </summary>
		public bool IsSerializable
		{
			get { return _isSerializable; }
			protected set { _isSerializable = value; }
		}

		bool _isSerializable = true;

		public ContentManager content;

	    protected GameScreen()
	    {
	        IsPopup = false;
	    }

	    public object Tag { get; set; }
		
		#endregion

		#region Initialization


		/// <summary>
		/// Activates the screen. Called when the screen is added to the screen manager or if the game resumes
		/// from being paused or tombstoned.
		/// </summary>
		/// <param name="instancePreserved">
		/// True if the game was preserved during deactivation, false if the screen is just being added or if the game was tombstoned.
		/// On Xbox and Windows this will always be false.
		/// </param>
		public virtual void Activate(bool instancePreserved) { }


		/// <summary>
		/// Deactivates the screen. Called when the game is being deactivated due to pausing or tombstoning.
		/// </summary>
		public virtual void Deactivate() { }


		/// <summary>
		/// Unload content for the screen. Called when the screen is removed from the screen manager.
		/// </summary>
		public virtual void Unload()
		{
			if (content != null)
				content.Unload();
			content = null;
		}


		#endregion

		#region Update and Draw


		/// <summary>
		/// Allows the screen to run logic, such as updating the transition position.
		/// Unlike HandleInput, this method is called regardless of whether the screen
		/// is active, hidden, or in the middle of a transition.
		/// </summary>
		public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			_otherScreenHasFocus = otherScreenHasFocus;

			if (_isExiting)
			{
				// If the screen is going away to die, it should transition off.
				_screenState = ScreenState.TransitionOff;

				if (!UpdateTransition(gameTime, _transitionOffTime, 1))
				{
					// When the transition finishes, remove the screen.
					ScreenManager.RemoveScreen(this);
				}
			}
			else if (coveredByOtherScreen)
			{
				// If the screen is covered by another, it should transition off.
				if (UpdateTransition(gameTime, _transitionOffTime, 1))
				{
					// Still busy transitioning.
					_screenState = ScreenState.TransitionOff;
				}
				else
				{
					// Transition finished!
					_screenState = ScreenState.Hidden;
				}
			}
			else
			{
				// Otherwise the screen should transition on and become active.
				if (UpdateTransition(gameTime, _transitionOnTime, -1))
				{
					// Still busy transitioning.
					_screenState = ScreenState.TransitionOn;
				}
				else
				{
					// Transition finished!
					_screenState = ScreenState.Active;
				}
			}
		}


		/// <summary>
		/// Helper for updating the screen transition position.
		/// </summary>
		bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
		{
			// How much should we move by?
			float transitionDelta;

			if (time == TimeSpan.Zero)
				transitionDelta = 1;
			else
				transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

			// Update the transition position.
			TransitionPosition += transitionDelta * direction;

			// Did we reach the end of the transition?
			if (((direction < 0) && (TransitionPosition <= 0)) ||
				((direction > 0) && (TransitionPosition >= 1)))
			{
				TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
				return false;
			}

			// Otherwise we are still busy transitioning.
			return true;
		}


		/// <summary>
		/// Allows the screen to handle user input. Unlike Update, this method
		/// is only called when the screen is active, and not when some other
		/// screen has taken the focus.
		/// </summary>
		public virtual void HandleInput(GameTime gameTime, InputService input) { }


		/// <summary>
		/// This is called when the screen should draw itself.
		/// </summary>
		public virtual void Draw(GameTime gameTime) { }


		#endregion

		#region Public Methods

		/// <summary>
		/// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
		/// instantly kills the screen, this method respects the transition timings
		/// and will give the screen a chance to gradually transition off.
		/// </summary>
		public void ExitScreen()
		{
			if (TransitionOffTime == TimeSpan.Zero)
			{
				// If the screen has a zero transition time, remove it immediately.
				ScreenManager.RemoveScreen(this);
			}
			else
			{
				// Otherwise flag that it should transition off and then exit.
				_isExiting = true;
			}
		}


		#endregion
	}
}
