using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public class AlphaAnimation : AlphaDoubleAnimation
    {
        public double AlphaInicial { get; set; }
        public double AlphaFinal { get; set; }

        public AlphaAnimation(IAlphaAnimatable animatable, double alphaInicial, double alphaFinal, TimeSpan duracao)
            : this(animatable, alphaInicial, alphaFinal, (int)duracao.TotalMilliseconds)
        {
        }

        public AlphaAnimation(IAlphaAnimatable animatable, double alphaInicial, double alphaFinal, TimeSpan duracao, TimeSpan aguardar)
            : this(animatable, alphaInicial, alphaFinal, (int)duracao.TotalMilliseconds, (int)aguardar.TotalMilliseconds)
        {
        }

        public AlphaAnimation(IAlphaAnimatable animatable, double alphaInicial, double alphaFinal, int duracao)
            : this(animatable, alphaInicial, alphaFinal, duracao, 0)
        {
        }

        public AlphaAnimation(IAlphaAnimatable animatable, double alphaInicial, double alphaFinal, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            AlphaInicial = alphaInicial;
            AlphaFinal = alphaFinal;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Aguardando)
                return;

            Animatable.Alpha = AlphaInicial + (AlphaFinal - AlphaInicial) * Progresso;
        }

        public override void Start()
        {
            base.Start();
            Animatable.Alpha = AlphaInicial;
        }
    }
}
