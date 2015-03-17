using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.Animations;

namespace XNA8DFramework.UI
{
	public class ScrollableImage : DrawableGameComponent, IVector2Animatable, IAngleAnimatable, IAlphaAnimatable, IScaleAnimatable, IColorAnimatable
	{
		protected Texture2D Texture;

		private Vector2 _position;
		public Vector2 Position
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

		private Rectangle _sourceRec;
		public Rectangle SourceRec
		{
			get
			{
				return _sourceRec;
			}
		}

		public void UpdateRec()
		{
			_rec.X = (int)_position.X;
			_rec.Y = (int)_position.Y;
		}

		public float Angle { get; set; }

		public Vector2 Origin { get; set; }

		public double Alpha { get; set; }

		public float Scale { get; set; }

		public Color Color { get; set; }

		public bool Horizontal { get; set; }
		public bool Vertical { get; set; }

		public SpriteEffects Effects;

		protected SpriteBatch SpriteBatch;

		float _clickXDif;
		float _clickYDif;

		public float XDif { get; private set; }
		public float YDif { get; private set; }

		public int Width
		{
			get
			{
				if (Texture != null)
					return Texture.Width;
				return 0;
			}
			set
			{
				_sourceRec.Width = value;
			}
		}

		public int Height
		{
			get
			{
				if (Texture != null)
					return Texture.Height;
				return 0;
			}
			set
			{
				_sourceRec.Height = value;
			}
		}

		public ScrollableImage(Game game, Texture2D texture)
			: this(game, texture, Vector2.Zero)
		{
		}

		public ScrollableImage(Game game, Texture2D texture, Vector2 position)
			: base(game)
		{
			Texture = texture;
			Position = position;
			Effects = SpriteEffects.None;
			Scale = 1;
			Alpha = 1;
			Color = Color.White;
			_sourceRec = new Rectangle(0, 0, texture.Width, texture.Height);

			_rec = new Rectangle((int)_position.X, (int)_position.Y, texture.Width, texture.Height);

			Horizontal = false;
			Vertical = true;
		}

		protected override void LoadContent()
		{
			SpriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

			base.LoadContent();
		}

		public override void Update(GameTime gameTime)
		{
			if (!Enabled || !Game.IsActive)
				return;

			if (Rec.Contains(ScrollableGame.MousePoint))
			{
				if (ScrollableGame.CheckLeftPressed())
				{
					_clickXDif = ScrollableGame.MousePos.X;
					_clickYDif = ScrollableGame.MousePos.Y;
				}
				else if (ScrollableGame.CheckLeftDown())
				{
					if (Horizontal)
					{
						XDif -= ScrollableGame.MousePos.X - _clickXDif;
						if (XDif > Width - _sourceRec.Width)
							XDif = Width - _sourceRec.Width;
						if (XDif < 0)
							XDif = 0;
						_sourceRec.X = (int)XDif;
						_clickXDif = ScrollableGame.MousePos.X;
					}
					if (Vertical)
					{
						YDif -= ScrollableGame.MousePos.Y - _clickYDif;
						if (YDif > Height - _sourceRec.Height)
							YDif = Height - _sourceRec.Height;
						if (YDif < 0)
							YDif = 0;
						_sourceRec.Y = (int) YDif;
						_clickYDif = ScrollableGame.MousePos.Y;
					}
				}
			}

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (!Visible)
				return;

			Texture2D tex = Texture;

#if SILVERLIGHT
			var color = new Color(Color, MathHelper.Clamp((Color.A / 255.0f) * (float)Alpha, 0, 1));
#else
			var color = Color * (float)Alpha;
#endif

			if (tex != null)
			{
				SpriteBatch.Draw(tex, Position + Origin, SourceRec, color, Angle, Origin, Scale, Effects, 0);
			}

			base.Draw(gameTime);
		}
	}
}
