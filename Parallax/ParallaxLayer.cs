using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using XNA8DFramework.Animations;

namespace XNA8DFramework.Parallax
{
	public class ParallaxLayer : DrawableGameComponent, IAlphaAnimatable, IVector2Animatable
	{
		public String TextureFileName;

		public double Alpha { get; set; }

		public Vector2 Position { get; set; }

		public Texture2D Textura
		{
			get
			{
				if (Texturas != null && Texturas.Any())
					return Texturas[0];
				return null;
			}
			set
			{
				if (Texturas != null && Texturas.Any())
					Texturas[0] = value;
			}
		}

		public Texture2D[] Texturas = null;
		int _texturasIndex;
		int TexturasIndex
		{
			get
			{
				return _texturasIndex;
			}
			set
			{
				_texturasIndex = value;
				if (_texturasIndex < 0)
					_texturasIndex = Texturas.Count() - 1;
				else if (_texturasIndex > Texturas.Count() - 1)
					_texturasIndex = 0;
			}
		}

		internal Parallax Parallax = null;

		SpriteBatch _spriteBatch;

		public float Velocidade { get; set; }

		public Vector2 LayerPosOffSet { get; set; }

		float _layerOffset;

		public float LayerOffset
		{
			get { return _layerOffset; }
			set
			{
				_layerOffset = value;
				if (_layerOffset < 0)
				{
					_layerOffset += Textura.Width;
					TexturasIndex--;
				}
				if (_layerOffset > Textura.Width)
				{
					_layerOffset -= Textura.Width;
					TexturasIndex++;
				}
			}
		}

		[Obsolete]
		public ParallaxLayer(Game game, string textureFileName, float velocidade)
			: base(game)
		{
			Texturas = new Texture2D[1];
			TextureFileName = textureFileName;
			Velocidade = velocidade;
			Alpha = 1;
		}

		public ParallaxLayer(Game game, Texture2D texture, float velocidade)
			: base(game)
		{
			Texturas = new Texture2D[1];
			Textura = texture;
			Velocidade = velocidade;
			Alpha = 1;
		}

		public ParallaxLayer(Game game, float velocidade, params Texture2D[] texturas)
			: base(game)
		{
			Texturas = texturas;
			Velocidade = velocidade;
			Alpha = 1;
		}

		protected override void LoadContent()
		{
			_spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

			LoadTexture();

			base.LoadContent();
		}

		public void LoadTexture()
		{
			if (!string.IsNullOrEmpty(TextureFileName))
			{
				Textura = Game.Content.Load<Texture2D>(TextureFileName);
			}
		}

		public override void Update(GameTime gameTime)
		{
			if (Parallax != null)
			{
				LayerOffset += Parallax.Dif * Velocidade;
			}

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (Texturas != null && Texturas.Any() && Parallax != null)
			{
#if SILVERLIGHT
				var color = new Color(Color.White, MathHelper.Clamp((float)Alpha, 0, 1));
#else
				var color = Color.White * (float)Alpha;
#endif

				var pos = Parallax.Position + Position;
				var count = (int)Math.Ceiling(Parallax.ScreenWidth / (float)Textura.Width) + 1;
				var index = TexturasIndex;
				for (int i = 0; i < count; i++)
				{
					if (index < 0)
						index = Texturas.Count() - 1;
					else if (index > Texturas.Count() - 1)
						index = 0;
					Texture2D textura = Texturas[index];
					if (textura != null)
					{
						var width = textura.Width;
						var height = textura.Height;

						var posX = 0;
						if (i == 0)
						{
							width -= (int)_layerOffset;
							posX = (int)_layerOffset;
						}
						_spriteBatch.Draw(textura, pos + LayerPosOffSet, new Rectangle(posX, 0, width, height), color);
						pos.X += width;
					}
					else
					{
						var width = Textura.Width;
						if (i == 0)
						{
							width -= (int)_layerOffset;
						}
						pos.X += width;
					}
					index++;
				}

				/*
				spriteBatch.Draw(Textura, new Rectangle(0, 0, (int)(LayerWidth - _LayerOffset), (int)LayerHeight),
					new Rectangle((int)_LayerOffset, 0, (int)(Textura.Width - _LayerOffset), (int)Textura.Height), color);

				spriteBatch.Draw(Textura, new Rectangle((int)(LayerWidth - _LayerOffset), 0, Math.Abs((int)(this.parallax.screenWidth - (LayerWidth - _LayerOffset))), (int)LayerHeight),
					new Rectangle(0, 0, Math.Abs((int)(this.parallax.screenWidth - (Textura.Width - _LayerOffset))), (int)Textura.Height), color);
				*/
			}

			base.Draw(gameTime);
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(TextureFileName))
				return Path.GetFileName(TextureFileName);
			return "";
		}
		
		/*
		public override void Dispose ()
		{
			base.Dispose ();
			if(Textura != null)
			{
				Textura.Dispose();
				Textura = null;
			}
		}
		*/
	}
}
