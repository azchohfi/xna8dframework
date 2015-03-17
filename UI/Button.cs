using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.Animations;

namespace XNA8DFramework.UI
{
	public class Button : DrawableGameComponent, IDrawable8D
	{
		protected Texture2D _Texture;
		public Texture2D Texture
		{
			get
			{
				return _Texture;
			}
			set
			{
				_Texture = value;
				if (_Texture != null)
					_rec = new Rectangle((int)_position.X, (int)_position.Y, (int)(Texture.Width * Scale2D.X), (int)(Texture.Height * Scale2D.Y));
			}
		}

		protected Texture2D _Texture2;
		public Texture2D Texture2
		{
			get
			{
				return _Texture2;
			}
			set
			{
				_Texture2 = value;
				if (Texture == null && _Texture2 != null)
					_rec = new Rectangle((int)_position.X, (int)_position.Y, (int)(Texture2.Width * Scale2D.X), (int)(Texture2.Height * Scale2D.Y));
			}
		}

		public float Angle { get; set; }

		public virtual Vector2 Origin { get; set; }

		public virtual float Scale
		{
			get
			{
				return Scale2D.X;
			}
			set
			{
				Scale2D = new Vector2(value);
			}
		}

		Vector2 _scale2D;
		public virtual Vector2 Scale2D
		{
			get
			{
				return _scale2D;
			}
			set
			{
				_scale2D = value;
				if (_Texture != null)
					_rec = new Rectangle((int)_position.X, (int)_position.Y, (int)(Texture.Width * Scale2D.X), (int)(Texture.Height * Scale2D.Y));
				else if (_Texture2 != null)
					_rec = new Rectangle((int)_position.X, (int)_position.Y, (int)(Texture2.Width * Scale2D.X), (int)(Texture2.Height * Scale2D.Y));
			}
		}

		public Color Color { get; set; }

		public bool UseScrolling { get; set; }

		public object Tag { get; set; }

		public virtual int Width
		{
			get
			{
				if (Texture != null)
				{
					if (_rec.Width != (int)(Texture.Width * Scale2D.X))
						return _rec.Width;
					return (int)(Texture.Width * Scale2D.X);
				}
				if (Texture2 != null)
				{
					if (_rec.Width != (int)(Texture2.Width * Scale2D.X))
						return _rec.Width;
					return (int)(Texture2.Width * Scale2D.X);
				}
				return _rec.Width;
			}
			set
			{
				_rec.Width = value;
			}
		}

		public virtual double Alpha { get; set; }

		public virtual int Height
		{
			get
			{
				if (Texture != null)
				{
					if (_rec.Height != (int)(Texture.Height * Scale2D.Y))
						return _rec.Height;
					return (int)(Texture.Height * Scale2D.Y);
				}
				if (Texture2 != null)
				{
					if (_rec.Height != (int)(Texture2.Height * Scale2D.Y))
						return _rec.Height;
					return (int)(Texture2.Height * Scale2D.Y);
				}
				return _rec.Height;
			}
			set
			{
				_rec.Height = value;
			}
		}

		protected string Path;
		protected string Path2;
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

		public event EventHandler Click;

		public event EventHandler MouseOver;

		public event EventHandler MouseOut;

		protected SpriteBatch SpriteBatch;

		private readonly bool _animated;
		public bool IsMouseOver { get; private set; }
		private bool _mouseDown;
		public Vector2 MouseDownEffect { get; set; }
		private bool _pressedOver;

		public SpriteEffects Effects;

		protected bool _Hits;
		public bool Hits
		{
			get
			{
				return _Hits && Enabled;
			}
			private set
			{
				_Hits = value;
			}
		}

		internal static bool CancelClick = false;

		[Obsolete]
		public Button(Game game, string path, string path2, Vector2 position)
			: base(game)
		{
			MouseDownEffect = new Vector2(2);
			Angle = 0;
			Origin = Vector2.Zero;
			Path = path;
			Path2 = path2;
			_animated = true;
			Position = position;
			Hits = false;
			Effects = SpriteEffects.None;
			_pressedOver = false;
			Alpha = 1;
			Scale = 1;
			Color = Color.White;
			UseScrolling = false;
		}

		[Obsolete]
		public Button(Game game, string path, Vector2 position)
			: base(game)
		{
			MouseDownEffect = new Vector2(2);
			Angle = 0;
			Origin = Vector2.Zero;
			Path = path;
			Path2 = "";
			_animated = false;
			Position = position;
			Hits = false;
			Effects = SpriteEffects.None;
			_pressedOver = false;
			Alpha = 1;
			Scale = 1;
			Color = Color.White;
			UseScrolling = false;
		}

