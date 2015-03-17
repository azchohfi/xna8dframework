using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public interface IDrawable8D : IDrawable, IUpdateable, IGameComponent, IVector2Animatable, IAngleAnimatable, IScale2DAnimatable, IAlphaAnimatable, ISizeable, IColorAnimatable
    {
    }
}
