using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.UI
{
	public class DropDownList : Button
	{
		public override Vector2 Position
		{
			get
			{
				return base.Position;
			}
			set
			{
				base.Position = value;
				if (_speechBubble != null)
					_speechBubble.Position = value;
			}
		}

		public override int Height
		{
			get
			{
				return Rec.Height;
			}
			set
			{
				base.Height = value;
				_speechBubble.Height = value;
			}
		}

		public override int Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width = value;
				_speechBubble.Width = value;
				foreach (var item in _scrollerItems.Drawables)
				{
					var dropDownListItem = item as DropDownListItem;
					if (dropDownListItem != null)
						dropDownListItem.Width = value;
				}
				if (_scrollableRenderTarget != null)
					_scrollableRenderTarget.Width = value - SpacingX;
			}
		}

		public int MinHeight { get; set; }
		private int _maxHeight;
		public int MaxHeight
		{
			get
			{
				if (_scrollableRenderTarget != null)
					return _scrollableRenderTarget.Height;
				return _maxHeight;
			}
			set
			{
				_maxHeight = value;
				if (_scrollableRenderTarget != null)
					_scrollableRenderTarget.Height = value;
			}
		}

		public int SpacingX { get; set; }

		public static Guid SelectedGuid = Guid.NewGuid();

		public Guid MyGuid;

		readonly SpeechBubble _speechBubble;

		DropDownListItem _selectedItem;
		public DropDownListItem SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				if (value == null || _scrollerItems.Drawables.Contains(value))
				{
					LoseFocus();
					_selectedItem = value;
					if (Selected != null)
						Selected(this, EventArgs.Empty);
				}
				else
					throw new ArgumentException("Este elemento não pertence à lista de items.");
			}
		}

		readonly DGCScroller _scrollerItems;

		ScrollableRenderTarget _scrollableRenderTarget;

		readonly Texture2D _textureUp;
		readonly Texture2D _textureDown;

		public List<DropDownListItem> Drawables
		{
			get
			{
				return _scrollerItems.Drawables.OfType<DropDownListItem>().ToList();
			}
		}

		public event EventHandler GotFocus;

		public event EventHandler LostFocus;

		public event EventHandler Selected;

		public void AddItem(DropDownListItem dropDownListItem)
		{
			dropDownListItem.DropDownList = this;
			dropDownListItem.Width = Width;
			_scrollerItems.AddItem(dropDownListItem);
		}

		public void ClearItems()
		{
			_scrollerItems.Clear();
			_selectedItem = null;
		}

		public DropDownList(Game game, Vector2 position, Texture2D speechBubbleTexture, Texture2D textureUp, Texture2D textureDown)
			: base(game, (Texture2D)null, position)
		{
			MyGuid = Guid.NewGuid();

			_speechBubble = new SpeechBubble(Game, speechBubbleTexture, Position, "", null, Width, Height, 5);
			_scrollerItems = new DGCScroller(Game, null, null, 1000);

			_textureUp = textureUp;
			_textureDown = textureDown;

			SpacingX = 5;
			MinHeight = 30;
			MaxHeight = MinHeight * 5;
			Width = 300;
			Height = MinHeight;

			Scale = 1;
			Alpha = 1;
			Position = position;
		}

		public override void Initialize()
		{
			SpriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

			_scrollableRenderTarget = new ScrollableRenderTarget(Game, Width - SpacingX, MaxHeight, _scrollerItems,
				new ScrollableRenderTargetScroll(Game, _textureUp, _textureDown));

			_scrollerItems.Initialize();

			UpdateSelectedItemPos();

			_scrollableRenderTarget.Initialize();

			_speechBubble.Initialize();

			_selectedItem = null;

			_oldHasFocus = HasFocus;

			base.Initialize();
		}

#if !SILVERLIGHT
		protected override void Dispose (bool disposing)
		{
			if (_scrollableRenderTarget != null)
				_scrollableRenderTarget.Dispose();
			base.Dispose (disposing);
		}
#endif

		public void SetFocus()
		{
			if (SelectedGuid == MyGuid)
				return;
			SelectedGuid = MyGuid;
			OnGotFocus();
		}

		protected virtual void OnGotFocus()
		{
			if (GotFocus != null)
				GotFocus(this, EventArgs.Empty);
		}

		public void LoseFocus()
		{
			if (SelectedGuid != MyGuid)
				return;
			SelectedGuid = Guid.NewGuid();
			OnLostFocus();
		}

		protected virtual void OnLostFocus()
		{
			if (LostFocus != null)
				LostFocus(this, EventArgs.Empty);
		}

		private bool _oldHasFocus;

		public bool HasFocus
		{
			get
			{
				return SelectedGuid == MyGuid;
			}
		}

		public override void Update(GameTime gameTime)
		{
			if (!Enabled || !Game.IsActive)
				return;

			base.Update(gameTime);

			UpdateSelectedItemPos();

			if (_oldHasFocus && !HasFocus)
			{
				OnLostFocus();
			}

			if (HasFocus)
			{
				_scrollableRenderTarget.Update(gameTime);
				_Hits = true;
				_speechBubble.Height = Height + _scrollableRenderTarget.Height;
			}
			else
			{
				_speechBubble.Height = Height;
			}

			_oldHasFocus = HasFocus;
		}

		protected override void OnClick()
		{
			if (HasFocus)
				LoseFocus();
			else
				SetFocus();
			base.OnClick();
		}

		public override void Draw(GameTime gameTime)
		{
		    if (!Visible)
		        return;

			SpriteBatch.Begin();

			_speechBubble.Alpha = Alpha;
			_speechBubble.Draw(gameTime);

			if (_selectedItem != null)
			{
				_selectedItem.Alpha = Alpha;
				_selectedItem.Draw(gameTime);
			}

			base.Draw(gameTime);

			SpriteBatch.End();

			if (HasFocus)
			{
				_scrollableRenderTarget.Alpha = Alpha;
				_scrollableRenderTarget.Draw(gameTime);
			}
		}

		void UpdateSelectedItemPos()
		{
			var pos = Position;

			if (_selectedItem != null)
			{
				_selectedItem.Position = pos + new Vector2(SpacingX, Height/2 - _selectedItem.Origin.Y);
				_selectedItem.Width = Width - SpacingX;
				pos.Y += _selectedItem.Height;
			}
			else
			{
				pos.Y += MinHeight;
			}

			_scrollableRenderTarget.Position = pos + new Vector2(SpacingX, 0);
		}
	}
}
