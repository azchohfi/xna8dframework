#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
#if WINDOWS8
using Windows.Storage;
using System.Threading.Tasks;
#else
using System.IO.IsolatedStorage;
#endif
using XNA8DFramework;
#if !IPHONE && !SILVERLIGHT
using System.Xml.Linq;
#endif
#endregion

namespace GameStateManagement
{
	/// <summary>
	/// The screen manager is a component which manages one or more GameScreen
	/// instances. It maintains a stack of screens, calls their Update and Draw
	/// methods at the appropriate times, and automatically routes input to the
	/// topmost active screen.
	/// </summary>
	public class ScreenManager : DrawableGameComponent
	{
		#region Fields

		private const string StateFilename = "ScreenManagerState.xml";

		readonly List<GameScreen> _screens = new List<GameScreen>();
		readonly List<GameScreen> _tempScreensList = new List<GameScreen>();

		readonly InputService _input;

		SpriteBatch _spriteBatch;
		SpriteFont _font;
		readonly string _fontName;
		Texture2D _blankTexture;
		
		GraphicsDeviceManager _graphics;
		
		ContentManager _content;

		bool _isInitialized;

		bool _traceEnabled;

		#endregion

		#region Properties


		/// <summary>
		/// A default SpriteBatch shared by all the screens. This saves
		/// each screen having to bother creating their own local instance.
		/// </summary>
		public SpriteBatch SpriteBatch
		{
			get { return _spriteBatch; }
		}


		/// <summary>
		/// A default font shared by all the screens. This saves
		/// each screen having to bother loading their own local copy.
		/// </summary>
		public SpriteFont Font
		{
			get { return _font; }
		}

		public GraphicsDeviceManager Graphics
		{
			get { return _graphics; }
		}

		/// <summary>
		/// If true, the manager prints out a list of all the screens
		/// each time it is updated. This can be useful for making sure
		/// everything is being added and removed at the right times.
		/// </summary>
		public bool TraceEnabled
		{
			get { return _traceEnabled; }
			set { _traceEnabled = value; }
		}

		public bool SuppressInputUpdate { get; set; }

#if IPHONE
		public bool FixScaleIPhone { get; set; }
#endif

		#endregion

		#region Initialization


		/// <summary>
		/// Constructs a new screen manager component.
		/// </summary>
		public ScreenManager(Game game, string fontName)
			: base(game)
		{
			_fontName = fontName;
			_input = new InputService(game);
			_input.Initialize();
			SuppressInputUpdate = false;
			ScrollableGame.Input = _input;

			game.Services.AddService(typeof(InputService), _input);
#if WINDOWS_PHONE || WINDOWS || ANDROID
			TouchPanel.EnabledGestures = GestureType.None;
#endif
#if IPHONE
			FixScaleIPhone = true;
#endif
		}


		/// <summary>
		/// Initializes the screen manager component.
		/// </summary>
		public override void Initialize()
		{
			_graphics = Game.Services.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager;

			base.Initialize();

			UpdateTouchPanelSize();

			_isInitialized = true;
		}

		private void UpdateTouchPanelSize()
		{
#if ANDROID
			if (GraphicsDevice.Viewport.Width != TouchPanel.DisplayWidth)
				TouchPanel.DisplayWidth = GraphicsDevice.Viewport.Width;
			if (GraphicsDevice.Viewport.Height != TouchPanel.DisplayHeight)
				TouchPanel.DisplayHeight = GraphicsDevice.Viewport.Height;
#endif
		}


		/// <summary>
		/// Load your graphics content.
		/// </summary>
		protected override void LoadContent()
		{
			_content = new ContentManager(Game.Services, Game.Content.RootDirectory);

			_spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
			try
			{
				_font = _content.Load<SpriteFont>(_fontName).Fix();
			}
			catch(Exception e) {
				Debug.WriteLine("ScreenManager's font not loaded. Error: " + e);
			}

			_blankTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
			_blankTexture.SetData(new[] { Color.White });

			// Tell each of the screens to load their content.
			foreach (GameScreen screen in _screens)
			{
				if (screen.content == null)
					screen.content = new ContentManager(Game.Services, Game.Content.RootDirectory);
				screen.Activate(false);
			}
		}

		/// <summary>
		/// Unload your graphics content.
		/// </summary>
		protected override void UnloadContent()
		{
			// Tell each of the screens to unload their content.
			foreach (GameScreen screen in _screens)
			{
				screen.Unload();
			}
		}


		#endregion

		#region Update and Draw


		/// <summary>
		/// Allows each screen to run logic.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			UpdateTouchPanelSize();

			// Read the keyboard and gamepad.
			if (!SuppressInputUpdate)
				_input.Update(gameTime);
			SuppressInputUpdate = false;

