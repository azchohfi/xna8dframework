using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using XNA8DFramework.Animations;
#if IPHONE || WINDOWS8 || WINDOWS_PHONE || ANDROID
using Microsoft.Xna.Framework.GamerServices;
#endif
#if IPHONE
using MonoTouch.UIKit;
using System.Linq;
#endif

namespace XNA8DFramework.UI
{
	public class InputBox : DrawableGameComponent, IVector2Animatable, IAlphaAnimatable, IScaleAnimatable, ISizeable
	{
		public Texture2D Textura { get; set; }

		public virtual SpriteFont Font { get; set; }

		public virtual double Alpha { get; set; }

		public virtual float Scale { get; set; }

		protected static readonly Queue<char> CharsQueue = new Queue<char>();

		public static void AddCharToQueue(char c)
		{
			CharsQueue.Enqueue(c);
		}

		public static bool CharsQueueContains(char c)
		{
			return CharsQueue.Contains(c);
		}

		public static void ClearCharsQueue()
		{
			CharsQueue.Clear();
		}

		private Vector2 _position;
		public virtual Vector2 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
				UpdateRec();
			}
		}

		private Rectangle _rec;
		public Rectangle Rec
		{
			get
			{
				return _rec;
			}
		}

		public void UpdateRec()
		{
			_rec.X = (int)_position.X;
			_rec.Y = (int)_position.Y;
		}

		public int MaxSize { get; set; }

		public bool IsPassword { get; set; }

		public const int DefaultTextMaxSize = 100;
		public virtual Color DefaultTextColor { get; set; }
		public virtual Color DefaultBackColor { get; set; }
		public virtual Color DefaultSelectedColor { get; set; }
		public class KeyPressedEventArgs : EventArgs
		{
			public char Key { get; protected set; }
			public KeyPressedEventArgs(char key)
			{
				Key = key;
			}
		}
		public event EventHandler GotFocus;
		public event EventHandler TextChanged;
		public event EventHandler<KeyPressedEventArgs> KeyPressedEvent;
		public event EventHandler SelectedEvent;
		protected string _textToDraw;
		protected string _text;
		public virtual string Text {
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				_textToDraw = _text;
				if (!string.IsNullOrEmpty(_textToDraw))
				{
					if (IsPassword)
					{
						var s = new StringBuilder();
						for (var i = 0; i < _textToDraw.Length; i++)
						{
							s.Append("*");
						}
						_textToDraw = s.ToString();
					}
					while (_textToDraw.Length > 0 && Font.MeasureString(_textToDraw).X*Scale > Width)
					{
						_textToDraw = _textToDraw.Substring(1, _textToDraw.Length - 1);
					}
				}
			}
		}
		public virtual string Description { get; set; }
		public const int DefaultWidth = 250;
		public const int DefaultHeight = 30;
		private int _width;
		public virtual int Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
				_rec = new Rectangle((int)_position.X, (int)_position.Y, _width, _height);
			}
		}
		private int _height;
		public virtual int Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
				_rec = new Rectangle((int)_position.X, (int)_position.Y, _width, _height);
			}
		}

		public static Guid SelectedGuid = Guid.NewGuid();

#if IPHONE || WINDOWS8 || WINDOWS_PHONE || ANDROID
		IAsyncResult _showKeyboardAsyncResult;
#endif
#if IPHONE
		UIViewController _gameViewController;
