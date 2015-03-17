using System;
using System.Linq;
using Microsoft.Xna.Framework;
using XNA8DFramework.Animations;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.UI
{
	public class ScrollableRenderTarget : DrawableGameComponent, IDrawable8D
	{
		protected SpriteBatch SpriteBatch;

		private IScrollable _scrollable;
		public IScrollable Scrollable
		{
			get
			{
				return _scrollable;
			}
			set
			{
				_scrollable = value;
				UpdateContentSize();
				Percentage = 0;
			}
		}

		public bool Horizontal { get; set; }

		bool _draging;
		Vector2 _draggedDif;
		Vector2 _draggedDifInicial;
		Rectangle _rect, _rectDraw;
		private float PosDif { get; set; }
		protected float _Percentage;
		public float Percentage
		{
			get
			{
				return _Percentage;
			}
			set
			{
				_Percentage = value;

				if (Horizontal)
				{
					var widthDif = ContentSize - Width;
					PosDif = widthDif * _Percentage;

					if (widthDif <= 0 || PosDif < 0)
						PosDif = 0;
					else if (widthDif > 0 && PosDif > widthDif)
						PosDif = widthDif;

					if (_scroll != null)
					{
						_scroll.PercentageX = Percentage;
					}
				}
				else
				{
					var heightDif = ContentSize - Height;
					PosDif = heightDif*_Percentage;

					if (heightDif <= 0 || PosDif < 0)
						PosDif = 0;
					else if (heightDif > 0 && PosDif > heightDif)
						PosDif = heightDif;

					if (_scroll != null)
					{
						_scroll.PercentageY = Percentage;
					}
				}
			}
		}

		protected Vector2 _Position;
		public Vector2 Position
		{
			get
			{
				return _Position;
			}
			set
			{
				_Position = value;
				UpdateRect();
			}
		}

		public float Angle { get; set; }

		public Vector2 Origin { get; set; }

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

		public virtual Vector2 Scale2D { get; set; }

		public double Alpha { get; set; }

		public Color Color { get; set; }

		public Color BackgroundColor { get; set; }

		public bool Culling { get; set; }

		public Matrix MatrixInternal { get; set; }
		public Matrix Matrix { get; set; }

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

		public event EventHandler ContentSizeChanged;

		private int _contentSize;
		public int ContentSize
		{
			get
			{
				return _contentSize;
			}
			private set
			{
				_contentSize = value;
				if (ContentSizeChanged != null)
					ContentSizeChanged(this, EventArgs.Empty);
			}
		}

		RenderTarget2D _renderTarget;

		readonly IScroll _scroll;

		public ScrollableRenderTarget(Game game, int width, int height, IScrollable scrollable)
			: this(game, width, height, scrollable, null)
		{
		}

		public ScrollableRenderTarget(Game game, int width, int height, IScrollable scrollable, IScroll scroll)
			: base(game)
		{
			_Width = width;
			_Height = height;
			_scrollable = scrollable;
			_scroll = scroll;
			if (_scroll != null)
			{
				_scroll.ScrollContainer = this;
			}
			Scale = 1;
			Alpha = 1;
			Color = Color.White;
			Culling = true;
#if SILVERLIGHT
			BackgroundColor = Color.TransparentWhite;
#else
			BackgroundColor = Color.Transparent;
#endif

			PosDif = 0;
			MatrixInternal = Matrix.Identity;
			Matrix = Matrix.Identity;
		}

		public override void Initialize()
		{
			base.Initialize();

			if (_scrollable != null)
				ContentSize = Horizontal ? _scrollable.ScrollWidth() : _scrollable.ScrollHeight();
			else
				ContentSize = 0;

			SpriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

			Percentage = 0;

			if (_scroll != null)
				_scroll.Initialize();

			var graphicsDeviceManager = Game.Services.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager;

			InitRenderTarget(graphicsDeviceManager);

#if !SILVERLIGHT
			if (graphicsDeviceManager != null)
				graphicsDeviceManager.DeviceReset += OnDeviceReset;
#endif

			UpdateRect();
		}

		private void InitRenderTarget(GraphicsDeviceManager graphics)
		{
			//var height = this.GraphicsDevice.PresentationParameters.BackBufferHeight;
			var height = graphics.PreferredBackBufferHeight;
			if (height == 0)
				height = _Height;

#if !SILVERLIGHT
			if (_renderTarget != null)
				_renderTarget.Dispose();
#endif

#if SILVERLIGHT
			_renderTarget = new RenderTarget2D(this.GraphicsDevice, _Width, height, 1, this.GraphicsDevice.PresentationParameters.BackBufferFormat, RenderTargetUsage.DiscardContents);
#elif WINDOWS || WINDOWS_PHONE || ANDROID
			if (Game.GraphicsDevice.GraphicsProfile == GraphicsProfile.Reach && height > 2048)
				height = 2048;

			_renderTarget = new RenderTarget2D(GraphicsDevice, _Width, height, false,
								GraphicsDevice.PresentationParameters.BackBufferFormat,
								DepthFormat.None,
								0,
								RenderTargetUsage.PreserveContents);
#elif IPHONE
			_renderTarget = new RenderTarget2D(GraphicsDevice, (int)(2048), (int)(2048), false,
											  GraphicsDevice.PresentationParameters.BackBufferFormat,
											  DepthFormat.None,
											  0,
											  RenderTargetUsage.PreserveContents);
#endif
		}

#if !SILVERLIGHT
		private void OnDeviceReset(object sender, EventArgs eventArgs)
		{
			InitRenderTarget(sender as GraphicsDeviceManager);
		}
#endif

		protected void UpdateRect()
		{
			_rect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
		}

#if !SILVERLIGHT
		protected override void Dispose (bool disposing)
		{
			if (_renderTarget != null && !_renderTarget.IsDisposed)
				_renderTarget.Dispose();
			var graphicsDeviceManager = Game.Services.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager;
			if (graphicsDeviceManager != null)
				graphicsDeviceManager.DeviceReset -= OnDeviceReset;
			base.Dispose (disposing);
		}
#endif

		public override void Update(GameTime gameTime)
		{
			if (!Enabled)
				return;
		
			bool scrollDrag = false;

			if (_scroll != null)
			{
				_scroll.Update(gameTime);
				scrollDrag = _scroll.Draging || _scroll.Hits;
			}

			if (!scrollDrag)
			{
				// Select for Drag
				if (ScrollableGame.CheckLeftPressed(true))
				{
					if (_rect.Contains(ScrollableGame.MousePoint))
					{
						_draging = true;
						_draggedDifInicial = _draggedDif = ScrollableGame.MousePos;
					}
				}

				// Dragging 
				if (_draging && ScrollableGame.CheckLeftDown())
				{
					if (Horizontal)
					{
						var pos = ScrollableGame.MousePos.X - _draggedDif.X;
						_draggedDif = ScrollableGame.MousePos;
						PosDif -= (int)pos;

						if (Math.Abs(ScrollableGame.MousePos.X - _draggedDifInicial.X) > 5)
						{
							Button.CancelClick = true;
						}

						var widthDif = ContentSize - Width;

						if (widthDif <= 0 || PosDif < 0)
							PosDif = 0;
						else if (widthDif > 0 && PosDif > widthDif)
							PosDif = widthDif;

						if (widthDif > 0)
							Percentage = PosDif / widthDif;
					}
					else
					{
						var pos = ScrollableGame.MousePos.Y - _draggedDif.Y;
						_draggedDif = ScrollableGame.MousePos;
						PosDif -= (int) pos;

						if (Math.Abs(ScrollableGame.MousePos.Y - _draggedDifInicial.Y) > 5)
						{
							Button.CancelClick = true;
						}

						var heightDif = ContentSize - Height;

						if (heightDif <= 0 || PosDif < 0)
							PosDif = 0;
						else if (heightDif > 0 && PosDif > heightDif)
							PosDif = heightDif;

						if (heightDif > 0)
							Percentage = PosDif/heightDif;
					}
				}

				// Drop
				if (ScrollableGame.CheckLeftReleased()
#if SILVERLIGHT
					|| !_rect.Contains(ScrollableGame.MousePoint)
#endif
)
				{
					_draging = false;
#if SILVERLIGHT
					Microsoft.Xna.Framework.Input.Mouse.LeftButtonDown = false;
#endif
				}

				ScrollableGame.Begin(this);

				if (_scrollable != null)
					_scrollable.Update(gameTime);

				ScrollableGame.End(this);
			}

			UpdateContentSize();

			base.Update(gameTime);
		}

		public void UpdateContentSize()
		{
			if (_scrollable == null)
			{
				ContentSize = 0;
				return;
			}
			int size = Horizontal ? _scrollable.ScrollWidth() : _scrollable.ScrollHeight();
			if (ContentSize != size)
			{
				ContentSize = size;
			}
		}

		public override void Draw(GameTime gameTime)
		{
			if (!Visible)
				return;
		
#if SILVERLIGHT
			var oldRenderTarget = GraphicsDevice.GetRenderTarget(0) as RenderTarget2D;
			this.GraphicsDevice.SetRenderTarget(0, _renderTarget);

			// Fix para GraphicsDevice.Clear que não limpa com Transparent
			Array.Clear(_renderTarget.GetTexture().ImageSource.Pixels, 0, _renderTarget.GetTexture().ImageSource.Pixels.Length);
#else
			RenderTarget2D oldRenderTarget = null;
			if (GraphicsDevice.GetRenderTargets().Count() > 0)
				oldRenderTarget = GraphicsDevice.GetRenderTargets()[0].RenderTarget as RenderTarget2D;
			GraphicsDevice.SetRenderTarget(_renderTarget);
#endif

			GraphicsDevice.Clear(BackgroundColor);

#if SILVERLIGHT
			SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, MatrixInternal);
#else
			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, MatrixInternal);
#endif


			int size = Math.Min(_contentSize, Horizontal ? _Width : _Height);

			if (_scrollable != null)
			{
				_scrollable.Alpha = Alpha;
				_scrollable.Position = Horizontal ? new Vector2(-PosDif, 0) : new Vector2(0, -PosDif);
				var scrollableCullable = _scrollable as IScrollableCullable;
				if (Culling && scrollableCullable != null)
					scrollableCullable.DrawLimited(gameTime, PosDif, size);
				else
					_scrollable.Draw(gameTime);
			}

			SpriteBatch.End();

#if SILVERLIGHT
			GraphicsDevice.SetRenderTarget(0, oldRenderTarget);
			SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, Matrix);

			var color = new Color(Color, MathHelper.Clamp((float)Alpha, 0, 1));
			Texture2D tex = _renderTarget.GetTexture();
#else
			GraphicsDevice.SetRenderTarget(oldRenderTarget);
			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix);

			var color = Color * (float)Alpha;
			Texture2D tex = _renderTarget;
#endif

			if (Horizontal)
			{
				_rectDraw.Width = size;
				_rectDraw.Height = _Height;
			}
			else
			{
				_rectDraw.Width = _Width;
				_rectDraw.Height = size;
			}

			SpriteBatch.Draw(tex, Position, _rectDraw, color);

			if (_scroll != null)
			{
				_scroll.Alpha = Alpha;
				_scroll.Draw(gameTime);
			}

			SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
