using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using XNA8DFramework;
using XNA8DFramework.UI;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;

namespace XNA8DVisualizadorAnims
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        AnimacaoFrames Anim;
        AnimacaoMultiplosArquivos Anims;

        InputService inputService;

        int Speed = 100;

        string[] Paths;

        SpriteFont Arial;

        Button btnSpeedUp;
        Button btnSpeedDown;

        Button btnXCountUp;
        Button btnXCountDown;

        Button btnYCountUp;
        Button btnYCountDown;

        public Game1(string[] Paths)
        {
            this.Paths = Paths;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);

            inputService = new InputService(this);
            inputService.Initialize();

            TouchPanel.EnabledGestures = GestureType.None;

            ScrollableGame.Input = inputService;

            btnSpeedDown = new Button(this, "SetaEsquerda", new Vector2(400, 100));
            btnSpeedDown.Click += new EventHandler(btnSpeedDown_Click);
            btnSpeedDown.Initialize();

            btnSpeedUp = new Button(this, "SetaDireita", new Vector2(500, 100));
            btnSpeedUp.Click += new EventHandler(btnSpeedUp_Click);
            btnSpeedUp.Initialize();

            btnXCountDown = new Button(this, "SetaEsquerda", new Vector2(400, 200));
            btnXCountDown.Click += new EventHandler(btnXCountDown_Click);
            btnXCountDown.Initialize();

            btnXCountUp = new Button(this, "SetaDireita", new Vector2(500, 200));
            btnXCountUp.Click += new EventHandler(btnXCountUp_Click);
            btnXCountUp.Initialize();

            btnYCountDown = new Button(this, "SetaEsquerda", new Vector2(400, 300));
            btnYCountDown.Click += new EventHandler(btnYCountDown_Click);
            btnYCountDown.Initialize();

            btnYCountUp = new Button(this, "SetaDireita", new Vector2(500, 300));
            btnYCountUp.Click += new EventHandler(btnYCountUp_Click);
            btnYCountUp.Initialize();

            if (Paths.Count() > 1)
            {
                btnXCountUp.Visible = false;
                btnXCountUp.Enabled = false;
                btnXCountDown.Visible = false;
                btnXCountDown.Enabled = false;

                btnYCountUp.Visible = false;
                btnYCountUp.Enabled = false;
                btnYCountDown.Visible = false;
                btnYCountDown.Enabled = false;
            }

            base.Initialize();
        }

        void btnYCountDown_Click(object sender, EventArgs e)
        {
            if (Anim.YCount > 1)
                Anim.YCount--;
        }

        void btnYCountUp_Click(object sender, EventArgs e)
        {
            Anim.YCount++;
        }

        void btnXCountDown_Click(object sender, EventArgs e)
        {
            if (Anim.XCount > 1)
                Anim.XCount--;
        }

        void btnXCountUp_Click(object sender, EventArgs e)
        {
            Anim.XCount++;
        }

        void btnSpeedDown_Click(object sender, EventArgs e)
        {
            if (Speed >= 10)
                Speed -= 10;
        }

        void btnSpeedUp_Click(object sender, EventArgs e)
        {
            Speed += 10;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            if (Paths.Count() == 1)
            {
                Texture2D tex = Texture2D.FromStream(GraphicsDevice, File.OpenRead(Paths[0])).PreMultiplyAlpha();
                Anim = new AnimacaoFrames(this, tex, 100);
                Anim.XCount = 1;
                Anim.YCount = 1;
                Anim.Initialize();
            }
            else
            {
                List<Texture2D> texts = new List<Texture2D>();
                foreach (string s in Paths)
                {
                    Texture2D tex = Texture2D.FromStream(GraphicsDevice, File.OpenRead(s)).PreMultiplyAlpha();
                    texts.Add(tex);
                }
                Anims = new AnimacaoMultiplosArquivos(this, texts, 100);
                Anims.Initialize();
            }

            Arial = this.Content.Load<SpriteFont>("Arial");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Anim != null)
            {
                Anim.Speed = Speed;
                Anim.Update(gameTime);
            }
            else if (Anims != null)
            {
                Anims.Speed = Speed;
                Anims.Update(gameTime);
            }

            inputService.Update(gameTime);

            btnSpeedUp.Update(gameTime);
            btnSpeedDown.Update(gameTime);

            btnXCountUp.Update(gameTime);
            btnXCountDown.Update(gameTime);

            btnYCountUp.Update(gameTime);
            btnYCountDown.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(127, 127, 127));

            spriteBatch.Begin();

            if (Anim != null)
            {
                Anim.Draw(gameTime);

                btnXCountUp.Draw(gameTime);
                spriteBatch.DrawString(Arial, "XCount: " + Anim.XCount, new Vector2(560, 200), Color.White);
                btnXCountDown.Draw(gameTime);

                btnYCountUp.Draw(gameTime);
                spriteBatch.DrawString(Arial, "YCount: " + Anim.YCount, new Vector2(560, 300), Color.White);
                btnYCountDown.Draw(gameTime);
            }
            else if (Anims != null)
            {
                Anims.Draw(gameTime);
            }

            btnSpeedUp.Draw(gameTime);
            spriteBatch.DrawString(Arial, "Speed: " + Speed, new Vector2(560, 100), Color.White);
            btnSpeedDown.Draw(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
