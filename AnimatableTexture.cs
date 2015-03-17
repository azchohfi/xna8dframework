using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.Animations;
using XNA8DFramework.UI;

namespace XNA8DFramework
{
    public class AnimatableTexture : DrawableGameComponent, IScrollable
#if WINDOWS
        , ICloneable
#endif
    {
        public virtual Texture2D Texture { get; set; }

        public SpriteEffects Effect { get; set; }

        public virtual int Width
        {
            get
            {
                if (Texture != null)
                    return Texture.Width;
                return 0;
            }
        }

        public virtual int Height
        {
            get
            {
                if (Texture != null)
                    return Texture.Height;
                return 0;
            }
        }

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

        public int ScrollHeight()
        {
            return Height;
        }

        public int ScrollWidth()
        {
            return Width;
        }

        public virtual double Alpha { get; set; }
        public float Angle { get; set; }
        public Vector2 Origin { get; set; }
        protected Vector2 _Position;
        public virtual Vector2 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
                if (OnPositionChanged != null)
                    OnPositionChanged(this, EventArgs.Empty);
            }
        }
        public Color Color { get; set; }

        public event EventHandler OnPositionChanged;

        protected string Path;

        protected SpriteBatch SpriteBatch;

        [Obsolete]
        public AnimatableTexture(Game game, string path)
            : this(game, path, Vector2.Zero)
        {
        }

        [Obsolete]
        public AnimatableTexture(Game game, string path, Vector2 position)
            : base(game)
        {
            Color = Color.White;
            Position = position;
            Path = path;
            Alpha = 1;
            Scale = 1;
            Effect = SpriteEffects.None;
        }

        public AnimatableTexture(Game game, Texture2D texture)
            : this(game, texture, Vector2.Zero)
        {
        }

        public AnimatableTexture(Game game, Texture2D texture, Vector2 position)
            : base(game)
        {
            Color = Color.White;
            Texture = texture;
            Position = position;
            Alpha = 1;
            Scale = 1;
            Effect = SpriteEffects.None;
        }

        public override void Initialize()
        {
            SpriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            if (Texture == null && !string.IsNullOrEmpty(Path))
                Texture = Game.Content.Load<Texture2D>(Path);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

#if SILVERLIGHT
            var color = new Color(Color, MathHelper.Clamp((Color.A / 255.0f) * (float)Alpha, 0, 1));
#else
            var color = Color * (float)Alpha;
#endif

            if (Texture != null)
                SpriteBatch.Draw(Texture, Position + Origin, null, color, Angle, Origin, Scale2D, Effect, 0);

            base.Draw(gameTime);
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
