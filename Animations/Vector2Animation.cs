namespace XNA8DFramework.Animations
{
    public abstract class Vector2Animation : Animation
    {
        protected Vector2Animation(IVector2Animatable animatable, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            Animatable = animatable;
        }
        /// <summary>
        /// Objeto a ser animado.
        /// </summary>
        public IVector2Animatable Animatable { get; protected set; }
    }
}
