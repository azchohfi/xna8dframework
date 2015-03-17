using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.UI
{
    public abstract class DropDownListItem : Button
    {
        internal DropDownList DropDownList { private get; set; }

        protected DropDownListItem(Game game)
            : base(game, (Texture2D)null, Vector2.Zero)
        {
        }

        protected DropDownListItem(Game game, Texture2D texture)
            : base(game, texture, Vector2.Zero)
        {
        }

        protected override void OnClick()
        {
            if (DropDownList != null)
                DropDownList.SelectedItem = this;
            base.OnClick();
        }
    }
}