#endif

		private TimeSpan _cursorFlickTime;
		protected bool ShowCursorFlick;

		public Guid MyGuid;

		protected SpriteBatch SpriteBatch;

		protected bool _hits;
		public bool Hits
		{
			get
			{
				return _hits && Enabled;
			}
			private set
			{
				_hits = value;
			}
		}

		public InputBox(Game game, SpriteFont font, Vector2 position)
			: this(game, font, position, DefaultTextMaxSize)
		{
		}

		public InputBox(Game game, SpriteFont font, Vector2 position, int maxSize)
			: this(game, font, position, maxSize, false)
		{
		}

		public InputBox(Game game, SpriteFont font, Vector2 position, bool selected)
			: this(game, font, position, DefaultTextMaxSize, selected)
		{
		}

		public InputBox(Game game, SpriteFont font, Vector2 position, int maxSize, bool selected)
			: base(game)
		{
			Scale = 1;
			Alpha = 1;
			Font = font;
			IsPassword = false;
			ShowCursorFlick = false;
			MaxSize = maxSize;
			MyGuid = Guid.NewGuid();
			Position = position;
			Description = "";
			Width = DefaultWidth;
			Height = DefaultHeight;
			Text = "";
			DefaultTextColor = Color.White;
			DefaultBackColor = Color.Black;
			DefaultSelectedColor = Color.Blue;
#if !WINDOWS8 && !IPHONE
			if (selected)
				SelectedGuid = MyGuid;
#endif
			Hits = false;
			_cursorFlickTime = TimeSpan.Zero;
#if IPHONE
			if(_gameViewController == null)
				_gameViewController = (UIViewController)Game.Services.GetService (typeof(UIViewController));
#endif
		}

		public override void Initialize ()
		{
			if (SelectedGuid == MyGuid)
				SetFocus();
			base.Initialize ();
		}

		public void SetFocus()
		{
			ClearCharsQueue();
			SelectedGuid = MyGuid;
			if (GotFocus != null)
				GotFocus(this, EventArgs.Empty);
#if IPHONE || WINDOWS_PHONE || ANDROID || WINDOWS8
			if (ScrollableGame.TouchIsFromTouchPanel)
			{
				_showKeyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "", Description, Text, ar =>
				{
					string result = Guide.EndShowKeyboardInput(ar);
					_showKeyboardAsyncResult = null;
					ClearCharsQueue();
					if (SelectedGuid == MyGuid && result != null)
					{
						Text = result;
						if (LimitReached())
							Text = Text.Substring(0, MaxSize);
						OnTextChanged();
					}
				}, null, IsPassword);
#if IPHONE
				var subviews = _gameViewController.PresentedViewController.View.Subviews;
				var scrollViewContainer = subviews.FirstOrDefault(s => s is UIScrollView) as UIScrollView;
				if(scrollViewContainer != null)
				{
					var textField = 
						scrollViewContainer.Subviews.FirstOrDefault(s => s is UITextField) as UITextField;
					if(textField != null)
						textField.AutocorrectionType = UITextAutocorrectionType.No;
				}
#endif
			}
