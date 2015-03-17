using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.Animations;

namespace XNA8DFramework.UI
{
    public class Canvas : DrawableGameComponent, IScrollableCullable
    {
        public Vector2 Position { get; set; }

        public Color Color { get; set; }

        public double Alpha { get; set; }

        public float Angle { get; set; }

        public Vector2 Origin { get; set; }

        public virtual float Scale
        {
            get
            {
                return Scale2D.X;
            }
            set
            {
                Scale2D = new Vector2(value);
            }
        }

        public virtual Vector2 Scale2D { get; set; }

        public List<IDrawable8D> Drawables { get; private set; }

        public Canvas(Game game, Vector2 position)
            : base(game)
        {
            Color = Color.White;
            Alpha = 1;
            Scale = 1;
            Position = position;
            Drawables = new List<IDrawable8D>();
        }

        public void AddItem(IDrawable8D drawable)
        {
            Drawables.Add(drawable);
        }

        public void Remove(IDrawable8D drawable)
        {
            Drawables.Remove(drawable);
        }

        public void Clear()
        {
            Drawables.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            for (var i = 0; i < Drawables.Count; i++)
            {
                Drawables[i].Position += Position;
                Drawables[i].Update(gameTime);
                Drawables[i].Position -= Position;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawLimited(gameTime, 0, Height);

            base.Draw(gameTime);
        }

        public void DrawLimited(GameTime gameTime, float posDif, float limit)
        {
            for (var i = 0; i < Drawables.Count; i++)
            {
                Drawables[i].Position += Position;
                Drawables[i].Alpha = Alpha;
                // Culling
                //Todo
                //if (posY + Position.Y - Drawables[i].Height <= Drawables[i].Position.Y &&
                //    Drawables[i].Position.Y <= posY + Position.Y + limitHeight)
                Drawables[i].Draw(gameTime);
                Drawables[i].Position -= Position;
            }
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public int ScrollHeight()
        {
            return Height;
        }

        public int ScrollWidth()
        {
            return Width;
        }
    }
}
