using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.UI;

namespace XNA8DFramework
{
    public class AnimatableString : DrawableGameComponent, IScrollable
    {
        public SpriteEffects Effect { get; set; }

        protected Vector2 Size;

        public int Width
        {
            get
            {
                return (int)Size.X;
            }
        }

        public int Height
        {
            get
            {
                return (int)Size.Y;
            }
        }

        public int ScrollHeight()
        {
            return Height;
        }

        public int ScrollWidth()
        {
            return Width;
        }

        public double Alpha { get; set; }
        public float Angle { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }

        public virtual float Scale
        {
            get
            {
                return Scale2D.X;
            }
            set
            {
                if (Scale2D.X != value)
                    IsDirty = true;
                Scale2D = new Vector2(value);
            }
        }

        public virtual Vector2 Scale2D { get; set; }

        protected string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                    IsDirty = true;
                _text = value;
            }
        }

        protected int _maxLineWidth;
        public int MaxLineWidth
        {
            get
            {
                return _maxLineWidth;
            }
            set
            {
                if (_maxLineWidth != value)
                    IsDirty = true;
                _maxLineWidth = value;
            }
        }

        protected int _maxLines;
        public int MaxLines
        {
            get
            {
                return _maxLines;
            }
            set
            {
                if (_maxLines != value)
                    IsDirty = true;
                _maxLines = value;
            }
        }

        protected int _maxLineHeight;
        public int MaxLineHeight
        {
            get
            {
                return _maxLineHeight;
            }
            set
            {
                if (_maxLineHeight != value)
                    IsDirty = true;
                _maxLineHeight = value;
            }
        }

        protected SpriteBatch SpriteBatch;

        protected SpriteFont _font;
        public SpriteFont Font
        {
            get
            {
                return _font;
            }
            set
            {
                if (_font != value)
                    IsDirty = true;
                _font = value;
            }
        }

        protected string s;
        protected bool IsDirty;

        public AnimatableString(Game game, string text, SpriteFont font)
            : this(game, text, font, Vector2.Zero)
        {
        }

        public AnimatableString(Game game, string text, SpriteFont font, Vector2 position)
            : base(game)
        {
            MaxLineWidth = -1;
            MaxLines = -1;
            MaxLineHeight = -1;
            Color = Color.White;
            Position = position;
            Text = text;
            Font = font;
            Alpha = 1;
            Scale = 1;
            Effect = SpriteEffects.None;
            Size = font.MeasureString(text);
            IsDirty = false;
        }

        public override void Initialize()
        {
            SpriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

            UpdateSize();

            base.Initialize();
        }

        public virtual void UpdateSize()
        {
            Size = Font.MeasureString(Text, Scale, MaxLineWidth, MaxLines, MaxLineHeight, out s);
            IsDirty = false;
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

            if (IsDirty)
                UpdateSize();

            SpriteBatch.DrawString(Font, s, Position + Origin, color, Angle, Origin, Scale, Effect, 0);

            base.Draw(gameTime);
        }
    }
}
