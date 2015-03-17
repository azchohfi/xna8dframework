using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace GameStateManagement
{
    public abstract class TabScreen
    {
        protected TabScreen()
            : this(false)
        {
        }

        protected TabScreen(bool autoUnload)
        {
            Activated = false;
            AutoUnload = autoUnload;
        }

        public TabHostScreen TabHostScreen { get; internal set; }

        public ContentManager content;

        public bool AutoUnload { get; set; }

        public object Tag { get; set; }

        public float TransitionPosition
        {
            get { return _transitionPosition; }
            internal set { _transitionPosition = value; }
        }

        float _transitionPosition = 1;

        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        internal bool Activated;
        
        public virtual void Activate(bool instancePreserved) { }

        public virtual void Deactivate() { }

        public virtual void Selected() { }

        public virtual void Deselected() { }

        public bool IsSelected
        {
            get
            {
                if (TabHostScreen == null)
                    return false;
                return TabHostScreen.Screens[TabHostScreen.SelectedTabIndex] == this;
            }
        }

        public virtual void Unload()
        {
            if (content != null)
            {
                content.Unload();
                content = null;
            }
            Activated = false;
        }

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) { }

        public virtual void HandleInput(GameTime gameTime, InputService input) { }

        public virtual void Draw(GameTime gameTime) { }
    }
}
