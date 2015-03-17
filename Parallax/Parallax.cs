using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Parallax
{
	public class Parallax : DrawableGameComponent
	{
	    readonly List<ParallaxLayer> _layers;

		public Vector2 Position;

		public float Dif;

		public int ScreenWidth { get; set; }
		public int ScreenHeight { get; set; }

		public Parallax(Game game, int screenWidth, int screenHeight)
			: base(game)
		{
			ScreenWidth = screenWidth;
			ScreenHeight = screenHeight;
			Dif = 0;
			_layers = new List<ParallaxLayer>();
		}

		public void AddLayer(ParallaxLayer layer)
		{
			layer.Parallax = this;
			_layers.Add(layer);
		}

		public float LayersAlpha
		{
			set
			{
				foreach (ParallaxLayer layer in _layers)
				{
					layer.Alpha = value;
				}
			}
		}

		public override void Initialize()
		{
			foreach (var pl in _layers)
			{
				pl.Initialize();
			}
			base.Initialize();
		}

		public override void Update(GameTime gameTime)
		{
			foreach (ParallaxLayer pl in _layers)
				pl.Update(gameTime);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			foreach (ParallaxLayer pl in _layers)
				pl.Draw(gameTime);

			base.Draw(gameTime);
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
			_layers.Clear();
		}
	}
}
