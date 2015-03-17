using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public class Scale2DAnimation : ScaleVector2Animation
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

        public Vector2 ScaleInicial { get; set; }
        public Vector2 ScaleFinal { get; set; }

        public Scale2DAnimation(IScale2DAnimatable animatable, Vector2 scaleInicial, Vector2 scaleFinal, int duracao)
            : this(animatable, scaleInicial, scaleFinal, duracao, 0)
        {
        }

        public Scale2DAnimation(IScale2DAnimatable animatable, Vector2 scaleInicial, Vector2 scaleFinal, int duracao, int aguardar)
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

            Vector2 scale2D = ScaleInicial + (ScaleFinal - ScaleInicial) * Progresso;

            if (Bounce)
            {
                var dif = (float) (Progresso*Math.Sin(Progresso*MathHelper.Pi));
                if (Math.Abs(dif) < 0.001)
                    dif = 0;

                scale2D.X += dif;
                scale2D.Y += dif;
            }
            Animatable.Scale2D = scale2D;
        }

        public override void Start()
        {
            base.Start();
            Animatable.Scale2D = ScaleInicial;
        }
    }
}
