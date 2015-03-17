using Microsoft.Xna.Framework;

namespace XNA8DFramework.UI
{
    public interface IScrollableCullable : IScrollable
    {
        void DrawLimited(GameTime gameTime, float posDif, float limit);
    }
}
