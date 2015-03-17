using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.UI;

namespace XNA8DFramework.Tests
{
	/// <summary>
	/// Sample showing how to manage different game states, with transitions
	/// between menu screens, a loading screen, the game itself, and a pause
	/// menu. This main game class is extremely simple: all the interesting
	/// stuff happens in the ScreenManager component.
	/// </summary>
	public class GameStateManagementGame : Game
	{
		public GraphicsDeviceManager graphics;
		ScreenManager screenManager;
		SpriteBatch spriteBatch;

		RenderTarget2D renderTarget;

		/// <summary>
		/// The main game constructor.
		/// </summary>
		public GameStateManagementGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			Services.AddService(typeof(GraphicsDeviceManager), graphics);

			this.IsMouseVisible = true;

			graphics.PreferredBackBufferWidth = 760;
			graphics.PreferredBackBufferHeight = 540;

			//TargetElapsedTime = TimeSpan.FromTicks(333333);

			// Create the screen manager component.
			screenManager = new ScreenManager(this, @"Fonts\Arial");
			Components.Add(screenManager);

			this.Initialize();
		}

		protected override void Initialize()
		{
#if WINDOWS
			EventInput.Initialize(this);
			EventInput.CharEntered += new EventHandler<CharacterEventArgs>(EventInput_CharEntered);
#endif

			graphics.ApplyChanges();

			spriteBatch = new SpriteBatch(GraphicsDevice);
			this.Services.AddService(typeof(SpriteBatch), spriteBatch);

			base.Initialize();

#if SILVERLIGHT
			renderTarget = new RenderTarget2D(this.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1, this.GraphicsDevice.PresentationParameters.BackBufferFormat, RenderTargetUsage.DiscardContents);
#else
			renderTarget = new RenderTarget2D(this.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, false,
								this.GraphicsDevice.PresentationParameters.BackBufferFormat,
								DepthFormat.Depth24,
								0,
								RenderTargetUsage.PreserveContents);
#endif

			ShowMainMenu(this.screenManager);
		}

#if WINDOWS
		void EventInput_CharEntered(object sender, CharacterEventArgs e)
		{
			InputBox.AddCharToQueue(e.Character);
		}
#endif

		public static void ShowMainMenu(ScreenManager screenManager)
		{
			screenManager.AddScreen(new GameplayScreen());
			GC.Collect();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		protected override void Draw(GameTime gameTime)
		{
#if SILVERLIGHT
			graphics.GraphicsDevice.SetRenderTarget(0, renderTarget);
#else
			graphics.GraphicsDevice.SetRenderTarget(renderTarget);
#endif

			graphics.GraphicsDevice.Clear(Color.Black);

			// The real drawing happens inside the screen manager component.
			base.Draw(gameTime);

#if SILVERLIGHT
			graphics.GraphicsDevice.SetRenderTarget(0, null);
#else
			graphics.GraphicsDevice.SetRenderTarget(null);
#endif

			spriteBatch.Begin();

#if SILVERLIGHT
			spriteBatch.Draw(renderTarget.GetTexture(), Vector2.Zero, Color.White);
#else
			spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
#endif

			spriteBatch.End();
		}
	}
}
