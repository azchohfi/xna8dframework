using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework
{
    public class FPSCounter : DrawableGameComponent
    {
        TimeSpan _timer;
        int _countFps;

        SpriteBatch spriteBatch;

        public int Fps { get; private set; }
        public int Interval { get; set; }

        string _fontPath;
        SpriteFont Font;

        public FPSCounter(Game game, string fontPath)
            : base(game)
        {
            Interval = 1000;
            _fontPath = fontPath;
        }

        public override void Initialize()
        {
            _timer = TimeSpan.Zero;
            this.DrawOrder = int.MaxValue;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = this.Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            if (!string.IsNullOrEmpty(_fontPath))
                Font = this.Game.Content.Load<SpriteFont>(_fontPath);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _countFps++;
            _timer += gameTime.ElapsedGameTime;
            if (_timer.TotalMilliseconds >= 1000)
            {
                _timer -= TimeSpan.FromMilliseconds(1000);
                Fps = _countFps;
                _countFps = 0;
                Debug.WriteLine("FPS: " + Fps);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!this.Visible || Font == null)
                return;

            spriteBatch.Begin();

            spriteBatch.DrawString(Font, Fps.ToString(), Vector2.Zero, Color.Blue);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
