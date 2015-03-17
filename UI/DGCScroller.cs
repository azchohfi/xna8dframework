using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.Animations;

namespace XNA8DFramework.UI
{
	public class DGCScroller : DrawableGameComponent, IScrollableCullable
	{
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
				PosicionarBotoes();
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

		public Color Color { get; set; }

		public double Alpha { get; set; }

		protected SpriteBatch SpriteBatch;

		public List<IDrawable8D> Drawables { get; private set; }

		protected Texture2D BackgroundTexture;
		protected Texture2D BtnUpTexture;

		protected Button BtnUp;
		protected Button BtnDown;

		public int BaseIndex { get; set; }
		public int ShowCount { get; private set; }

		public bool AlignCenter { get; set; }

		public bool Horizontal { get; set; }

		public DGCScroller(Game game, Texture2D backgroundTexture, Texture2D btnUpTexture, int showCount)
			: base(game)
		{
			Color = Color.White;
			Alpha = 1;
			Scale = 1;
			BaseIndex = 0;
			ShowCount = showCount;
			AlignCenter = false;
			Horizontal = false;

			Drawables = new List<IDrawable8D>();

			BackgroundTexture = backgroundTexture;
			BtnUpTexture = btnUpTexture;
		}

		public void AddItem(IDrawable8D drawable)
		{
			Drawables.Add(drawable);
		}

		public void Remove(IDrawable8D drawable)
		{
			Drawables.Remove(drawable);
		}

		public void Clear()
		{
			Drawables.Clear();
			BaseIndex = 0;
		}

		public override void Initialize()
		{
			SpriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

			BtnUp = new Button(Game, BtnUpTexture, Vector2.Zero);
			BtnUp.Click += btnUp_Click;
			BtnUp.Initialize();

			BtnDown = new Button(Game, BtnUpTexture, Vector2.Zero);
			BtnDown.Click += btnDown_Click;
			BtnDown.Initialize();

			if (Horizontal)
			{
				BtnUp.Origin = new Vector2(0, BtnUp.Height / 2);
				BtnDown.Effects = SpriteEffects.FlipHorizontally;
				BtnDown.Origin = new Vector2(0, BtnDown.Height / 2);
			}
			else
			{
				BtnUp.Origin = new Vector2(BtnUp.Width / 2, 0);
				BtnDown.Effects = SpriteEffects.FlipVertically;
				BtnDown.Origin = new Vector2(BtnDown.Width / 2, 0);
			}

			PosicionarBotoes();

			base.Initialize();
		}

		protected virtual void PosicionarBotoes()
		{
			if (Horizontal)
			{
				if (BtnUp != null)
					BtnUp.Position = Position + new Vector2(0, Height / 2) - BtnUp.Origin;
				if (BtnDown != null)
					BtnDown.Position = Position + new Vector2(Width - BtnDown.Width, Height / 2) - BtnDown.Origin;
			}
			else
			{
				if (BtnUp != null)
					BtnUp.Position = Position + new Vector2(Width / 2, 0) - BtnUp.Origin;
				if (BtnDown != null)
					BtnDown.Position = Position + new Vector2(Width / 2, Height - BtnDown.Height) - BtnDown.Origin;
			}
		}

		void btnUp_Click(object sender, EventArgs e)
		{
			if (BaseIndex > 0)
				BaseIndex--;
		}

		void btnDown_Click(object sender, EventArgs e)
		{
			if (BaseIndex + ShowCount < Drawables.Count)
				BaseIndex++;
		}

		public override void Update(GameTime gameTime)
		{
			if (BaseIndex > 0)
				BtnUp.Update(gameTime);
			if (BaseIndex + ShowCount < Drawables.Count)
				BtnDown.Update(gameTime);

			int count = Math.Min(Drawables.Count, BaseIndex + ShowCount);
			for (int i = BaseIndex; i < count; i++)
			{
				Drawables[i].Update(gameTime);
			}

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			DrawLimited(gameTime, 0, Height);

			base.Draw(gameTime);
		}

		public void DrawLimited(GameTime gameTime, float posDif, float limit)
		{
#if SILVERLIGHT
			var color = new Color(Color, MathHelper.Clamp((float)Alpha, 0, 1));
#else
			var color = Color * (float)Alpha;
#endif

			if (BackgroundTexture != null)
				SpriteBatch.Draw(BackgroundTexture, Position, color);

			Vector2 pos;
			if (Horizontal)
			{
				pos = Position + new Vector2(BtnUp.Width, Height / 2);
			}
			else
			{
				pos = Position + new Vector2(Width / 2, BtnUp.Height);
			}
			var count = Math.Min(Drawables.Count, BaseIndex + ShowCount);
			for (var i = BaseIndex; i < count; i++)
			{
				if (Horizontal)
				{
					if (AlignCenter)
						Drawables[i].Position = pos - new Vector2(0, Drawables[i].Origin.Y);
					else
						Drawables[i].Position = pos - new Vector2(0, Height / 2);
				}
				else
				{
					if (AlignCenter)
						Drawables[i].Position = pos - new Vector2(Drawables[i].Origin.X, 0);
					else
						Drawables[i].Position = pos - new Vector2(Width / 2, 0);
				}
				Drawables[i].Alpha = Alpha;

				// Culling
				if (limit == 0 || (
					(Horizontal && posDif + Position.X - Drawables[i].Width <= Drawables[i].Position.X &&
						Drawables[i].Position.X <= posDif + Position.X + limit) || 
					(!Horizontal && posDif + Position.Y - Drawables[i].Height <= Drawables[i].Position.Y &&
						Drawables[i].Position.Y <= posDif + Position.Y + limit)))
					Drawables[i].Draw(gameTime);

				if (Horizontal)
					pos.X += Drawables[i].Width;
				else
					pos.Y += Drawables[i].Height;
			}

			if (BaseIndex > 0)
			{
				BtnUp.Alpha = Alpha;
				BtnUp.Draw(gameTime);
			}

			if (BaseIndex + ShowCount < Drawables.Count)
			{
				BtnDown.Alpha = Alpha;
				BtnDown.Draw(gameTime);
			}
		}

		private int _width;
		public int Width
		{
			get
			{
				if (BackgroundTexture != null)
					return BackgroundTexture.Width;
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		private int _height;
		public int Height
		{
			get
			{
				if (BackgroundTexture != null)
					return BackgroundTexture.Height;
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		public int ScrollHeight()
		{
			return Drawables.Sum(drawable => drawable.Height);
		}

		public int ScrollWidth()
		{
			return Drawables.Sum(drawable => drawable.Width);
		}
	}
}
