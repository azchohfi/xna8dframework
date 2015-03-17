using System;
using Microsoft.Xna.Framework;
using XNA8DFramework.Animations;

namespace XNA8DFramework.UI
{
    public interface IScroll : IDrawable, IGameComponent, IUpdateable, IVector2Animatable, IAngleAnimatable, IAlphaAnimatable, IColorAnimatable, ISizeable
    {
        event EventHandler PercentageChanged;

        ScrollableRenderTarget ScrollContainer { get; set; }

        float PercentageX { get; set; }

        float PercentageY { get; set; }

        bool Horizontal { get; }

        bool Vertical { get; }

        int BarWidth { get; }

        int BarHeight { get; }

        bool Draging { get; }

        bool Hits { get; }
    }
}
