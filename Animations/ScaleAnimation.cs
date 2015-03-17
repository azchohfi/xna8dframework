using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public class ScaleAnimation : ScaleFloatAnimation
    {
        protected bool _Bounce = false;
        public bool Bounce
        {
            get
            {
                return _Bounce;
            }
            set
            {
                _Bounce = value;
            }
        }

        public float ScaleInicial { get; set; }
        public float ScaleFinal { get; set; }

        public ScaleAnimation(IScaleAnimatable animatable, float scaleInicial, float scaleFinal, TimeSpan duracao)
            : this(animatable, scaleInicial, scaleFinal, (int)duracao.TotalMilliseconds)
        {
        }

        public ScaleAnimation(IScaleAnimatable animatable, float scaleInicial, float scaleFinal, TimeSpan duracao, TimeSpan aguardar)
            : this(animatable, scaleInicial, scaleFinal, (int)duracao.TotalMilliseconds, (int)aguardar.TotalMilliseconds)
        {
        }

        public ScaleAnimation(IScaleAnimatable animatable, float scaleInicial, float scaleFinal, int duracao)
            : this(animatable, scaleInicial, scaleFinal, duracao, 0)
        {
        }

        public ScaleAnimation(IScaleAnimatable animatable, float scaleInicial, float scaleFinal, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            ScaleInicial = scaleInicial;
            ScaleFinal = scaleFinal;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Aguardando)
                return;

            float scale = ScaleInicial + (ScaleFinal - ScaleInicial) * Progresso;

            if (Bounce)
            {
                var dif = (float)(Progresso * Math.Sin(Progresso * MathHelper.Pi));
                if (Math.Abs(dif) < 0.001)
                    dif = 0;

                scale += dif;
            }
            Animatable.Scale = scale;
        }

        public override void Start()
        {
            base.Start();
            Animatable.Scale = ScaleInicial;
        }
    }
}
