using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement
{
	public class MenuTextEntry : MenuEntry
	{
		/// <summary>
		/// The text rendered for this entry.
		/// </summary>
		string _text;

		/// <summary>
		/// Gets or sets the text of this menu entry.
		/// </summary>
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		/// <summary>
		/// Constructs a new menu entry with the specified text.
		/// </summary>
		public MenuTextEntry(string text)
		{
			_text = text;
		}

		public MenuTextEntry(string text, Vector2 startPosition)
			: base(startPosition)
		{
			_text = text;
		}

		public override void Draw(MenuScreen screen, GameTime gameTime)
		{
			// Pulsate the size of the selected menu entry.
			double time = gameTime.TotalGameTime.TotalSeconds;
			
			float pulsate = (float)Math.Sin(time * 6) + 1;

			var scale = Scale + pulsate * 0.05f * SelectionFade;

#if SILVERLIGHT
			var color = new Color(Color, MathHelper.Clamp((Color.A / 255.0f) * screen.TransitionAlpha * (float)Alpha, 0, 1));
#else
			var color = Color * screen.TransitionAlpha * (float)Alpha;
#endif

			// Draw text, centered on the middle of each line.
			ScreenManager screenManager = screen.ScreenManager;
			SpriteBatch spriteBatch = screenManager.SpriteBatch;
			SpriteFont font = screenManager.Font;

			spriteBatch.DrawString(font, _text, position + Origin, color, Angle,
								   Origin, scale, Effect, 0);
		}

		public override int GetHeight(MenuScreen screen)
		{
			return (int)(screen.ScreenManager.Font.LineSpacing * Scale);
		}

		public override int GetWidth(MenuScreen screen)
		{
			return (int)(screen.ScreenManager.Font.MeasureString(Text).X * Scale);
		}
	}
}
