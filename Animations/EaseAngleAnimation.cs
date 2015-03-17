using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public class EaseAngleAnimation : AngleFloatAnimation
    {
        public float AngleInicial { get; set; }
        public float AngleFinal { get; set; }

        public double Power { get; set; }

        public EaseAngleAnimation(IAngleAnimatable animatable, float angleInicial, float angleFinal, int duracao, double power)
            : this(animatable, angleInicial, angleFinal, duracao, power, 0)
        {
        }

        public EaseAngleAnimation(IAngleAnimatable animatable, float angleInicial, float angleFinal, int duracao, double power, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            AngleInicial = angleInicial;
            AngleFinal = angleFinal;
            Power = power;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Aguardando)
                return;

            Animatable.Angle = AngleInicial + (AngleFinal - AngleInicial) * (float)Math.Pow(Progresso, Power);
        }

        public override void Start()
        {
            base.Start();
            Animatable.Angle = AngleInicial;
        }
    }
}
