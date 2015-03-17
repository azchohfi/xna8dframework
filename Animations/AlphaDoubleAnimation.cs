namespace XNA8DFramework.Animations
{
    public abstract class AlphaDoubleAnimation : Animation
    {
        protected AlphaDoubleAnimation(IAlphaAnimatable animatable, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            Animatable = animatable;
        }

        /// <summary>
        /// Objeto a ser animado.
        /// </summary>
        public IAlphaAnimatable Animatable { get; protected set; }
    }
}
