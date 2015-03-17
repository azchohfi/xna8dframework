using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public class EaseVector2Animation : Vector2Animation
    {
        public Vector2 PosInicial { get; set; }
        public Vector2 PosFinal { get; set; }

        public double Power { get; set; }

        public EaseVector2Animation(IVector2Animatable animatable, Vector2 posInicial, Vector2 posFinal, int duracao, double power)
            : this(animatable, posInicial, posFinal, duracao, power, 0)
        {
        }

        public EaseVector2Animation(IVector2Animatable animatable, Vector2 posInicial, Vector2 posFinal, int duracao, double power, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            PosInicial = posInicial;
            PosFinal = posFinal;
            Power = power;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Aguardando)
                return;

            Animatable.Position = PosInicial + (PosFinal - PosInicial) * (float)Math.Pow(Progresso, Power);
        }

        public override void Start()
        {
            base.Start();
            Animatable.Position = PosInicial;
        }
    }
}
