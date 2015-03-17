using System;

namespace XNA8DFramework
{
    public class AnimUpdateEventArgs : EventArgs
    {
        public AnimUpdateEventArgs(int anim)
        {
            Anim = anim;
        }
        public int Anim { get; private set; }
    }
}
