using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameStateManagement
{
	public class MenuCheckBoxEntry : MenuEntry
	{
		public Texture2D TextureBack { get; set; }
		public Texture2D TextureChecked { get; set; }
		public string Text { get; set; }

		public bool Checked { get; set; }

		protected int Spacing = 10;

		public Color TextColor { get; set; }

		public MenuCheckBoxEntry(Texture2D textureBack, Texture2D textureChecked, string text)
		{
			TextureBack = textureBack;
			TextureChecked = textureChecked;
			Text = text;
			TextColor = Color.White;
			Checked = false;
		}

		public MenuCheckBoxEntry(Texture2D textureBack, Texture2D textureChecked, string text, Vector2 startPosition)
			: base(startPosition)
		{
			TextureBack = textureBack;
			TextureChecked = textureChecked;
			Text = text;
			TextColor = Color.White;
			Checked = false;
		}

		public override void Draw(MenuScreen screen, GameTime gameTime)
		{
			if (!Visible)
				return;

			// Pulsate the size of the selected menu entry.
			double time = gameTime.TotalGameTime.TotalSeconds;

			var pulsate = (float)Math.Sin(time * 6) + 1;

			var scale = Scale + pulsate * 0.05f * SelectionFade;

#if SILVERLIGHT
			var color = new Color(Color, (Color.A / 255.0f) * screen.TransitionAlpha * (float)Alpha);
#else
			var color = Color * screen.TransitionAlpha * (float)Alpha;
#endif

#if SILVERLIGHT
			var textColor = new Color(TextColor, MathHelper.Clamp((TextColor.A / 255.0f) * screen.TransitionAlpha * (float)Alpha, 0, 1));
#else
			var textColor = TextColor * screen.TransitionAlpha * (float)Alpha;
#endif

			// Draw text, centered on the middle of each line.
			ScreenManager screenManager = screen.ScreenManager;
			SpriteBatch spriteBatch = screenManager.SpriteBatch;
			SpriteFont font = screenManager.Font;

			spriteBatch.Draw(TextureBack, position + Origin, null, color, Angle,
								   Origin, scale, Effect, 0);
			if (Checked)
			{
				spriteBatch.Draw(TextureChecked, position + Origin, null, color, Angle,
					   Origin, scale, Effect, 0);
			}
			Vector2 size = font.MeasureString(Text);
			spriteBatch.DrawString(font, Text, position + Origin + new Vector2(TextureBack.Width + Spacing, TextureBack.Height / 2), textColor, Angle,
								   Origin + new Vector2(0, size.Y / 2), scale, Effect, 0);
		}

		protected internal override void OnSelectEntry()
		{
			Checked = !Checked;
			base.OnSelectEntry();
		}

		public override int GetHeight(MenuScreen screen)
		{
			return Math.Max(TextureBack.Height, screen.ScreenManager.Font.LineSpacing);
		}

		public override int GetWidth(MenuScreen screen)
		{
			return TextureBack.Width + Spacing + (int)screen.ScreenManager.Font.MeasureString(Text).X;
		}
	}
}
