using System;
using System.Globalization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.UI
{
    public class TextArea : InputBox
    {
        public TextArea(Game game, SpriteFont font, Vector2 position)
            : this(game, font, position, DefaultTextMaxSize)
        {
        }

        public TextArea(Game game, SpriteFont font, Vector2 position, int maxSize)
            : this(game, font, position, maxSize, false)
        {
        }

        public TextArea(Game game, SpriteFont font, Vector2 position, bool selected)
            : this(game, font, position, DefaultTextMaxSize, selected)
        {
        }

        public TextArea(Game game, SpriteFont font, Vector2 position, int maxSize, bool selected)
            : base(game, font, position, maxSize, selected)
        {
        }

        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                _textToDraw = _text;
                if (!string.IsNullOrEmpty(_textToDraw))
                {
                    if (IsPassword)
                    {
                        var s = new StringBuilder();
                        for (var i = 0; i < _textToDraw.Length; i++)
                        {
                            s.Append("*");
                        }
                        _textToDraw = s.ToString();
                    }
                    var str = new StringBuilder();
                    foreach (var character in Text)
                    {
                        str.Append(character);
                        if (Font.MeasureString(str.ToString()).X*Scale > Width)
                        {
                            str.Insert(str.Length - 1, Environment.NewLine);
                        }
                    }
                    if (str[str.Length - 1].ToString() == Environment.NewLine)
                        str.Remove(str.Length - 1, 1);
                    _textToDraw = str.ToString();
                }
            }
        }
    }
}
