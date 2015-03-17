using System;

namespace XNA8DFramework.Animations
{
    public class VoidAnimation : Animation
    {
        public VoidAnimation(TimeSpan duracao)
            : this((int)duracao.TotalMilliseconds)
        {
        }

        public VoidAnimation(TimeSpan duracao, TimeSpan aguardar)
            : this((int)duracao.TotalMilliseconds, (int)aguardar.TotalMilliseconds)
        {
        }

        public VoidAnimation(int duracao)
            : this(duracao, 0)
        {
        }

        public VoidAnimation(int duracao, int aguardar)
            : base(null, duracao, aguardar)
        {
        }
    }
}
