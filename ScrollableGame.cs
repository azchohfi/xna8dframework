#if WINDOWS || SILVERLIGHT
#define MOUSE
#endif

#if (!WINDOWS && !SILVERLIGHT) || WINDOWS8
#define TOUCHPANEL
#endif

#if WINDOWS_PHONE || WINDOWS || WINDOWS8 || IPHONE || ANDROID || SILVERLIGHT
#define GAMEPAD
#endif

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XNA8DFramework.UI;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;

namespace XNA8DFramework
{
	public static class ScrollableGame
	{
		public static Vector2 Scrolling;

		public static Random Rnd { get; private set; }

		public static InputService Input;

		private static ScrollableRenderTarget _selectedScrollableRenderTarget;
		private static TouchLocation _touchLocation;
		private static TouchLocation _oldTouchLocation;
#if TOUCHPANEL
		public static bool TouchIsFromTouchPanel { get; private set; }
#endif
		private static Vector2 _mousePos;
		public static Vector2 MousePos
		{
			get
			{
				return _mousePos;
			}
		}
		private static Point _mousePoint;
		public static Point MousePoint
		{
			get
			{
				return _mousePoint;
			}
		}
		public static KeyboardState Oldkbs;
		public static KeyboardState Kbs;

		static ScrollableGame()
		{
			Rnd = new Random();
		}

		internal static void Begin()
		{
			Kbs = Keyboard.GetState();
			try
			{
				Oldkbs.GetPressedKeys();
			}
			catch
			{
				Oldkbs = Kbs;
			}
			if (Input != null)
			{
				_oldTouchLocation = _touchLocation;
				_touchLocation = Input.GetInputPoint();
				
#if TOUCHPANEL
				TouchIsFromTouchPanel = Input.TouchIsFromTouchPanel;
#if !MOUSE
				if (_touchLocation.State != TouchLocationState.Invalid)
#endif
#endif
				{
					_mousePos = _touchLocation.Position;
				}
			}
			_mousePoint.X = (int)_mousePos.X;
			_mousePoint.Y = (int)_mousePos.Y;
		}

		internal static void End()
		{
			Oldkbs = Kbs;
		}

		public static bool CheckLeftPressed()
		{
			return CheckLeftPressed(false);
		}

		internal static bool CheckLeftPressed(bool force)
		{
#if SILVERLIGHT
			return ((!InputService.EventOnly || force) && _touchLocation.State == TouchLocationState.Pressed) || InputService.JustPressedMouse > 0;
#else
			return _touchLocation.State == TouchLocationState.Pressed;
#endif
		}

		public static bool CheckLeftDown()
		{
			return _touchLocation.State == TouchLocationState.Moved;
		}

		public static bool CheckLeftReleased()
		{
			return _touchLocation.State == TouchLocationState.Released ||
				   (_touchLocation.State == TouchLocationState.Invalid && 
				   _oldTouchLocation.State == TouchLocationState.Moved);
		}

		public static bool CheckValidTouchLocation()
		{
#if TOUCHPANEL && !MOUSE
			return _touchLocation.State != TouchLocationState.Invalid;
#else
			return true;
#endif

		}

		public static bool IsKeyPressed(Keys key)
		{
			return Oldkbs.IsKeyUp(key) && Kbs.IsKeyDown(key);
		}

		internal static void Begin(ScrollableRenderTarget scrollableRenderTarget)
		{
			_selectedScrollableRenderTarget = scrollableRenderTarget;
			_mousePos.X -= scrollableRenderTarget.Position.X;
			_mousePos.Y -= scrollableRenderTarget.Position.Y;
			_mousePoint.X = (int) _mousePos.X;
			_mousePoint.Y = (int) _mousePos.Y;
		}

		internal static void End(ScrollableRenderTarget scrollableRenderTarget)
		{
			_selectedScrollableRenderTarget = null;
			_mousePos.X += scrollableRenderTarget.Position.X;
			_mousePos.Y += scrollableRenderTarget.Position.Y;
			_mousePoint.X = (int) _mousePos.X;
			_mousePoint.Y = (int) _mousePos.Y;
		}

		internal static bool InScrollableRenderTarget(Point point)
		{
			if (_selectedScrollableRenderTarget == null)
				return true;

			point.X += (int) _selectedScrollableRenderTarget.Position.X;
			point.Y += (int) _selectedScrollableRenderTarget.Position.Y;
			var rect = new Rectangle((int) _selectedScrollableRenderTarget.Position.X,
									 (int) _selectedScrollableRenderTarget.Position.Y,
									 _selectedScrollableRenderTarget.Width,
									 _selectedScrollableRenderTarget.Height);
			return rect.Contains(point);
		}
	}
}