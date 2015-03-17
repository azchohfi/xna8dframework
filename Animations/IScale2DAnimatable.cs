using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public interface IScale2DAnimatable : IScaleAnimatable
    {
        Vector2 Scale2D { get; set; }
    }
}
