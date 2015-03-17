using System;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.IO;

#if WINDOWS8
using Windows.ApplicationModel.Resources.Core;
#elif ANDROID
using Java.Util;
#endif

namespace XNA8DFramework
{
	public static class ExtensionMethods
	{
		public static Texture2D PreMultiplyAlpha(this Texture2D tex)
		{
#if IPHONE
			return tex;
#else
			var data = new Color[tex.Width * tex.Height];
			tex.GetData(data);

			for (var i = 0; i < data.Length; i++)
			{
				var c = data[i];
#if SILVERLIGHT
				data[i] = new Color(c.R * c.A / 255f, c.G * c.A / 255f, c.B * c.A / 255f);
#else
				data[i] = new Color(c.R, c.G, c.B) * (c.A / 255f);
#endif
			}

			tex.SetData(data);
			return tex;
#endif
		}

		public static Texture2D AddColorKey(this Texture2D tex)
		{
			var data = new Color[tex.Width * tex.Height];
			tex.GetData(data);

			for (int i = 0; i < data.Length; i++)
			{
				Color c = data[i];
				if (c.R == 255 && c.G == 0 && c.B == 255)
				{
					data[i] = new Color(0, 0, 0, 0);
				}
			}

			tex.SetData(data);
			return tex;
		}

		public static Texture2D ToGrayScale(this Texture2D tex)
		{
			var texRet = new Texture2D(tex.GraphicsDevice, tex.Width, tex.Height);
			var data = new Color[tex.Width * tex.Height];
			tex.GetData(data);

			for (var i = 0; i < data.Length; i++)
			{
				var c = data[i];
				var grayScaleVal = (0.222f * c.R + 0.707f * c.G + 0.071f * c.B) / 255;
				data[i] = new Color(grayScaleVal, grayScaleVal, grayScaleVal);
			}

			texRet.SetData(data);
			return texRet;
		}

		public static Texture2D ToMagentaAlpha(this Texture2D tex)
		{
			var texRet = new Texture2D(tex.GraphicsDevice, tex.Width, tex.Height);
			var data = new Color[tex.Width * tex.Height];
			tex.GetData(data);

			for (int i = 0; i < data.Length; i++)
			{
				Color c = data[i];
				if (c.R == 255 && c.G == 0 && c.B == 255)
					c.A = 0;
				data[i] = c;
			}

			texRet.SetData(data);
			return texRet;
		}

		// DrawWrappedStringWidth

		public static void DrawWrappedStringWidth(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float maxLineWidth)
		{
			DrawWrappedStringWidth(spriteBatch, spriteFont, text, position, color, maxLineWidth, -1);
		}

		public static void DrawWrappedStringWidth(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float maxLineWidth, int maxLines)
		{
			Vector2 size;
			DrawWrappedStringWidth(spriteBatch, spriteFont, text, position, color, maxLineWidth, out size, maxLines);
		}

		public static void DrawWrappedStringWidth(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float maxLineWidth, out Vector2 size)
		{
			DrawWrappedStringWidth(spriteBatch, spriteFont, text, position, color, maxLineWidth, out size, -1);
		}

		public static void DrawWrappedStringWidth(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float maxLineWidth, out Vector2 size, int maxLines)
		{
			DrawWrappedStringWidth(spriteBatch, spriteFont, text, position, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0, maxLineWidth, out size, maxLines);
		}

		public static void DrawWrappedStringWidth(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, float maxLineWidth)
		{
			DrawWrappedStringWidth(spriteBatch, spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth, maxLineWidth, -1);
		}

		public static void DrawWrappedStringWidth(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, float maxLineWidth, int maxLines)
		{
			Vector2 size;
			DrawWrappedStringWidth(spriteBatch, spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth, maxLineWidth, out size, maxLines);
		}

		public static void DrawWrappedStringWidth(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, float maxLineWidth, out Vector2 size)
		{
			DrawWrappedStringWidth(spriteBatch, spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth, maxLineWidth, out size, -1);
		}

		public static void DrawWrappedStringWidth(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, float maxLineWidth, out Vector2 size, int maxLines)
		{
			DrawWrappedStringWidthHeight(spriteBatch, spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth, maxLineWidth, out size, maxLines, -1);
		}

		// DrawWrappedStringWidthHeight

		public static void DrawWrappedStringWidthHeight(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float maxLineWidth, int maxLines, float maxLineHeight)
		{
			Vector2 size;
			DrawWrappedStringWidthHeight(spriteBatch, spriteFont, text, position, color, maxLineWidth, out size, maxLines, maxLineHeight);
		}

		public static void DrawWrappedStringWidthHeight(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float maxLineWidth, out Vector2 size, int maxLines, float maxLineHeight)
		{
			DrawWrappedStringWidthHeight(spriteBatch, spriteFont, text, position, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0, maxLineWidth, out size, maxLines, maxLineHeight);
		}

		public static void DrawWrappedStringWidthHeight(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, float maxLineWidth, int maxLines, float maxLineHeight)
		{
			Vector2 size;
			DrawWrappedStringWidthHeight(spriteBatch, spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth, maxLineWidth, out size, maxLines, maxLineHeight);
		}

		public static void DrawWrappedStringWidthHeight(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, float maxLineWidth, out Vector2 size, int maxLines, float maxLineHeight)
		{
			if (string.IsNullOrEmpty(text))
			{
				size = Vector2.Zero;
				return;
			}

			string s;

			size = spriteFont.MeasureString(text, scale, maxLineWidth, maxLines, maxLineHeight, out s);

			spriteBatch.DrawString(spriteFont, s, position, color, rotation, origin, scale, effects, layerDepth);
		}

