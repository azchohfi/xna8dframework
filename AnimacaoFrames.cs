using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XNA8DFramework.Animations;

namespace XNA8DFramework
{
    public class AnimacaoFrames : DrawableGameComponent, IDrawable8D
    {
        private Texture2D _textura;
        public Texture2D Textura
        {
            get
            {
                return _textura;
            }
            set
            {
                _textura = value;
                if (XCount != 0 && YCount != 0)
                    SetXCount();
                if (XCount != 0 && YCount != 0)
                    UpdateRec();
                Anim = 0;
            }
        }

        private int _anim;
        public int Anim
        {
            get
            {
                return _anim;
            }
            set
            {
                _anim = value;
                if (XCount != 0 && YCount != 0)
                    UpdateSourceRec();
                TotalAnimTime = TimeSpan.Zero;
            }
        }
        public TimeSpan TotalAnimTime { get; set; }
        private Vector2 _position;
        public virtual Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                UpdateRec();
            }
        }

        private void SetXCount()
        {
            XCount = Textura.Width / (Textura.Height / YCount);
            if (XCount == 0)
                XCount = 1;
        }

        public Vector2 Origin { get; set; }
        public float Angle { get; set; }

        public virtual float Scale
        {
            get
            {
                return Scale2D.X;
            }
            set
            {
                Scale2D = new Vector2(value);
            }
        }

        public virtual Vector2 Scale2D { get; set; }

        public int Width
        {
            get
            {
                if (Textura != null)
                    return Textura.Width / XCount;
                return 0;
            }
        }

        public int Height
        {
            get
            {
                if (Textura != null)
                    return Textura.Height / YCount;
                return 0;
            }
        }

        readonly string _path = "";

        public int XCount { get; set; }

        public int YCount { get; set; }

        Rectangle _sourceRec;

        public Rectangle SourceRec
        {
            get
            {
                return _sourceRec;
            }
        }

        Rectangle _rec;

        public virtual Rectangle Rec
        {
            get
            {
                return _rec;
            }
        }

        public double Alpha { get; set; }
        public Color Color { get; set; }

        private Color _colorToDraw;

        public bool Started;
        public bool UseScrolling;

        public SentidosAnim SentidoAnim { get; set; }

        public int Speed;

        public SpriteEffects Effect { get; set; }

        protected SpriteBatch SpriteBatch { get; private set; }

        public bool Play { get; set; }

        public const int DefaultSpeed = 200;
        
        public event EventHandler EventoTerminouAnim;
        protected virtual void OnEventoTerminouAnim(object sender, EventArgs args) { }

        public event EventHandler<AnimUpdateEventArgs> EventoAnimUpdate;
        protected virtual void OnEventoAnimUpdate(object sender, AnimUpdateEventArgs args) { }

        [Obsolete]
        public AnimacaoFrames(Game game, string path)
            : this(game, path, DefaultSpeed, null)
        {
        }

        [Obsolete]
        public AnimacaoFrames(Game game, string path, int speed)
            : this(game, path, speed, null)
        {
        }

        [Obsolete]
        public AnimacaoFrames(Game game, string path, int speed, EventHandler eventoTerminouAnim)
            : base(game)
        {
            Color = Color.White;
            EventoTerminouAnim += eventoTerminouAnim;
            Alpha = 1;
            Scale = 1;
            SentidoAnim = SentidosAnim.Normal;
            Speed = speed;
            Anim = 0;
            _path = path;
            TotalAnimTime = TimeSpan.Zero;
            YCount = 1;
            Started = false;
        }

        public AnimacaoFrames(Game game, Texture2D textura)
            : this(game, textura, DefaultSpeed, null)
        {
        }

        public AnimacaoFrames(Game game, Texture2D textura, int speed)
            : this(game, textura, speed, null)
        {
        }

        public AnimacaoFrames(Game game, Texture2D textura, int speed, EventHandler eventoTerminouAnim)
            : base(game)
        {
            Color = Color.White;
            EventoTerminouAnim += eventoTerminouAnim;
            Alpha = 1;
            Scale = 1;
            SentidoAnim = SentidosAnim.Normal;
            Speed = speed;
            Anim = 0;
            YCount = 1;
            Textura = textura;
            TotalAnimTime = TimeSpan.Zero;
        }

        public override void Initialize()
        {
            UseScrolling = true;
            Effect = SpriteEffects.None;
            Play = true;
            base.Initialize();
            UpdateRec();
        }

        protected override void LoadContent()
        {
            SpriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            if (_path.Length > 0)
            {
                Textura = Game.Content.Load<Texture2D>(_path);
            }
            if (XCount == 0)
                SetXCount();
            _sourceRec = new Rectangle(0, 0, Textura.Width / XCount, Textura.Height / YCount);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            TotalAnimTime += gameTime.ElapsedGameTime;

            if (Play && TotalAnimTime.TotalMilliseconds > Speed)
            {
                Started = true;

                if (SentidoAnim == SentidosAnim.Normal)
                {
                    Anim++;
                    if (Anim >= XCount * YCount)
                    {
                        Anim = 0;
                        if (EventoTerminouAnim != null)
                            EventoTerminouAnim(this, EventArgs.Empty);
                        OnEventoTerminouAnim(this, EventArgs.Empty);
                    }
                }
                else
                {
                    Anim--;
                    if (Anim < 0)
                    {
                        Anim = XCount * YCount - 1;
                        if (EventoTerminouAnim != null)
                            EventoTerminouAnim(this, EventArgs.Empty);
                        OnEventoTerminouAnim(this, EventArgs.Empty);
                    }
                }
                var animUpdateEventArgs = new AnimUpdateEventArgs(Anim);
                if (EventoAnimUpdate != null)
                    EventoAnimUpdate(this, animUpdateEventArgs);
                OnEventoAnimUpdate(this, animUpdateEventArgs);
            }

            base.Update(gameTime);
        }

        public void UpdateSourceRec()
        {
            if (XCount != 0 && YCount != 0)
            {
                _sourceRec.X = (Anim % XCount) * (Textura.Width / XCount);
                _sourceRec.Y = Anim / XCount * (Textura.Height / YCount);
                _sourceRec.Width = Textura.Width / XCount;
                _sourceRec.Height = Textura.Height / YCount;
            }
        }

        private void UpdateRec()
        {
            if (Textura != null && XCount != 0 && YCount != 0)
            {
                _rec.X = (int)_position.X;
                _rec.Y = (int)_position.Y;
                _rec.Width = Textura.Width / XCount;
                _rec.Height = Textura.Height / YCount;
                _rec.Inflate((int)(_rec.Width * (Scale2D.X - 1) / 2), (int)(_rec.Height * (Scale2D.Y - 1) / 2));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;
#if SILVERLIGHT
            //colorToDraw = new Color(this.color, (this.color.A / 255.0f) * (float)Alpha);
            _colorToDraw = Color;
            _colorToDraw.A = Convert.ToByte(Color.A * (float)Alpha);
#else
            _colorToDraw = Color * (float)Alpha;
#endif

            if (UseScrolling)
                SpriteBatch.Draw(Textura, Position + ScrollableGame.Scrolling + Origin, _sourceRec, _colorToDraw, Angle, Origin, Scale2D, Effect, 0);
            else
                SpriteBatch.Draw(Textura, Position + Origin, _sourceRec, _colorToDraw, Angle, Origin, Scale2D, Effect, 0);

            base.Draw(gameTime);
        }

        public object Clone()
        {
            var anim = (AnimacaoFrames)MemberwiseClone();
            anim.TotalAnimTime = TimeSpan.Zero;
            anim.Position = new Vector2(Position.X, Position.Y);
            anim._sourceRec = new Rectangle(_sourceRec.X, _sourceRec.Y, _sourceRec.Width, _sourceRec.Height);
            return anim;
        }
    }
}
