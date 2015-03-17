using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.UI
{
	public class CheckBox : Button
	{
		public Texture2D TextureBack { get; set; }
		public Texture2D TextureChecked { get; set; }

		public AnimatableString AnimatableString { get; protected set; }

		bool _checked;
		public bool Checked
		{
			get
			{
				return _checked;
			}
			set
			{
				_checked = value;

				var oldWidth = Width;
				var oldHeight = Height;
				Texture = Checked ? TextureChecked : TextureBack;
				Width = oldWidth;
				Height = oldHeight;
			}
		}

		protected int Spacing = 10;

		public override double Alpha
		{
			get
			{
				return base.Alpha;
			}
			set
			{
				base.Alpha = value;
				if (AnimatableString != null)
					AnimatableString.Alpha = value;
			}
		}

		public override Vector2 Position
		{
			get
			{
				return base.Position;
			}
			set
			{
				base.Position = value;
				PositionString();
			}
		}

		public override Vector2 Origin
		{
			get
			{
				return base.Origin;
			}
			set
			{
				base.Origin = value;
				PositionString();
			}
		}

		protected virtual void PositionString()
		{
			if (AnimatableString != null)
			{
				AnimatableString.Position = Position + new Vector2(TextureBack.Width + Spacing, 0);
				AnimatableString.Origin = Origin + new Vector2(0, AnimatableString.Height / 2);
			}
		}

		public CheckBox(Game game, Texture2D textureBack, Texture2D textureChecked, string text, SpriteFont font, Vector2 position)
			: base(game, textureBack, position)
		{
			AnimatableString = new AnimatableString(game, text, font)
			{
			    Color = Color.White
			};
		    TextureBack = textureBack;
			TextureChecked = textureChecked;
			PositionString();
			Checked = false;
		}

		public override void Initialize()
		{
			base.Initialize();
			AnimatableString.Initialize();
			Width = TextureBack.Width + Spacing + AnimatableString.Width;
		}

		public override void Draw(GameTime gameTime)
		{
			if (!Visible)
				return;

			base.Draw(gameTime);

			AnimatableString.Draw(gameTime);
		}

		protected override void OnClick()
		{
			if (CancelClick)
			{
				CancelClick = !CancelClick;
				return;
			}
			Checked = !Checked;
			base.OnClick();
		}

		public override int Width
		{
			get
			{
				return TextureBack.Width + Spacing + AnimatableString.Width;
			}
		}
	}
}