		public static Vector2 MeasureString(this SpriteFont spriteFont, string text, float scale, float maxLineWidth, int maxLines, float maxLineHeight, out string s)
		{
			if (string.IsNullOrEmpty(text))
			{
				s = text;
				return Vector2.Zero;
			}

			text = text.Replace("\r", "");

			var phrases = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var sb = new StringBuilder();

			float totalHeight = 0f;

			float spaceWidth = 0;
#if !SILVERLIGHT
			if (spriteFont.Characters.Contains(' '))
			{
#endif
				spaceWidth = spriteFont.MeasureString(" ").X * scale;
#if !SILVERLIGHT
			}
#endif

			var lineCount = 1;

			var exit = false;

			foreach (var phrase in phrases)
			{
				var words = phrase.Split(' ');

				var lineWidth = 0f;
				totalHeight += spriteFont.LineSpacing * scale;

				foreach (string word in words)
				{
					var sizeWord = spriteFont.MeasureString(word) * scale;

					if (maxLineWidth == -1 || lineWidth + sizeWord.X < maxLineWidth)
					{
						sb.Append(word);
						sb.Append(' ');
						lineWidth += sizeWord.X + spaceWidth;
					}
					else
					{
						lineWidth = sizeWord.X + spaceWidth;
						lineCount++;
						totalHeight += spriteFont.LineSpacing * scale;
						if ((maxLines != -1 && lineCount > maxLines) ||
							(maxLineHeight != -1 && totalHeight >= maxLineHeight))
						{
							exit = true;
							break;
						}
						if (sb.Length > 0)
							sb.Append('\n');
						sb.Append(word);
						sb.Append(' ');
					}
				}
				if (exit)
					break;
				sb.Append('\n');
				lineCount++;
				totalHeight += spriteFont.LineSpacing * scale;
				if ((maxLines != -1 && lineCount > maxLines) ||
					(maxLineHeight != -1 && totalHeight >= maxLineHeight))
				{
					//exit = true;
					break;
				}
			}

			if (sb.Length >= 1 && sb[0] == '\n')
				sb.Remove(0, 1);

			while (sb.Length >= 1 && sb[sb.Length - 1] == '\n')
				sb = sb.Remove(sb.Length - 1, 1);

			s = sb.ToString().Trim();

			return spriteFont.MeasureString(s) * scale;
		}

		public static void DrawStringAlignedLeft(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color)
		{
			DrawStringAlignedLeft(spriteBatch, spriteFont, text, position, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		}

		public static void DrawStringAlignedLeft(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			Vector2 dif = spriteFont.MeasureString(text);
			spriteBatch.DrawString(spriteFont, text, position + new Vector2(-dif.X * scale, 0), color, rotation, origin, scale, effects, layerDepth);
		}

		public static T LoadLocalized<T>(this ContentManager content, string assetName)
		{
#if ANDROID
			assetName = assetName.Replace('\\', Path.DirectorySeparatorChar);
#endif

#if WINDOWS8 || ANDROID
			string root = Path.GetDirectoryName(assetName);
			string fileName = Path.GetFileName(assetName);
#if WINDOWS8
			string subTreePath = Path.Combine(content.RootDirectory, root, fileName) + ".xnb";

			var resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Files");
			var resourceCandidate = resourceMap.GetValue(subTreePath);
			if (resourceCandidate.Qualifiers.Count > 0)
			{
				var localization = resourceCandidate.Qualifiers[0].QualifierValue;
				assetName = Path.Combine(root, localization, fileName);
			}
#elif ANDROID
			var localization = (Locale.Default.Language + "-" + Locale.Default.Country).ToLower();

			string subTreePath = Path.Combine(content.RootDirectory, root, localization, fileName) + ".xnb";

			//var fileDescriptor = Game.Activity.Assets.List(Path.Combine(content.RootDirectory, root));

			assetName = Path.Combine(root, localization, fileName);
#endif
#endif
			return content.Load<T>(assetName);
		}

		public static SpriteFont Fix(this SpriteFont spriteFont)
		{
#if SILVERLIGHT
			if (spriteFont.FontSize == (double)(14.0f * 4f / 3f))
			{
				spriteFont.LineSpacing = 22;
			}
			else if (spriteFont.FontSize == (double)(24.0f * 4f / 3f))
			{
				spriteFont.LineSpacing = 37;
			}
#endif
			return spriteFont;
		}

		/// <summary>
		/// Converts a Stream (from file or memory or anything) into a Texture2D. Automatically calls PreMultiplyAlpha
		/// </summary>
		/// <param name="stream">The stream to be converted.</param>
		/// <param name="graphicsDevice">A graphics device manager to create the Texture2D from.</param>
		/// <returns>Retorna a Texture2D criada a partir do stream do parâmetro do método.</returns>
		public static Texture2D ToTexture2D(this Stream stream, GraphicsDevice graphicsDevice)
		{
#if WINDOWS || WINDOWS8 || IPHONE || ANDROID || WINDOWS_PHONE
			var texture = Texture2D.FromStream(graphicsDevice, stream).PreMultiplyAlpha();
#elif SILVERLIGHT
			var texture = new Texture2D(stream, graphicsDevice, new Vector2(0, 0));
#endif
			return texture;
		}
	}
}
