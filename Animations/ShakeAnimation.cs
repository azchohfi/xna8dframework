using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public class ShakeAnimation : Vector2Animation
    {
        public Vector2 PosInicial { get; set; }

        public Vector2 ShakeAmmount { get; set; }

        public ShakeAnimation(IVector2Animatable animatable, Vector2 posInicial, Vector2 shakeAmmount, TimeSpan duracao)
            : this(animatable, posInicial, shakeAmmount, (int)duracao.TotalMilliseconds, 0)
        {
        }

        public ShakeAnimation(IVector2Animatable animatable, Vector2 posInicial, Vector2 shakeAmmount, TimeSpan duracao, TimeSpan aguardar)
            : this(animatable, posInicial, shakeAmmount, (int)duracao.TotalMilliseconds, (int)aguardar.TotalMilliseconds)
        {
        }

        public ShakeAnimation(IVector2Animatable animatable, Vector2 posInicial, Vector2 shakeAmmount, int duracao)
            : this(animatable, posInicial, shakeAmmount, duracao, 0)
        {
        }

        public ShakeAnimation(IVector2Animatable animatable, Vector2 posInicial, Vector2 shakeAmmount, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            PosInicial = posInicial;
            ShakeAmmount = shakeAmmount;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(Aguardando)
                return;
            var angle = (float)(ScrollableGame.Rnd.NextDouble() * Math.PI * 2);
            var ammount = (float)ScrollableGame.Rnd.NextDouble();
            Animatable.Position = PosInicial + new Vector2((float)Math.Cos(angle) * ShakeAmmount.X, (float)Math.Sin(angle) * ShakeAmmount.Y) * ammount;
        }

        public override void Start()
        {
            base.Start();
            Animatable.Position = PosInicial;
        }
    }
}
