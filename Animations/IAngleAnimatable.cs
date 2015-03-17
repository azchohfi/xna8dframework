using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public interface IAngleAnimatable
    {
        float Angle { get; set; }
        Vector2 Origin { get; set; }
    }
}
