#if WINDOWS || SILVERLIGHT
#define MOUSE
#endif

#if (!WINDOWS && !SILVERLIGHT) || WINDOWS8
#define TOUCHPANEL
#endif

#if WINDOWS_PHONE || WINDOWS || WINDOWS8 || IPHONE || ANDROID || SILVERLIGHT
#define GAMEPAD
#endif

#if WINDOWS_PHONE || WINDOWS || WINDOWS8 || IPHONE || ANDROID
#define GESTURES
#endif

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

#if SILVERLIGHT
using System.Windows;
using System.Windows.Input;
#endif

#if SILVERLIGHT
namespace Microsoft.Xna.Framework.Input.Touch
{
	// Summary:
	//     Provides methods and properties for interacting with a touch location on
	//     a touch screen device. Reference page contains links to related code samples.
	public struct TouchLocation : IEquatable<TouchLocation>
	{
		//
		// Summary:
		//     Initializes a new TouchLocation with an ID, state, position, and pressure.
		//
		// Parameters:
		//   id:
		//     ID of the new touch location.
		//
		//   state:
		//     State of the new touch location.
		//
		//   position:
		//     Position, in screen coordinates, of the new touch location.
		public TouchLocation(int id, TouchLocationState state, Vector2 position)
			: this()
		{
			Id = id;
			State = state;
			Position = position;
		}

		// Summary:
		//     Determines whether two TouchLocation instances are unequal.
		//
		// Parameters:
		//   value1:
		//     The TouchLocation to compare with the second.
		//
		//   value2:
		//     The TouchLocation to compare with the first.
		public static bool operator !=(TouchLocation value1, TouchLocation value2)
		{
			return !(value1 == value2);
		}
		//
		// Summary:
		//     Determines whether two TouchLocation instances are equal.
		//
		// Parameters:
		//   value1:
		//     The TouchLocation to compare with the second.
		//
		//   value2:
		//     The TouchLocation to compare with the first.
		public static bool operator ==(TouchLocation value1, TouchLocation value2)
		{
			return value1.Id == value2.Id && value1.Position == value2.Position && value1.State == value2.State;
		}

		// Summary:
		//     Gets the ID of the touch location.
		public int Id { get; private set; }
		//
		// Summary:
		//     Gets the position of the touch location.
		public Vector2 Position { get; private set; }
		//
		// Summary:
		//     Gets the state of the touch location.
		public TouchLocationState State { get; private set; }

		// Summary:
		//     Determines whether the current TouchLocation is equal to the specified object.
		//
		// Parameters:
		//   obj:
		//     The Object to compare with the touch location.
		public override bool Equals(object obj)
		{
			try
			{
				var other = (TouchLocation)obj;
				return Id == other.Id && Position == other.Position && State == other.State;
			}
			catch
			{
				return false;
			}
		}
		//
		// Summary:
		//     Determines whether the current TouchLocation is equal to the specified TouchLocation.
		//
		// Parameters:
		//   other:
		//     The TouchLocation to compare with this instance.
		public bool Equals(TouchLocation other)
		{
			return Id == other.Id && Position == other.Position && State == other.State;
		}
		//
		// Summary:
		//     Gets the hash code for this TouchLocation.
		public override int GetHashCode()
		{
			return Position.GetHashCode() + Id + State.GetHashCode();
		}
		//
		// Summary:
		//     Gets a string representation of the TouchLocation.
		public override string ToString()
		{
			return "{Id:" + Id + ", Position:" + Position + ", State:" + State + "}";
		}
		//
		// Summary:
		//     Attempts to get the previous location of this touch location object.
		//
		// Parameters:
		//   previousLocation:
		//     [OutAttribute] Previous location data, as a TouchLocation.
		public bool TryGetPreviousLocation(out TouchLocation previousLocation)
		{
			previousLocation = new TouchLocation();
			return false;
		}
	}
	// Summary:
	//     Defines the possible states of a touch location. Reference page contains
	//     links to related code samples.
	public enum TouchLocationState
	{
		// Summary:
		//     This touch location position is invalid. Typically, you will encounter this
		//     state when a new touch location attempts to get the previous state of itself.
		Invalid = 0,
		//
		// Summary:
		//     This touch location position was released.
		Released = 1,
		//
		// Summary:
		//     This touch location position is new.
		Pressed = 2,
		//
		// Summary:
		//     This touch location position was updated or pressed at the same position.
		Moved = 3,
	}
}
#endif

namespace GameStateManagement
{
	public class InputService : GameComponent
	{
#if MOUSE
		static MouseState _mouseState;
		static MouseState _oldMouseState;
#if SILVERLIGHT
		public static bool EventOnly = false;
		internal static int JustPressedMouse;
#endif
#endif

#if TOUCHPANEL
		List<TouchLocation> touchCollection;
		public bool TouchIsFromTouchPanel { get; private set; }
#endif

#if GAMEPAD
		public GamePadState CurrentGamePadState;
		public GamePadState LastGamePadState;
#endif

#if WINDOWS8 || SILVERLIGHT || WINDOWS
		GraphicsDeviceManager _graphics;
#endif

#if GESTURES
		public readonly List<GestureSample> Gestures = new List<GestureSample>();
#endif