#endif
		}

		public void LoseFocus()
		{
			if (SelectedGuid == MyGuid)
			{
				SelectedGuid = Guid.NewGuid();
#if IPHONE || WINDOWS8 || WINDOWS_PHONE || ANDROID
				if (_showKeyboardAsyncResult != null)
				{
#if IPHONE || ANDROID
					//Guide.CancelKeyboardInput(showKeyboardAsyncResult);
#else
					//TODO
#endif
					_showKeyboardAsyncResult = null;
				}
#endif
			}
		}

		public bool HasFocus()
		{
			return SelectedGuid == MyGuid;
		}

		protected override void LoadContent()
		{
			SpriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

			Textura = new Texture2D(Game.GraphicsDevice, 1, 1);
			Textura.SetData(new[] { Color.White });

			_rec = new Rectangle((int)_position.X, (int)_position.Y, _width, _height);

			base.LoadContent();
		}

		public override void Update(GameTime gameTime)
		{
			if (!Enabled || !Game.IsActive)
				return;

			bool mouseDown = ScrollableGame.CheckLeftDown() || ScrollableGame.CheckLeftPressed();

			bool mouseOver = MouseCollide();

			Hits = Hits && mouseDown && mouseOver;

			if (mouseOver && ScrollableGame.CheckLeftPressed())
			{
				SetFocus();
				Hits = true;
			}

			if (SelectedGuid == MyGuid)
			{
				_cursorFlickTime += gameTime.ElapsedGameTime;
				if (_cursorFlickTime.TotalMilliseconds >= 500)
				{
					ShowCursorFlick = !ShowCursorFlick;
					_cursorFlickTime = TimeSpan.Zero;
				}
				if (ValidadeKeyboard())
				{
					if (SelectedEvent != null)
						SelectedEvent(this, new EventArgs());
				}
			}
			else
			{
				ShowCursorFlick = false;
			}

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (!Visible)
				return;

			var color = DefaultBackColor;
			if (SelectedGuid == MyGuid)
				color = DefaultSelectedColor;

#if SILVERLIGHT
			color = new Color(color, MathHelper.Clamp((color.A / 255.0f) * (float)Alpha, 0, 1));
#else
			color = color * (float)Alpha;
#endif

			SpriteBatch.Draw(Textura, Rec, color);
			Color colorDefaultTextWithAlpha = DefaultTextColor;
#if SILVERLIGHT
			colorDefaultTextWithAlpha = new Color(colorDefaultTextWithAlpha, (colorDefaultTextWithAlpha.A / 255.0f) * (float)Alpha);
#else
			colorDefaultTextWithAlpha = colorDefaultTextWithAlpha * (float)Alpha;
#endif

#if IPHONE || ANDROID
			SpriteBatch.DrawString(Font, _textToDraw, new Vector2(Position.X, Position.Y), colorDefaultTextWithAlpha, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
#else
			SpriteBatch.DrawString(Font, _textToDraw + (ShowCursorFlick ? "|" : ""), new Vector2(Position.X, Position.Y), colorDefaultTextWithAlpha, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
#endif
			base.Draw(gameTime);
		}

		public bool MouseCollide()
		{
			if (Rec.Contains(ScrollableGame.MousePoint.X, ScrollableGame.MousePoint.Y))
			{
				return true;
			}
			return false;
		}

		protected bool ValidadeKeyboard()
		{
			if (!Game.IsActive
#if IPHONE || WINDOWS8 || WINDOWS_PHONE || ANDROID
				|| _showKeyboardAsyncResult != null
#endif
				)
				return false;

			bool ret = false;

			if (ScrollableGame.Oldkbs.IsKeyUp(Keys.Back) && ScrollableGame.Kbs.IsKeyDown(Keys.Back))
			{
				if (!CharsQueue.Contains((char)8))
					AddCharToQueue((char)8);
			}
			if (ScrollableGame.Oldkbs.IsKeyUp(Keys.Enter) && ScrollableGame.Kbs.IsKeyDown(Keys.Enter))
			{
				ret = true;
			}

			while (CharsQueue.Count > 0)
			{
				char c = CharsQueue.Dequeue();
				if (c == 8)
					RemoveChar();
				else if (c == 13)
				{
					ret = true;
				}
				else if (((32 <= c && c <= 126) || (128 <= c && c <= 255)) && !LimitReached())
				{
					Text += c;
					OnKeyPressed(c);
					OnTextChanged();
				}
			}

			return ret;
		}

		private void RemoveChar()
		{
			bool changed = false;
			if (Text.Length > 0)
			{
				Text = Text.Remove(Text.Length - 1, 1);
				changed = true;
			}
			OnKeyPressed((char)8); // BackSpace Ascii Code
			if (changed)
				OnTextChanged();
		}

		private bool LimitReached()
		{
			return Text.Length >= MaxSize;
		}

		public void OnKeyPressed(char key)
		{
			if (KeyPressedEvent != null)
				KeyPressedEvent(this, new KeyPressedEventArgs(key));
		}

		public void OnTextChanged()
		{
			if (TextChanged != null)
				TextChanged(this, EventArgs.Empty);
		}
	}
}
