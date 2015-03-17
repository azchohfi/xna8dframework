using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameStateManagement
{
    public class TabHostScreen : GameScreen
    {
        int _selectedTabIndex;

        public int SelectedTabIndex
        {
            get
            {
                return _selectedTabIndex;
            }
            set
            {
                if (Screens != null)
                    Deselected(Screens[SelectedTabIndex]);
                _selectedTabIndex = value;
                if (Screens != null)
                    Activate(Screens[SelectedTabIndex], false);
            }
        }

        public TabScreen[] Screens { get; private set; }

        public TabHostScreen(params TabScreen[] screens)
        {
            SelectedTabIndex = 0;
            Screens = screens;
            foreach (var tabScreen in screens)
            {
                tabScreen.TabHostScreen = this;
            }
        }

        public override float TransitionPosition
        {
            protected set
            {
                base.TransitionPosition = value;
                foreach (TabScreen tabScreen in Screens)
                {
                    tabScreen.TransitionPosition = value;
                }
            }
        }

        void Activate(TabScreen tabScreen, bool instancePreserved)
        {
            if (!tabScreen.Activated)
            {
                tabScreen.Activated = true;
                if (tabScreen.content == null)
                    tabScreen.content = new ContentManager(ScreenManager.Game.Services, content.RootDirectory);
                tabScreen.Activate(instancePreserved);
            }
            tabScreen.Selected();
        }

        public override void Activate(bool instancePreserved)
        {
            Activate(Screens[SelectedTabIndex], instancePreserved);

            base.Activate(instancePreserved);
        }

        void Deselected(TabScreen tabScreen)
        {
            tabScreen.Deselected();
            if (tabScreen.AutoUnload && tabScreen.Activated)
                tabScreen.Unload();
        }

        public override void Deactivate()
        {
            Deselected(Screens[SelectedTabIndex]);

            foreach (TabScreen tabScreen in Screens)
            {
                if (tabScreen.Activated)
                    tabScreen.Deactivate();
            }
            base.Deactivate();
        }

        public override void Unload()
        {
            foreach (TabScreen tabScreen in Screens)
            {
                if(tabScreen.Activated)
                    tabScreen.Unload();
            }
            base.Unload();
        }

        public override void HandleInput(GameTime gameTime, InputService input)
        {
            Screens[SelectedTabIndex].HandleInput(gameTime, input);
            base.HandleInput(gameTime, input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            Screens[SelectedTabIndex].Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            Screens[SelectedTabIndex].Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
