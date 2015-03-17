using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public enum Ease
    {
        In,
        Out,
        InOut
    }
    public class BounceAnimation : ScaleFloatAnimation
    {
        public Ease Ease { get; set; }

        public BounceAnimation(IScaleAnimatable animatable, int duracao, Ease ease)
            : this(animatable, duracao, ease, 0)
        {
        }

        public BounceAnimation(IScaleAnimatable animatable, int duracao, Ease ease, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            Ease = ease;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Aguardando)
                return;

            if (Ease == Ease.In)
                Animatable.Scale = EaseInBounce(0, 1, Progresso);
            else if (Ease == Ease.Out)
                Animatable.Scale = EaseOutBounce(0, 1, Progresso);
            else if (Ease == Ease.InOut)
                Animatable.Scale = EaseInOutBounce(0, 1, Progresso);
        }

        private float EaseInBounce(float start, float end, float value)
        {
            end -= start;
            const float d = 1f;
            return end - EaseOutBounce(0, end, d - value) + start;
        }

        private static float EaseOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < (1 / 2.75f))
            {
                return end * (7.5625f * value * value) + start;
            }
            if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return end * (7.5625f * (value) * value + .75f) + start;
            }
            if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return end * (7.5625f * (value) * value + .9375f) + start;
            }
            
            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }

        private float EaseInOutBounce(float start, float end, float value)
        {
            end -= start;
            const float d = 1f;
            if (value < d / 2)
                return EaseInBounce(0, end, value * 2) * 0.5f + start;
            return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
        }

        public override void Start()
        {
            base.Start();
            Animatable.Scale = 1;
        }
    }
}

