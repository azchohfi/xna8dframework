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

namespace XNA8DVisualizadorImagens
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        AnimatableTexture Anim;

        InputService inputService;

        string Path;

        SpriteFont Arial;

        Button btnQuantidadeUp;
        Button btnQuantidadeDown;
        Button btnRedUp;
        Button btnRedDown;
        Button btnGreenUp;
        Button btnGreenDown;
        Button btnBlueUp;
        Button btnBlueDown;

        byte Quantidade = 5;

        public Game1(string[] Paths)
        {
            this.Path = Paths[0];
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 640;
            graphics.PreferredBackBufferWidth = 960;
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

            btnQuantidadeDown = new Button(this, "SetaEsquerda", new Vector2(700, 400));
            btnQuantidadeDown.Click += new EventHandler(btnQuantidadeDown_Click);
            btnQuantidadeDown.Initialize();
            btnQuantidadeUp = new Button(this, "SetaDireita", new Vector2(900, 400));
            btnQuantidadeUp.Click += new EventHandler(btnQuantidadeUp_Click);
            btnQuantidadeUp.Initialize();

            btnRedDown = new Button(this, "SetaEsquerda", new Vector2(700, 450));
            btnRedDown.Click += new EventHandler(btnRedDown_Click);
            btnRedDown.Initialize();
            btnRedUp = new Button(this, "SetaDireita", new Vector2(900, 450));
            btnRedUp.Click += new EventHandler(btnRedUp_Click);
            btnRedUp.Initialize();

            btnGreenDown = new Button(this, "SetaEsquerda", new Vector2(700, 500));
            btnGreenDown.Click += new EventHandler(btnGreenDown_Click);
            btnGreenDown.Initialize();

            btnGreenUp = new Button(this, "SetaDireita", new Vector2(900, 500));
            btnGreenUp.Click += new EventHandler(btnGreenUp_Click);
            btnGreenUp.Initialize();

            btnBlueDown = new Button(this, "SetaEsquerda", new Vector2(700, 550));
            btnBlueDown.Click += new EventHandler(btnBlueDown_Click);
            btnBlueDown.Initialize();

            btnBlueUp = new Button(this, "SetaDireita", new Vector2(900, 550));
            btnBlueUp.Click += new EventHandler(btnBlueUp_Click);
            btnBlueUp.Initialize();

            base.Initialize();
        }

        void btnQuantidadeUp_Click(object sender, EventArgs e)
        {
            Quantidade++;
        }

        void btnQuantidadeDown_Click(object sender, EventArgs e)
        {
            if (Quantidade > 1)
                Quantidade--;
        }

        void btnBlueUp_Click(object sender, EventArgs e)
        {
            Color c = Anim.Color;
            if (Anim.Color.B + Quantidade <= 255)
            {
                c.B += Quantidade;
                Anim.Color = c;
            }
        }

        void btnBlueDown_Click(object sender, EventArgs e)
        {
            Color c = Anim.Color;
            if (Anim.Color.B >= Quantidade)
            {
                c.B -= Quantidade;
                Anim.Color = c;
            }
        }

        void btnGreenUp_Click(object sender, EventArgs e)
        {
            Color c = Anim.Color;
            if (Anim.Color.G + Quantidade <= 255)
            {
                c.G += Quantidade;
                Anim.Color = c;
            }
        }

        void btnGreenDown_Click(object sender, EventArgs e)
        {
            Color c = Anim.Color;
            if (Anim.Color.G >= Quantidade)
            {
                c.G -= Quantidade;
                Anim.Color = c;
            }
        }

        void btnRedUp_Click(object sender, EventArgs e)
        {
            Color c = Anim.Color;
            if (Anim.Color.R + Quantidade <= 255)
            {
                c.R += Quantidade;
                Anim.Color = c;
            }
        }

        void btnRedDown_Click(object sender, EventArgs e)
        {
            Color c = Anim.Color;
            if (Anim.Color.R >= Quantidade)
            {
                c.R -= Quantidade;
                Anim.Color = c;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Texture2D tex = Texture2D.FromStream(GraphicsDevice, File.OpenRead(Path)).PreMultiplyAlpha();
            Anim = new AnimatableTexture(this, tex);
            Anim.Initialize();

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

            Anim.Update(gameTime);

            inputService.Update(gameTime);

            btnQuantidadeUp.Update(gameTime);
            btnQuantidadeDown.Update(gameTime);

            btnRedUp.Update(gameTime);
            btnRedDown.Update(gameTime);

            btnGreenUp.Update(gameTime);
            btnGreenDown.Update(gameTime);

            btnBlueUp.Update(gameTime);
            btnBlueDown.Update(gameTime);

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

            Anim.Draw(gameTime);

            btnQuantidadeUp.Draw(gameTime);
            spriteBatch.DrawString(Arial, "Quantidade: " + Quantidade, new Vector2(730, 400), Color.White);
            btnQuantidadeDown.Draw(gameTime);

            btnRedUp.Draw(gameTime);
            spriteBatch.DrawString(Arial, "Red: " + Anim.Color.R, new Vector2(770, 450), Color.White);
            btnRedDown.Draw(gameTime);
            
            btnGreenUp.Draw(gameTime);
            spriteBatch.DrawString(Arial, "Green: " + Anim.Color.G, new Vector2(770, 500), Color.White);
            btnGreenDown.Draw(gameTime);

            btnBlueUp.Draw(gameTime);
            spriteBatch.DrawString(Arial, "Blue: " + Anim.Color.B, new Vector2(770, 550), Color.White);
            btnBlueDown.Draw(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
