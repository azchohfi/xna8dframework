namespace XNA8DFramework.Animations
{
    public abstract class ScaleFloatAnimation : Animation
    {
        protected ScaleFloatAnimation(IScaleAnimatable animatable, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            Animatable = animatable;
        }

        /// <summary>
        /// Objeto a ser animado.
        /// </summary>
        public IScaleAnimatable Animatable { get; protected set; }
    }
}
