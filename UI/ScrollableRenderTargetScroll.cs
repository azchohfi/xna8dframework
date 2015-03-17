using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.UI
{
	public class ScrollableRenderTargetScroll : DrawableGameComponent, IScroll
	{
		public event EventHandler PercentageChanged;
		private ScrollableRenderTarget _scrollContainer;
		public ScrollableRenderTarget ScrollContainer
		{
			get
			{
				return _scrollContainer;
			}
			set
			{
				_scrollContainer = value;
				_barraDown.Height = Height;
				UpdateRec();
				_scrollContainer.ContentSizeChanged += ScrollContainerContentSizeChanged;
			}
		}

		void ScrollContainerContentSizeChanged(object sender, EventArgs e)
		{
			if (_scrollContainer.ContentSize != 0)
			{
				var prop = Height / (float)_scrollContainer.ContentSize;

				if (prop < 1)
				{
					BarHeight = (int)(prop * Height);
					if (BarHeight < 20)
						BarHeight = 20;
				}
				else
				{
					BarHeight = Height;
				}
			}
			else
			{
				BarHeight = Height;
			}
			_barraUp.Height = BarHeight;
		}

		public void UpdateRec()
		{
			_rec2.X = (int)Position.X;
			_rec2.Y = (int)Position.Y;
			_rec2.Width = BarWidth;
			_rec2.Height = Height;

			_rec.X = (int)(Position.X);
			_rec.Y = (int)(Position.Y + PosY);
			_rec.Width = BarWidth;
			_rec.Height = BarHeight;
		}

		private Rectangle _rec2;

		private Rectangle _rec;
		public Rectangle Rect
		{
			get
			{
				return _rec;
			}
		}

		public Vector2 Position
		{
			get
			{
				if (ScrollContainer != null)
				{
					return ScrollContainer.Position + new Vector2(ScrollContainer.Width - BarWidth, 0) + PosDif;
				}
				return Vector2.Zero;
			}
			set
			{
				UpdateRec ();
			}
		}
		public float Angle { get; set; }
		public Vector2 Origin { get; set; }
		public double Alpha
		{
			get
			{
				return _barraUp.Alpha;
			}
			set
			{
				_barraUp.Alpha = value;
				_barraDown.Alpha = value;
			}
		}
		public Color Color
		{
			get
			{
				return _barraUp.Color;
			}
			set
			{
				_barraUp.Color = value;
				_barraDown.Color = value;
			}
		}

		public int Width
		{
			get
			{
				return BarWidth;
			}
		}

		public int Height
		{
			get
			{
				if (ScrollContainer != null)
					return ScrollContainer.Height;
				return 0;
			}
		}

		public bool Horizontal
		{
			get
			{
				return false;
			}
		}

		public bool Vertical
		{
			get
			{
				return true;
			}
		}

		public int BarWidth
		{
			get
			{
				return 6;
			}
		}

		public int BarHeight { get; protected set; }

		public bool Hits { get; protected set; }
		public bool Draging { get; protected set; }
		Vector2 _draggedDif;

		public float PercentageX { get { return 0; } set { } }

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

				if (PercentageChanged != null)
					PercentageChanged(this, EventArgs.Empty);
			}
		}

		readonly SpeechBubble _barraUp;
		readonly SpeechBubble _barraDown;

		public Vector2 PosDif { get; set; }

		public ScrollableRenderTargetScroll(Game game, Texture2D textureUp, Texture2D textureDown)
			: this(game, Vector2.Zero, textureUp, textureDown)
		{
		}

		public ScrollableRenderTargetScroll(Game game, Vector2 posDif, Texture2D textureUp, Texture2D textureDown)
			: base(game)
		{
			PosDif = posDif;

			_barraUp = new SpeechBubble(Game, textureUp, Position, "", null, BarWidth, BarHeight, 2);
			_barraUp.Initialize();

			_barraDown = new SpeechBubble(Game, textureDown, Position, "", null, BarWidth, Height, 2);
			_barraDown.Initialize();
		}

		public override void Initialize()
		{
			base.Initialize();

			UpdateRec();

			_barraUp.Position = Position;
			_barraDown.Position = Position;
		}

		public override void Update(GameTime gameTime)
		{
			if (!Enabled || !Game.IsActive)
				return;

			UpdateRec();

			// Select for Drag
			if (ScrollableGame.CheckLeftPressed(true))
			{
				if (Rect.Contains(ScrollableGame.MousePoint))
				{
					Draging = true;
					_draggedDif = ScrollableGame.MousePos;
				}
				if (_rec2.Contains(ScrollableGame.MousePoint))
				{
					Hits = true;
				}
			}

			// Dragging 
			if (Draging && ScrollableGame.CheckLeftDown())
			{
				var pos = ScrollableGame.MousePos - _draggedDif;
				_draggedDif = ScrollableGame.MousePos;

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
					UpdateRec();

					ScrollContainer.Percentage = PercentageY;
				}
			}

			// Drop
			if (ScrollableGame.CheckLeftReleased())
			{
				Draging = false;
				Hits = false;
			}

			_barraUp.Position = Position + new Vector2(0, PosY);
			_barraDown.Position = Position;

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (!Visible)
				return;

			if (BarHeight != Height)
			{
				_barraDown.Draw(gameTime);
				_barraUp.Draw(gameTime);
			}

			base.Draw(gameTime);
		}
	}
}
