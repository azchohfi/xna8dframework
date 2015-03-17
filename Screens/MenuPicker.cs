using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameStateManagement
{
    public class MenuPicker : MenuEntry
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
            var i = 0;
            foreach (var menuImageEntry in MenuImageEntries)
            {
                menuImageEntry.Position = pos;
                pos.X += menuImageEntry.GetWidth(null) + SpacingX;
                i++;
                if (i % CountX == 0)
                {
                    pos.Y += menuImageEntry.GetHeight(null) + SpacingY;
                    pos.X = Position.X;
                }
            }
        }

        public int CountX { get; set; }

        public float SpacingX { get; set; }
        public float SpacingY { get; set; }

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
        readonly List<Texture2D> _textures;
        readonly MenuScreen _menuScreen;

        public MenuPicker(MenuScreen menuScreen, Vector2 position, Texture2D selectedBgTex, Texture2D tex, int count)
            : base(position)
        {
            SelectedIndex = 0;
            _menuScreen = menuScreen;
            _texture = selectedBgTex;
            SpacingX = 10;
            SpacingY = 10;
            MenuImageEntries = new List<MenuImageEntry>();
            _textures = new List<Texture2D>();
            for (var i = 0; i < count; i++)
            {
                _textures.Add(tex);
            }
            Position = position;
        }

        public void Initialize()
        {
            var pos = Position;
            var i = 0;
            foreach (var tex in _textures)
            {
                var menuEntry = new MenuImageEntry(tex, pos)
                    {
                        Tag = i,
                        Origin = new Vector2(tex.Width, tex.Height)/2
                    };
                menuEntry.Selected += menuEntry_Selected;
                pos.X += tex.Width + SpacingX;
                MenuImageEntries.Add(menuEntry);
                _menuScreen.MenuEntries.Add(menuEntry);
                i++;
                if (i % CountX == 0)
                {
                    pos.Y += tex.Height + SpacingY;
                    pos.X = Position.X;
                }
            }
            UpdatePositions();
        }

        void menuEntry_Selected(object sender, EventArgs e)
        {
            var menuImageEntry = sender as MenuImageEntry;
            if (menuImageEntry != null)
                SelectedIndex = (int) menuImageEntry.Tag;
            OnSelectEntry();
        }

        public override void Draw(MenuScreen screen, GameTime gameTime)
        {
            if (!Visible)
                return;

#if SILVERLIGHT
            var color = new Color(Color, MathHelper.Clamp(screen.TransitionAlpha * (float)Alpha, 0, 1));
#else
            var color = Color * screen.TransitionAlpha * (float)Alpha;
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
