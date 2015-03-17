using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement
{
    public class MenuPickerHorizontal : MenuEntry
    {
        readonly Texture2D _texture;

        public override Vector2 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                UpdatePositions();
            }
        }

        private void UpdatePositions()
        {
            var pos = Position;
            foreach (var menuImageEntry in MenuImageEntries)
            {
                menuImageEntry.Position = pos;
                pos.X += menuImageEntry.GetWidth(null) + Spacing;
            }
        }

        public float Spacing { get; set; }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                if (MenuImageEntries != null)
                {
                    foreach (MenuImageEntry menuImageEntry in MenuImageEntries)
                    {
                        menuImageEntry.Enabled = value;
                    }
                }
                base.Enabled = value;
            }
        }

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                if (MenuImageEntries != null)
                {
                    foreach (MenuImageEntry menuImageEntry in MenuImageEntries)
                    {
                        menuImageEntry.Visible = value;
                    }
                }
                base.Visible = value;
            }
        }

        public int SelectedIndex { get; set; }

        public List<MenuImageEntry> MenuImageEntries;
        string[] _paths;
        readonly MenuScreen _menuScreen;

        public void UpdatePaths(params string[] paths)
        {
            _paths = paths;
            if (MenuImageEntries != null && MenuImageEntries.Count > 0)
            {
                foreach (MenuImageEntry menuEntry in MenuImageEntries)
                {
                    _menuScreen.MenuEntries.Remove(menuEntry);
                }
                MenuImageEntries.Clear();
            }
            _menuScreen.MenuEntries.Remove(this);
            Initialize();
            _menuScreen.MenuEntries.Add(this);
        }

        public MenuPickerHorizontal(MenuScreen menuScreen, Vector2 position, Texture2D selectedBgTex, params string[] paths)
            : base(position)
        {
            SelectedIndex = 0;
            _menuScreen = menuScreen;
            _texture = selectedBgTex;
            Spacing = 10;
            MenuImageEntries = new List<MenuImageEntry>();
            _paths = paths;
            Position = position;
        }

        public void Initialize()
        {
            var pos = Position;
            var i = 0;
            foreach (var path in _paths)
            {
                var tex = _menuScreen.content.Load<Texture2D>(path);
                var menuEntry = new MenuImageEntry(tex, pos) {Tag = i};
                menuEntry.Selected += menuEntry_Selected;
                pos.X += tex.Width + Spacing;
                MenuImageEntries.Add(menuEntry);
                _menuScreen.MenuEntries.Add(menuEntry);
                i++;
            }
            UpdatePositions();
        }

        void menuEntry_Selected(object sender, EventArgs e)
        {
            var menuImageEntry = sender as MenuImageEntry;
            if (menuImageEntry != null)
                SelectedIndex = (int)menuImageEntry.Tag;
            OnSelectEntry();
        }

        public override void Draw(MenuScreen screen, GameTime gameTime)
        {
            if (!Visible)
                return;

#if SILVERLIGHT
            var color = new Color(Color.White, MathHelper.Clamp(screen.TransitionAlpha, 0, 1));
#else
            var color = Color.White * screen.TransitionAlpha;
#endif

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            Vector2 origin = new Vector2(MenuImageEntries[SelectedIndex].GetWidth(null), MenuImageEntries[SelectedIndex].GetHeight(null)) / 2
                - new Vector2(_texture.Width, _texture.Height) / 2;

            spriteBatch.Draw(_texture, MenuImageEntries[SelectedIndex].Position + origin, color);
        }

        public override int GetHeight(MenuScreen screen)
        {
            return _texture.Height;
        }

        public override int GetWidth(MenuScreen screen)
        {
            return _texture.Width;
        }
    }
}
