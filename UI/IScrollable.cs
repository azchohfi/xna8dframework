using XNA8DFramework.Animations;

namespace XNA8DFramework.UI
{
    public interface IScrollable : IDrawable8D
    {
        int ScrollHeight();
        int ScrollWidth();
    }
}
