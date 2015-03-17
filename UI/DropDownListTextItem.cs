using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.UI
{
    public class DropDownListTextItem : DropDownListItem
    {
        public string Text { get; set; }

        public SpriteFont Font { get; set; }

        public DropDownListTextItem(Game game, string text, SpriteFont font)
            : base(game)
        {
            Text = text;
            Font = font;
            Height = (int)(font.MeasureString(text).Y * Scale);
        }

        public override void Initialize()
        {
            Origin = new Vector2(Width, Height) / 2;
            base.Initialize();
        }

        public override float Scale
        {
            get
            {
                return base.Scale;
            }
            set
            {
                base.Scale = value;
                if(Font != null)
                    Height = (int)(Font.MeasureString(Text).Y * value);
            }
        }

        public override void Draw(GameTime gameTime)
        {
#if SILVERLIGHT
            var color = new Color(Color, MathHelper.Clamp((float)Alpha, 0, 1));
#else
            var color = Color * (float)Alpha;
#endif
            SpriteBatch.DrawWrappedStringWidth(Font, Text, Position + Origin, color, Angle, Origin, Scale, SpriteEffects.None, 0, Width, 1);
        }
    }
}
