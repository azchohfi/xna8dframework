namespace XNA8DFramework.Animations
{
    public abstract class ColorAnimation : Animation
    {
        protected ColorAnimation(IColorAnimatable animatable, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            Animatable = animatable;
        }

        /// <summary>
        /// Objeto a ser animado.
        /// </summary>
        public IColorAnimatable Animatable { get; protected set; }
    }
}
