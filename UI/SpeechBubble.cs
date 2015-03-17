using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.Animations;

namespace XNA8DFramework.UI
{
	public class SpeechBubble : DrawableGameComponent, IDrawable8D
	{
		protected Vector2 _Position;
		public virtual Vector2 Position
		{
			get
			{
				return _Position;
			}
			set
			{
				_Position = value;
				UpdateAnimatableStringPosition();
			}
		}

		void UpdateAnimatableStringPosition()
		{
			if (AnimatableString != null)
				AnimatableString.Position = _Position + new Vector2(SpacingX, SpacingY);
		}

		public AnimatableString AnimatableString { get; protected set; }
		public string Text
		{
			get
			{
				if (AnimatableString != null)
					return AnimatableString.Text;
				return "";
			}
			set
			{
				if (AnimatableString != null)
					AnimatableString.Text = value;
			}
		}

		public double Alpha { get; set; }

		public virtual float Scale
		{
			get
			{
				if (AnimatableString != null)
					return AnimatableString.Scale;
				return 0;
			}
			set
			{
				if (AnimatableString != null)
					AnimatableString.Scale = value;
			}
		}

		public virtual Vector2 Scale2D
		{
			get
			{
				if (AnimatableString != null)
					return AnimatableString.Scale2D;
				return Vector2.Zero;
			}
			set
			{
				if (AnimatableString != null)
					AnimatableString.Scale2D = value;
			}
		}

		public Color Color { get; set; }

		public float Angle
		{
			get
			{
				if (AnimatableString != null)
					return AnimatableString.Angle;
				return 0;
			}
			set
			{
				if (AnimatableString != null)
					AnimatableString.Angle = value;
			}
		}
		public Vector2 Origin
		{
			get
			{
				if (AnimatableString != null)
					return AnimatableString.Origin;
				return Vector2.Zero;
			}
			set
			{
				if (AnimatableString != null)
					AnimatableString.Origin = value;
			}
		}

		private readonly Texture2D _texture;

		SpriteBatch _spriteBatch;

		public int BorderSize { get; set; }

		public bool ShouldReScaleText { get; set; }

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
				if (ShouldReScaleText && AnimatableString != null)
					AnimatableString.MaxLineWidth = Width - 2 * SpacingX;
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
				if (ShouldReScaleText && AnimatableString != null)
					AnimatableString.MaxLineHeight = Height - 2 * SpacingY;
			}
		}

		private int _spacingX;
		public int SpacingX
		{
			get
			{
				return _spacingX;
			}
			set
			{
				_spacingX = value;
				if (ShouldReScaleText && AnimatableString != null)
					AnimatableString.MaxLineWidth = Width - 2 * SpacingX;
			}
		}

		private int _spacingY;
		public int SpacingY
		{
			get
			{
				return _spacingY;
			}
			set
			{
				_spacingY = value;
				if (ShouldReScaleText && AnimatableString != null)
					AnimatableString.MaxLineHeight = Height - 2 * SpacingY;
			}
		}

		Rectangle _bottomBorder;
		Rectangle _interior;
		Rectangle _leftBorder;
		Rectangle _leftBottomCorner;
		Rectangle _leftTopCorner;
		Rectangle _rightBorder;
		Rectangle _rightBottomCorner;
		Rectangle _rightTopCorner;
		Rectangle _topBorder;

		public SpeechBubble(Game game, Texture2D texture, Vector2 position, string text, SpriteFont font, int width, int height, int borderSize)
			: base(game)
		{
			if (font != null)
			{
				AnimatableString = new AnimatableString(game, text, font)
				{
					Color = Color.Black
				};
			}
			Width = width;
			Height = height;
			BorderSize = borderSize;
			SpacingX = borderSize;
			SpacingY = borderSize;

			if (AnimatableString != null)
			{
				AnimatableString.MaxLineWidth = width - 2 * SpacingX;
				AnimatableString.MaxLines = -1;
				AnimatableString.MaxLineHeight = height - 2 * SpacingY;
			}

			_texture = texture;
			Position = position;
			Alpha = 1;
			Color = Color.White;
			ShouldReScaleText = true;
		}

		public override void Initialize()
		{
			_spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

			if (AnimatableString != null)
			{
				AnimatableString.Initialize();
				UpdateAnimatableStringPosition();
			}

			_bottomBorder = new Rectangle(BorderSize, _texture.Height - BorderSize, _texture.Width - 2 * BorderSize, BorderSize);
			_interior = new Rectangle(BorderSize, BorderSize, _texture.Width - 2 * BorderSize, _texture.Height - 2 * BorderSize);
			_leftBorder = new Rectangle(0, BorderSize, BorderSize, _texture.Height - 2 * BorderSize);
			_leftBottomCorner = new Rectangle(0, _texture.Height - BorderSize, BorderSize, BorderSize);
			_leftTopCorner = new Rectangle(0, 0, BorderSize, BorderSize);
			_rightBorder = new Rectangle(_texture.Width - BorderSize, BorderSize, BorderSize, _texture.Height - 2 * BorderSize);
			_rightBottomCorner = new Rectangle(_texture.Width - BorderSize, _texture.Height - BorderSize, BorderSize, BorderSize);
			_rightTopCorner = new Rectangle(_texture.Width - BorderSize, 0, BorderSize, BorderSize);
			_topBorder = new Rectangle(BorderSize, 0, _texture.Width - 2 * BorderSize, BorderSize);

			base.Initialize();
		}

		public override void Draw(GameTime gameTime)
		{
			if (!Visible)
				return;

#if SILVERLIGHT
			var color = new Color(Color, MathHelper.Clamp((float)Alpha, 0, 1));
#else
			var color = Color * (float)Alpha;
#endif

			// Top
			_spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, BorderSize, BorderSize), _leftTopCorner, color);
			_spriteBatch.Draw(_texture, new Rectangle((int)Position.X + BorderSize, (int)Position.Y, Width - 2 * BorderSize, BorderSize), _topBorder, color);
			_spriteBatch.Draw(_texture, new Rectangle((int)Position.X + Width - BorderSize, (int)Position.Y, BorderSize, BorderSize), _rightTopCorner, color);

			// Middle
			_spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y + BorderSize, BorderSize, Height - 2 * BorderSize), _leftBorder, color);
			_spriteBatch.Draw(_texture, new Rectangle((int)Position.X + BorderSize, (int)Position.Y + BorderSize, Width - 2 * BorderSize, Height - 2 * BorderSize), _interior, color);
			_spriteBatch.Draw(_texture, new Rectangle((int)Position.X + Width - BorderSize, (int)Position.Y + BorderSize, BorderSize, Height - 2 * BorderSize), _rightBorder, color);

			//bottom
			_spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y + Height - BorderSize, BorderSize, BorderSize), _leftBottomCorner, color);
			_spriteBatch.Draw(_texture, new Rectangle((int)Position.X + BorderSize, (int)Position.Y + Height - BorderSize, Width - 2 * BorderSize, BorderSize), _bottomBorder, color);
			_spriteBatch.Draw(_texture, new Rectangle((int)Position.X + Width - BorderSize, (int)Position.Y + Height - BorderSize, BorderSize, BorderSize), _rightBottomCorner, color);

			if (AnimatableString != null)
				AnimatableString.Draw(gameTime);
		}
	}
}
