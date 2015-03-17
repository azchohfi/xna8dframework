using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public class EaseScaleAnimation : ScaleFloatAnimation
    {
        public float ScaleInicial { get; set; }
        public float ScaleFinal { get; set; }

        public double Power { get; set; }

        public EaseScaleAnimation(IScaleAnimatable animatable, float scaleInicial, float scaleFinal, int duracao, double power)
            : this(animatable, scaleInicial, scaleFinal, duracao, power, 0)
        {
        }

        public EaseScaleAnimation(IScaleAnimatable animatable, float scaleInicial, float scaleFinal, int duracao, double power, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            ScaleInicial = scaleInicial;
            ScaleFinal = scaleFinal;
            Power = power;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Aguardando)
                return;

            Animatable.Scale = ScaleInicial + (ScaleFinal - ScaleInicial) * (float)Math.Pow(Progresso, Power);
        }

        public override void Start()
        {
            base.Start();
            Animatable.Scale = ScaleInicial;
        }
    }
}