		public Button(Game game, Texture2D texture, Vector2 position)
			: base(game)
		{
			MouseDownEffect = new Vector2(2);
			Angle = 0;
			Origin = Vector2.Zero;
			Path = "";
			Path2 = "";
			_animated = false;
			Position = position;
			Hits = false;
			Effects = SpriteEffects.None;
			_pressedOver = false;
			Alpha = 1;
			Scale = 1;
			Color = Color.White;
			UseScrolling = false;
			Texture = texture;
		}

		public Button(Game game, Texture2D texture, Texture2D texture2, Vector2 position)
			: base(game)
		{
			MouseDownEffect = new Vector2(2);
			Angle = 0;
			Origin = Vector2.Zero;
			Path = "";
			Path2 = "";
			_animated = true;
			Position = position;
			Hits = false;
			Effects = SpriteEffects.None;
			_pressedOver = false;
			Alpha = 1;
			Scale = 1;
			Color = Color.White;
			UseScrolling = false;
			Texture = texture;
			Texture2 = texture2;
		}

		protected override void LoadContent()
		{
			SpriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
			if (!string.IsNullOrEmpty(Path))
			{
				Texture = Game.Content.Load<Texture2D>(Path);
			}

			if (!string.IsNullOrEmpty(Path2))
			{
				Texture2 = Game.Content.Load<Texture2D>(Path2);
				if(Texture == null)
					_rec = new Rectangle((int)_position.X, (int)_position.Y, (int)(Texture2.Width * Scale2D.X), (int)(Texture2.Height * Scale2D.Y));
			}

			base.LoadContent();
		}

		public override void Update(GameTime gameTime)
		{
			if (!Enabled || !Game.IsActive)
				return;

			_mouseDown = ScrollableGame.CheckLeftDown() || ScrollableGame.CheckLeftPressed();

			bool newMouseOver = MouseCollide();

			// Se antes não estava com o MouseOver e agora esta
			if (!IsMouseOver && newMouseOver)
			{
				if (MouseOver != null)
					MouseOver(this, EventArgs.Empty);
			}
			// Se antes estava com o MouseOver e agora não esta
			if (!newMouseOver && IsMouseOver)
			{
				if (MouseOut != null)
					MouseOut(this, EventArgs.Empty);
			}

			IsMouseOver = newMouseOver;

			Hits = Hits && _mouseDown && IsMouseOver;

			if (IsMouseOver && ScrollableGame.CheckLeftPressed())
			{
				_pressedOver = true;
				Hits = true;
			}

			if (ScrollableGame.CheckLeftReleased())
			{
				if (IsMouseOver && _pressedOver)
				{
					OnClick();
					Hits = true;
				}
				_pressedOver = false;
			}
			
			base.Update(gameTime);
		}

		protected virtual void OnClick()
		{
#if SILVERLIGHT
			GameStateManagement.InputService.JustPressedMouse = 0;
#endif
			if (Click != null)
			{
				if (CancelClick)
				{
					CancelClick = !CancelClick;
					return;
				}
				Click(this, EventArgs.Empty);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			if (!Visible)
				return;

			Texture2D tex = Texture;
			Vector2 vec = Vector2.Zero;
			if (IsMouseOver)
			{
				if (_animated)
					tex = Texture2;
				if (_mouseDown)
					vec = MouseDownEffect;
			}
			
			if (tex != null)
			{
#if SILVERLIGHT
				var color = new Color(Color, MathHelper.Clamp((float)Alpha, 0, 1));
#else
				var color = Color * (float)Alpha;
#endif

				if (UseScrolling)
					SpriteBatch.Draw(tex, Position + ScrollableGame.Scrolling + vec + Origin, null, color, Angle, Origin, Scale2D, Effects, 0);
				else
					SpriteBatch.Draw(tex, Position + vec + Origin, null, color, Angle, Origin, Scale2D, Effects, 0);
			}

			base.Draw(gameTime);
		}

		protected bool MouseCollide()
		{
			Point point;
			if (UseScrolling)
			{
				point = new Point(ScrollableGame.MousePoint.X - (int)ScrollableGame.Scrolling.X, ScrollableGame.MousePoint.Y - (int)ScrollableGame.Scrolling.Y);
			}
			else
			{
				point = ScrollableGame.MousePoint;
			}
			return Rec.Contains(point) && ScrollableGame.InScrollableRenderTarget(point) && (ScrollableGame.CheckValidTouchLocation() || ScrollableGame.CheckLeftReleased());
		}
	}
}
