namespace XNA8DFramework.Animations
{
    public abstract class AngleFloatAnimation : Animation
    {
        protected AngleFloatAnimation(IAngleAnimatable animatable, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            Animatable = animatable;
        }

        /// <summary>
        /// Objeto a ser animado.
        /// </summary>
        public IAngleAnimatable Animatable { get; protected set; }
    }
}
