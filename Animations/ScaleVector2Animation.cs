namespace XNA8DFramework.Animations
{
    public abstract class ScaleVector2Animation : Animation
    {
        protected ScaleVector2Animation(IScale2DAnimatable animatable, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            Animatable = animatable;
        }

        /// <summary>
        /// Objeto a ser animado.
        /// </summary>
        public IScale2DAnimatable Animatable { get; protected set; }
    }
}
