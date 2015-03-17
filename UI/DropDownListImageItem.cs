using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.UI
{
    public class DropDownListImageItem : DropDownListItem
    {
        public DropDownListImageItem(Game game, Texture2D texture)
            : base(game, texture)
        {
            Height = texture.Height;
            Width = texture.Width;
        }
    }
}
