using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.UI
{
	public class Scroll : DrawableGameComponent, IScroll
	{
		protected Texture2D Texture;

		public ScrollableRenderTarget ScrollContainer { get; set; }

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
		public Rectangle Rect
		{
			get
			{
				return _rec;
			}
		}

		public void UpdateRec()
		{
			_rec.X = (int)(_position.X + PosX);
			_rec.Y = (int)(_position.Y + PosY);
		}

		public float Angle { get; set; }

		public Vector2 Origin { get; set; }

		public double Alpha { get; set; }

		public Color Color { get; set; }

		public event EventHandler PercentageChanged;

		public bool Hits { get; protected set; }

		public bool Draging { get; protected set; }
		Vector2 _draggedDif;

		public float PosY { get; private set; }
		protected float _PercentageY;
		public float PercentageY
		{
			get
			{
				return _PercentageY;
			}
			set
			{
				_PercentageY = value;

				var heightDif = Height - BarHeight;
				PosY = heightDif * _PercentageY;

				if (heightDif <= 0 || PosY < 0)
					PosY = 0;
				else if (heightDif > 0 && PosY > heightDif)
					PosY = heightDif;
				UpdateRec();
				if (PercentageChanged != null)
					PercentageChanged(this, EventArgs.Empty);
			}
		}

		public float PosX { get; private set; }
		protected float _PercentageX;
		public float PercentageX
		{
			get
			{
				return _PercentageX;
			}
			set
			{
				_PercentageX = value;

				var widthDif = Width - BarWidth;
				PosX = widthDif * _PercentageX;

				if (widthDif <= 0 || PosX < 0)
					PosX = 0;
				else if (widthDif > 0 && PosX > widthDif)
					PosX = widthDif;
				UpdateRec();
				if (PercentageChanged != null)
					PercentageChanged(this, EventArgs.Empty);
			}
		}

		public SpriteEffects Effects;

		protected SpriteBatch SpriteBatch;

		public bool Horizontal { get; set; }
		public bool Vertical { get; set; }

		protected int _Width;
		public int Width
		{
			get
			{
				return _Width;
			}
			set
			{
				_Width = value;
			}
		}

		protected int _Height;
		public int Height
		{
			get
			{
				return _Height;
			}
			set
			{
				_Height = value;
			}
		}

		public int BarWidth
		{
			get
			{
				return Texture.Width;
			}
		}

		public int BarHeight
		{
			get
			{
				return Texture.Height;
			}
		}

		public Scroll(Game game, Texture2D texture)
			: this(game, texture, Vector2.Zero, null)
		{
		}

		public Scroll(Game game, Texture2D texture, ScrollableRenderTarget scrollContainer)
			: this(game, texture, Vector2.Zero, scrollContainer)
		{
		}

		public Scroll(Game game, Texture2D texture, Vector2 position)
			: this(game, texture, position, null)
		{
		}

		public Scroll(Game game, Texture2D texture, Vector2 position, ScrollableRenderTarget scrollContainer)
			: base(game)
		{
			Texture = texture;
			Position = position;
			ScrollContainer = scrollContainer;
			if (scrollContainer != null)
			{
				Width = scrollContainer.Width;
				Height = scrollContainer.Height;
			}
			Effects = SpriteEffects.None;
			Alpha = 1;
			Color = Color.White;
			Horizontal = true;
			Vertical = false;

			_rec = new Rectangle((int)_position.X, (int)_position.Y, BarWidth, BarHeight);
		}

		public override void Initialize()
		{
			base.Initialize();

			UpdateRec();
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

			// Select for Drag
			if (ScrollableGame.CheckLeftPressed(true))
			{
				if (Rect.Contains(ScrollableGame.MousePoint))
				{
					Draging = true;
					Hits = true;
					_draggedDif = ScrollableGame.MousePos;
				}
			}

			// Dragging 
			if (Draging && ScrollableGame.CheckLeftDown())
			{
				var pos = ScrollableGame.MousePos - _draggedDif;
				_draggedDif = ScrollableGame.MousePos;

				if (Horizontal)
				{
					PosX += (int)pos.X;
					var widthDif = Width - BarWidth;
					if (widthDif <= 0 || PosX < 0)
						PosX = 0;
					else if (widthDif > 0 && PosX > widthDif)
						PosX = widthDif;
					if (widthDif > 0)
						PercentageX = PosX / widthDif;
					else
						UpdateRec();
				}

				if (Vertical)
				{
					PosY += (int)pos.Y;
					var heightDif = Height - BarHeight;
					if (heightDif <= 0 || PosY < 0)
						PosY = 0;
					else if (heightDif > 0 && PosY > heightDif)
						PosY = heightDif;
					if (heightDif > 0)
						PercentageY = PosY / heightDif;
					else
						UpdateRec();
				}
			}

			// Drop
			if (ScrollableGame.CheckLeftReleased())
			{
				Draging = false;
				Hits = false;
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
				SpriteBatch.Draw(tex, Rect, null, color, Angle, Origin, Effects, 0);
			}

			base.Draw(gameTime);
		}
	}
}
