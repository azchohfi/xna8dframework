using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Primitives
{
	public class Primitives : DrawableGameComponent
	{
		Texture2D _blankTexture;

		SpriteBatch _spriteBatch;

		public Primitives(Game game)
			: base(game)
		{
		}

	    protected override void LoadContent()
		{
			_spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

			_blankTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
			_blankTexture.SetData(new[] { Color.White });

			base.LoadContent();
		}

	    public void DrawLine(Vector2 vector1, Vector2 vector2, Color color)
		{
			DrawLine(vector1, vector2, color, 1);
		}

		public void DrawLine(Vector2 vector1, Vector2 vector2, Color color, float thickness)
		{
			// calculate the distance between the two vectors
			var distance = Vector2.Distance(vector1, vector2) + 1;

			// calculate the angle between the two vectors
			var angle = (float)Math.Atan2(vector2.Y - vector1.Y, vector2.X - vector1.X);

			// stretch the pixel between the two vectors
			_spriteBatch.Draw(_blankTexture,
				vector1,
				null,
				color,
				angle,
				new Vector2(0, 0),
				new Vector2(distance, thickness),
				SpriteEffects.None,
				0);
		}

		public void DrawCircle(Vector2 centerPosition, float radius, int sides, Color color)
		{
			var vectors = new List<Vector2>();
			const float max = 2 * (float)Math.PI;
			float step = max / sides;

			for (float theta = 0; theta < max; theta += step)
			{
				vectors.Add(centerPosition + new Vector2(radius * (float)Math.Cos(theta),
							radius * (float)Math.Sin(theta)));
			}

			// then add the first vector again so it's a complete loop
			vectors.Add(centerPosition + new Vector2(radius * (float)Math.Cos(0),
							radius * (float)Math.Sin(0)));

			for (int i = 1; i < vectors.Count; i++)
			{
				var vector1 = vectors[i - 1];
				var vector2 = vectors[i];
				DrawLine(vector1, vector2, color, 2);
			}
		}

		public void DrawFilledRectangle(Rectangle rect, Color fillColor, Color borderColor)
		{
			DrawFilledRectangle(rect, fillColor, borderColor, 1);
		}

		public void DrawFilledRectangle(Rectangle rect, Color fillColor, Color borderColor, float border)
		{
			// Draw the background rectangle.
			_spriteBatch.Draw(_blankTexture, rect, fillColor);
			if (border > 0 && borderColor.A > 0)
			{
				var pLt = new Vector2(rect.Left, rect.Top);
				var pRt = new Vector2(rect.Right, rect.Top);
				var pLb = new Vector2(rect.Left, rect.Bottom);
				var pRb = new Vector2(rect.Right, rect.Bottom);
				DrawLine(pLt, pRt, borderColor, border);
				DrawLine(pRt, pRb, borderColor, border);
				DrawLine(pRb, pLb, borderColor, border);
				DrawLine(pLb, pLt, borderColor, border);
			}
		}
	}
}