			// Make a copy of the master screen list, to avoid confusion if
			// the process of updating one screen adds or removes others.
			_tempScreensList.Clear();

			foreach (GameScreen screen in _screens)
				_tempScreensList.Add(screen);

			bool otherScreenHasFocus = !Game.IsActive;
			bool coveredByOtherScreen = false;

			// Loop as long as there are screens waiting to be updated.
			while (_tempScreensList.Count > 0)
			{
				// Pop the topmost screen off the waiting list.
				GameScreen screen = _tempScreensList[_tempScreensList.Count - 1];

				_tempScreensList.RemoveAt(_tempScreensList.Count - 1);

				// Update the screen.
				screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

				if (screen.ScreenState == ScreenState.TransitionOn ||
					screen.ScreenState == ScreenState.Active)
				{
					// If this is the first active screen we came across,
					// give it a chance to handle input.
					if (!otherScreenHasFocus)
					{
						screen.HandleInput(gameTime, _input);

						otherScreenHasFocus = true;
					}

					// If this is an active non-popup, inform any subsequent
					// screens that they are covered by it.
					if (!screen.IsPopup)
						coveredByOtherScreen = true;
				}
			}

			ScrollableGame.End();

			// Print debug trace?
			if (_traceEnabled)
				TraceScreens();
		}


		/// <summary>
		/// Prints a list of all the screens, for debugging.
		/// </summary>
		void TraceScreens()
		{
			//var screenNames = new List<string>();
			//foreach (GameScreen screen in _screens)
			//    screenNames.Add(screen.GetType().Name);
			//Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
			Debug.WriteLine(string.Join(", ", _screens.Select(screen => screen.GetType().Name).ToArray()));
		}


		/// <summary>
		/// Tells each screen to draw itself.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			foreach (GameScreen screen in _screens)
			{
				if (screen.ScreenState == ScreenState.Hidden)
					continue;

				screen.Draw(gameTime);
			}
		}


		#endregion

		#region Public Methods


		/// <summary>
		/// Adds a new screen to the screen manager.
		/// </summary>
		public void AddScreen(GameScreen screen)
		{
			screen.ScreenManager = this;
			screen.IsExiting = false;

			// If we have a graphics device, tell the screen to load content.
			if (_isInitialized)
			{
				if (screen.content == null)
					screen.content = new ContentManager(Game.Services, Game.Content.RootDirectory);
				screen.Activate(false);
			}

			_screens.Add(screen);

#if WINDOWS_PHONE || WINDOWS || ANDROID
			// update the TouchPanel to respond to gestures this screen is interested in
			TouchPanel.EnabledGestures = screen.EnabledGestures;
#endif
		}

		public void AddOnlyOne<T>(T screen) where T : GameScreen
		{
			if (_screens.Count(s => s is T) == 0)
				AddScreen(screen);
		}

		/// <summary>
		/// Removes a screen from the screen manager. You should normally
		/// use GameScreen.ExitScreen instead of calling this directly, so
		/// the screen can gradually transition off rather than just being
		/// instantly removed.
		/// </summary>
		public void RemoveScreen(GameScreen screen)
		{
			// If we have a graphics device, tell the screen to unload content.
			if (_isInitialized)
			{
				screen.Unload();
			}

			_screens.Remove(screen);
			_tempScreensList.Remove(screen);

#if WINDOWS_PHONE || WINDOWS || ANDROID
			// if there is a screen still in the manager, update TouchPanel
			// to respond to gestures that screen is interested in.
			if (_screens.Count > 0)
			{
				TouchPanel.EnabledGestures = _screens[_screens.Count - 1].EnabledGestures;
			}
#endif
		}

		/// <summary>
		/// Expose an array holding all the screens. We return a copy rather
		/// than the real master list, because screens should only ever be added
		/// or removed using the AddScreen and RemoveScreen methods.
		/// </summary>
		public GameScreen[] GetScreens()
		{
			return _screens.ToArray();
		}
		
		/// <summary>
		/// Helper draws a translucent white fullscreen sprite, used for fading
		/// screens in and out, and for whitening the background behind popups.
		/// </summary>
		public void FadeBackBufferToWhite(float alpha)
		{
			FadeBackBufferToColor(Color.White, alpha);
		}

		/// <summary>
		/// Helper draws a translucent black fullscreen sprite, used for fading
		/// screens in and out, and for darkening the background behind popups.
		/// </summary>
		public void FadeBackBufferToBlack(float alpha)
		{
			FadeBackBufferToColor(Color.Black, alpha);
		}
		
		private void FadeBackBufferToColor(Color color, float alpha)
		{
			_spriteBatch.Begin();

#if SILVERLIGHT
			Color colorAlpha = new Color(color, alpha);
#else
			Color colorAlpha = color * alpha;
#endif
			
			//int Max = Math.Max(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
			
			_spriteBatch.Draw(_blankTexture,
							 new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
							 colorAlpha);

			_spriteBatch.End();
		}

		/// <summary>
		/// Informs the screen manager to serialize its state to disk.
		/// </summary>
