using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.Animations;

namespace XNA8DFramework.UI
{
	public class StackPanel : DrawableGameComponent, IScrollableCullable
	{
		public Vector2 Position { get; set; }

		public Color Color { get; set; }

		public double Alpha { get; set; }

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

		public List<IDrawable8D> Drawables { get; private set; }

		public float SpacingX { get; set; }
		public float SpacingY { get; set; }

		public StackPanel(Game game, Vector2 position)
			: base(game)
		{
			Color = Color.White;
			Alpha = 1;
			Scale = 1;
			Position = position;
			SpacingX = 0;
			SpacingY = 0;
			Drawables = new List<IDrawable8D>();
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
		}

		public override void Update(GameTime gameTime)
		{
			for (var i = 0; i < Drawables.Count; i++)
			{
				Drawables[i].Update(gameTime);
			}

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			DrawLimited(gameTime, 0, this.Height);

			base.Draw(gameTime);
		}

		public void DrawLimited(GameTime gameTime, float posDif, float limit)
		{
			var pos = Position;
			var scrollSize = 0;
			var maxSize = 0;
			for (var i = 0; i < Drawables.Count; i++)
			{
				Drawables[i].Position = pos;
				Drawables[i].Alpha = Alpha;

				// Culling
				if (posDif + Position.Y - Drawables[i].Height <= Drawables[i].Position.Y &&
					Drawables[i].Position.Y <= posDif + Position.Y + limit)
					Drawables[i].Draw(gameTime);

				pos.X += Drawables[i].Width + SpacingX;
				if (Drawables[i].Height > maxSize)
					maxSize = Drawables[i].Height;
				if (pos.X - Position.X > Width || i == Drawables.Count - 1)
				{
					pos.X = Position.X;
					pos.Y += maxSize + SpacingY;
					scrollSize += maxSize;
					maxSize = 0;
				}
			}
			_ScrollHeight = scrollSize;
			_ScrollWidth = scrollSize;
		}

		public int Width { get; set; }

		public int Height { get; set; }

		protected float _ScrollHeight;
		public int ScrollHeight()
		{
			return (int)_ScrollHeight;
		}

		protected float _ScrollWidth;
		public int ScrollWidth()
		{
			return (int)_ScrollWidth;
		}
	}
}
