using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XNA8DFramework.Animations;

namespace XNA8DFramework
{
    public class AnimacaoMultiplosArquivos : DrawableGameComponent
    {
        public List<Texture2D> Texturas { get; set; }
        public int Anim { get; set; }
        public TimeSpan Time { get; set; }
        public Vector2 Posicao { get; set; }
        readonly List<string> _paths;
        public bool Started;
        public bool UseScrolling;

        public SentidosAnim SentidoAnim { get; set; }

        public int Speed;

        public SpriteEffects Effect { get; set; }

        public Color Color { get; set; }

        SpriteBatch _spriteBatch;

        public bool Play;

        public const int DefaultSpeed = 200;

        public event EventHandler EventoTerminouAnim;

        [Obsolete]
        public AnimacaoMultiplosArquivos(Game game, List<string> paths)
            : base(game)
        {
            Color = Color.White;
            SentidoAnim = SentidosAnim.Normal;
            Speed = AnimacaoFrames.DefaultSpeed;
            Anim = 0;
            _paths = paths;
            Time = TimeSpan.Zero;
            Started = false;
        }

        public AnimacaoMultiplosArquivos(Game game, List<Texture2D> texture, int speed)
            : this(game, texture)
        {
            Speed = speed;
        }

        public AnimacaoMultiplosArquivos(Game game, List<Texture2D> textures)
            : base(game)
        {
            Color = Color.White;
            _paths = new List<string>();
            SentidoAnim = SentidosAnim.Normal;
            Speed = AnimacaoFrames.DefaultSpeed;
            Anim = 0;
            Texturas = textures;
            Time = TimeSpan.Zero;
        }

        public override void Initialize()
        {
            UseScrolling = true;
            Effect = SpriteEffects.None;
            Play = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            int i=0;
            foreach(string path in _paths)
            {
                Texturas[i] = Game.Content.Load<Texture2D>(path);
                i++;
            }
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;
            Time += gameTime.ElapsedGameTime;
            if (Play && Time.TotalMilliseconds > Speed)
            {
                Started = true;

                if (SentidoAnim == SentidosAnim.Normal)
                {
                    Anim++;
                    if (Anim >= Texturas.Count)
                    {
                        Anim = 0;
                        if (EventoTerminouAnim != null)
                            EventoTerminouAnim(this, EventArgs.Empty);
                    }
                }
                else
                {
                    Anim--;
                    if (Anim < 0)
                    {
                        Anim = Texturas.Count - 1;
                        if (EventoTerminouAnim != null)
                            EventoTerminouAnim(this, EventArgs.Empty);
                    }
                }

                Time = TimeSpan.Zero;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;
            if (UseScrolling)
            {
                _spriteBatch.Draw(Texturas[Anim], Posicao + ScrollableGame.Scrolling, null, Color, 0, Vector2.Zero, 1, Effect, 0);
            }
            else
            {
                _spriteBatch.Draw(Texturas[Anim], Posicao, null, Color, 0, Vector2.Zero, 1, Effect, 0);
            }

            base.Draw(gameTime);
        }

        public object Clone()
        {
            var anim = (AnimacaoMultiplosArquivos)MemberwiseClone();
            anim.Time = TimeSpan.Zero;
            anim.Posicao = new Vector2(Posicao.X, Posicao.Y);
            return anim;
        }
    }
}
