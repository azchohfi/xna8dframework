using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement
{
	public class MenuImageEntry : MenuEntry
	{
		public Texture2D Texture { get; set; }

		public MenuImageEntry(Texture2D texture)
		{
			Texture = texture;
		}

		public MenuImageEntry(Texture2D texture, Vector2 startPosition)
			: base(startPosition)
		{
			Texture = texture;
		}

		public override void Draw(MenuScreen screen, GameTime gameTime)
		{
			if (!Visible)
				return;

			// Pulsate the size of the selected menu entry.
			var time = gameTime.TotalGameTime.TotalSeconds;

			var pulsate = (float)Math.Sin(time * 6) + 1;

			var scale = Scale + pulsate * 0.05f * SelectionFade;

#if SILVERLIGHT
			var color = new Color(Color, MathHelper.Clamp((Color.A / 255.0f) * screen.TransitionAlpha * (float)Alpha, 0, 1));
#else
			var color = Color * screen.TransitionAlpha * (float)Alpha;
#endif

			// Draw text, centered on the middle of each line.
			ScreenManager screenManager = screen.ScreenManager;
			SpriteBatch spriteBatch = screenManager.SpriteBatch;

			spriteBatch.Draw(Texture, position + Origin, null, color, Angle,
								   Origin, scale, Effect, 0);
		}

		public override int GetHeight(MenuScreen screen)
		{
			return Texture.Height;
		}

		public override int GetWidth(MenuScreen screen)
		{
			return Texture.Width;
		}
	}
}