		public InputService(Game game)
			: base(game)
		{
#if MOUSE
			_mouseState = _oldMouseState = Mouse.GetState();
#if SILVERLIGHT
			JustPressedMouse = 0;
#endif
#endif
#if TOUCHPANEL
			touchCollection = new List<TouchLocation>();
#endif
#if GAMEPAD
#if !SILVERLIGHT
			CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
#else
			GamePadState.MapKey(Buttons.Back, Keys.Escape);
#endif
			LastGamePadState = CurrentGamePadState;
#endif
		}

#if WINDOWS8 || SILVERLIGHT || WINDOWS
		public override void Initialize()
		{
			_graphics = Game.Services.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager;
			base.Initialize();
		}
#endif

		public override void Update (GameTime gameTime)
		{
			XNA8DFramework.ScrollableGame.Begin();
#if MOUSE
			_oldMouseState = _mouseState;
			_mouseState = Mouse.GetState();
#if SILVERLIGHT
			if (JustPressedMouse > 0)
				JustPressedMouse--;
#endif
#endif
#if TOUCHPANEL
			touchCollection.Clear();
			foreach(TouchLocation touch in TouchPanel.GetState())
			{
				if (touch.State != TouchLocationState.Invalid)
				{
					touchCollection.Add(touch);
				}
			}
#endif
#if GAMEPAD
			LastGamePadState = CurrentGamePadState;
			CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
#endif
#if GESTURES
			// Read in any detected gestures into our list for the screens to later process
			Gestures.Clear();
			while (TouchPanel.IsGestureAvailable)
			{
				Gestures.Add(TouchPanel.ReadGesture());
			}
#endif
			base.Update (gameTime);
		}

		public TouchLocation GetInputPoint()
		{
#if TOUCHPANEL
			if (touchCollection.Count > 0)
			{
				TouchIsFromTouchPanel = true;
#if WINDOWS8 || WINDOWS_PHONE
				var posT = touchCollection[0].Position;

#if WINDOWS_PHONE
				float propX = Game.GraphicsDevice.Viewport.Width / (float)TouchPanel.DisplayWidth;
				float propY = Game.GraphicsDevice.Viewport.Height / (float)TouchPanel.DisplayHeight;
				posT.X *= propX;
				posT.Y *= propY;
#endif
				posT.X -= Game.GraphicsDevice.Viewport.X;
				posT.Y -= Game.GraphicsDevice.Viewport.Y;

				TouchLocation previousState;
				touchCollection[0].TryGetPreviousLocation(out previousState);
				return new TouchLocation(touchCollection[0].Id, touchCollection[0].State, posT, previousState.State, previousState.Position);
#else
				return touchCollection[0];
#endif
			}
			TouchIsFromTouchPanel = false;
#endif
#if !MOUSE
#if WINDOWS_PHONE
			return new TouchLocation(-1, TouchLocationState.Invalid, Vector2.Zero);
#else
			return new TouchLocation(-1, TouchLocationState.Invalid, Vector2.Zero, TouchLocationState.Invalid, Vector2.Zero);
#endif
#endif

#if MOUSE
			var pos = new Vector2(_mouseState.X, _mouseState.Y);
#if SILVERLIGHT
			float propX = _graphics.PreferredBackBufferWidth / (float)Game.GraphicsDevice.Viewport.Width;
			float propY = _graphics.PreferredBackBufferHeight / (float)Game.GraphicsDevice.Viewport.Height;
			pos.X *= propX;
			pos.Y *= propY;
			pos.X -= Game.GraphicsDevice.Viewport.X;
			pos.Y -= Game.GraphicsDevice.Viewport.Y;
#endif
#if WINDOWS8 || WINDOWS
			float propX = _graphics.PreferredBackBufferWidth / (float)Game.Window.ClientBounds.Width;
			float propY = _graphics.PreferredBackBufferHeight / (float)Game.Window.ClientBounds.Height;
			pos.X *= propX;
			pos.Y *= propY;
			pos.X -= Game.GraphicsDevice.Viewport.X;
			pos.Y -= Game.GraphicsDevice.Viewport.Y;
#endif
			var touchLocationState = TouchLocationState.Invalid;

			if (_mouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
				touchLocationState = TouchLocationState.Pressed;
			else if(_mouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Pressed)
				touchLocationState = TouchLocationState.Moved;
			else if(_mouseState.LeftButton == ButtonState.Released && _oldMouseState.LeftButton == ButtonState.Pressed)
				touchLocationState = TouchLocationState.Released;

			return new TouchLocation(1, touchLocationState, pos);
#endif
		}

		public float ScrollWheelValue
		{
			get
			{
#if MOUSE && !SILVERLIGHT
				return _mouseState.ScrollWheelValue;
#else
				return 0;
#endif
			}
		}

#if SILVERLIGHT
		public static void Click(int X, int Y)
		{
			GameStateManagement.InputService.JustPressedMouse = 2;
			_mouseState = new MouseState()
			{
				X = X,
				Y = Y,
				LeftButton = ButtonState.Pressed
			};
		}
#endif
	}
}