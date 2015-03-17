using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public class AngleAnimation : AngleFloatAnimation
    {
        public float AngleInicial { get; set; }
        public float AngleFinal { get; set; }

        public AngleAnimation(IAngleAnimatable animatable, Vector2 origin, float angleInicial, float angleFinal, int duracao)
            : this(animatable, origin, angleInicial, angleFinal, duracao, 0)
        {
        }

        public AngleAnimation(IAngleAnimatable animatable, Vector2 origin, float angleInicial, float angleFinal, int duracao, int aguardar)
            : this(animatable, angleInicial, angleFinal, duracao, aguardar)
        {
            Animatable.Origin = origin;
        }

        public AngleAnimation(IAngleAnimatable animatable, float angleInicial, float angleFinal, TimeSpan duracao)
            : this(animatable, angleInicial, angleFinal, (int)duracao.TotalMilliseconds)
        {
        }

        public AngleAnimation(IAngleAnimatable animatable, float angleInicial, float angleFinal, TimeSpan duracao, TimeSpan aguardar)
            : this(animatable, angleInicial, angleFinal, (int)duracao.TotalMilliseconds, (int)aguardar.TotalMilliseconds)
        {
        }

        public AngleAnimation(IAngleAnimatable animatable, float angleInicial, float angleFinal, int duracao)
            : this(animatable, angleInicial, angleFinal, duracao, 0)
        {
        }

        public AngleAnimation(IAngleAnimatable animatable, float angleInicial, float angleFinal, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            AngleInicial = angleInicial;
            AngleFinal = angleFinal;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Aguardando)
                return;

            Animatable.Angle = AngleInicial + (AngleFinal - AngleInicial) * Progresso;
        }

        public override void Start()
        {
            base.Start();
            Animatable.Angle = AngleInicial;
        }
    }
}