#if WINDOWS8
		public async void Deactivate()
#else
		public void Deactivate()
#endif
		{
#if !WINDOWS_PHONE && !WINDOWS8 && !ANDROID
			return;
#else
#if WINDOWS_PHONE || ANDROID
			// Open up isolated storage
			using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
			{
#endif
			// Create an XML document to hold the list of screen types currently in the stack
				XDocument doc = new XDocument();
				XElement root = new XElement("ScreenManager");
				doc.Add(root);

				// Make a copy of the master screen list, to avoid confusion if
				// the process of deactivating one screen adds or removes others.
				_tempScreensList.Clear();
				foreach (GameScreen screen in _screens)
					_tempScreensList.Add(screen);

				// Iterate the screens to store in our XML file and deactivate them
				foreach (GameScreen screen in _tempScreensList)
				{
					// Only add the screen to our XML if it is serializable
					if (screen.IsSerializable)
					{
						// We store the screen's controlling player so we can rehydrate that value

						root.Add(new XElement(
							"GameScreen",
							new XAttribute("Type", screen.GetType().AssemblyQualifiedName)));
					}

					// Deactivate the screen regardless of whether we serialized it
					screen.Deactivate();
				}
				
				// Save the document
#if WINDOWS8
				StorageFolder folder = ApplicationData.Current.LocalFolder;
				using (Stream stream = await folder.OpenStreamForWriteAsync(StateFilename, CreationCollisionOption.ReplaceExisting))
				{
#else
				using (IsolatedStorageFileStream stream = storage.CreateFile(StateFilename))
				{
#endif
					doc.Save(stream);
#if WINDOWS8
					await stream.FlushAsync();
#endif
				}
#if WINDOWS_PHONE || ANDROID
			}
#endif
#endif // !WINDOWS_PHONE && !WINDOWS8 && !ANDROID
		}

#if WINDOWS8
		public async Task<bool> Activate(bool instancePreserved)
#else
		public bool Activate(bool instancePreserved)
#endif
		{
#if !WINDOWS_PHONE && !WINDOWS && !ANDROID
			return false;
#else
			// If the game instance was preserved, the game wasn't dehydrated so our screens still exist.
			// We just need to activate them and we're ready to go.
			if (instancePreserved)
			{
				// Make a copy of the master screen list, to avoid confusion if
				// the process of activating one screen adds or removes others.
				_tempScreensList.Clear();

				foreach (GameScreen screen in _screens)
					_tempScreensList.Add(screen);

				foreach (GameScreen screen in _tempScreensList)
				{
					screen.Activate(true);
				}
			}

			// Otherwise we need to refer to our saved file and reconstruct the screens that were present
			// when the game was deactivated.
			else
			{
				// Try to get the screen factory from the services, which is required to recreate the screens
				var screenFactory = Game.Services.GetService(typeof(IScreenFactory)) as IScreenFactory;
				if (screenFactory == null)
				{
					throw new InvalidOperationException(
						"Game.Services must contain an IScreenFactory in order to activate the ScreenManager.");
				}

#if WINDOWS8
				StorageFolder folder = ApplicationData.Current.LocalFolder;
				try
				{
					using (Stream file = await folder.OpenStreamForReadAsync(StateFilename))
					{
#else
				// Open up isolated storage
				using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
				{
					// Check for the file; if it doesn't exist we can't restore state
					if (!storage.FileExists(StateFilename))
						return false;

					// Read the state file so we can build up our screens
					using (IsolatedStorageFileStream file = storage.OpenFile(StateFilename, FileMode.Open))
					{
#endif
						XDocument doc = XDocument.Load(file);

						// Iterate the document to recreate the screen stack
						foreach (XElement screenElem in doc.Root.Elements("GameScreen"))
						{
							// Use the factory to create the screen
							Type screenType = Type.GetType(screenElem.Attribute("Type").Value);
							GameScreen screen = screenFactory.CreateScreen(screenType);

							if (screen != null)
							{
								// Add the screen to the screens list and activate the screen
								screen.ScreenManager = this;
								_screens.Add(screen);

								if (screen.content == null)
									screen.content = new ContentManager(Game.Services, Game.Content.RootDirectory);
								screen.Activate(false);

								// update the TouchPanel to respond to gestures this screen is interested in
								TouchPanel.EnabledGestures = screen.EnabledGestures;
							}
						}
					}
				}
#if WINDOWS8
				catch (FileNotFoundException)
				{
					return false;
				}
#endif
			}

			return true;
#endif
		}

		#endregion
	}
}
