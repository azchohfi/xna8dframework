using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public interface IAnimatableOrdable : IDrawable
    {
        Vector2 Centro { get; }
        Vector2 Position { get; }
    }
}
