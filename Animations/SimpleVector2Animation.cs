using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public class SimpleVector2Animation : Vector2Animation
    {
        public Vector2 PosInicial { get; set; }
        public Vector2 PosFinal { get; set; }

        public SimpleVector2Animation(IVector2Animatable animatable, Vector2 posInicial, Vector2 posFinal, TimeSpan duracao)
            : this(animatable, posInicial, posFinal, (int)duracao.TotalMilliseconds, 0)
        {
        }

        public SimpleVector2Animation(IVector2Animatable animatable, Vector2 posInicial, Vector2 posFinal, TimeSpan duracao, TimeSpan aguardar)
            : this(animatable, posInicial, posFinal, (int)duracao.TotalMilliseconds, (int)aguardar.TotalMilliseconds)
        {
        }

        public SimpleVector2Animation(IVector2Animatable animatable, Vector2 posInicial, Vector2 posFinal, int duracao)
            : this(animatable, posInicial, posFinal, duracao, 0)
        {
        }

        public SimpleVector2Animation(IVector2Animatable animatable, Vector2 posInicial, Vector2 posFinal, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            PosInicial = posInicial;
            PosFinal = posFinal;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(Aguardando)
                return;

            Animatable.Position = PosInicial + (PosFinal - PosInicial) * Progresso;
        }

        public override void Start()
        {
            base.Start();
            Animatable.Position = PosInicial;
        }
    }
}
