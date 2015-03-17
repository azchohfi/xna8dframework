using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.Animations
{
    public class SimpleColorAnimation : ColorAnimation
    {
        public Color ColorInicial { get; set; }
        public Color ColorFinal { get; set; }

        public SimpleColorAnimation(IColorAnimatable animatable, Color colorInicial, Color colorFinal, int duracao)
            : this(animatable, colorInicial, colorFinal, duracao, 0)
        {
        }

        public SimpleColorAnimation(IColorAnimatable animatable, Color colorInicial, Color colorFinal, int duracao, int aguardar)
            : base(animatable, duracao, aguardar)
        {
            ColorInicial = colorInicial;
            ColorFinal = colorFinal;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Aguardando)
                return;

            Color color = Color.White;
            color.R = (byte)(ColorInicial.R + (ColorFinal.R - ColorInicial.R) * Progresso);
            color.G = (byte)(ColorInicial.G + (ColorFinal.G - ColorInicial.G) * Progresso);
            color.B = (byte)(ColorInicial.B + (ColorFinal.B - ColorInicial.B) * Progresso);
            color.A = (byte)(ColorInicial.A + (ColorFinal.A - ColorInicial.A) * Progresso);

            Animatable.Color = color;
        }

        public override void Start()
        {
            base.Start();
            Animatable.Color = ColorInicial;
        }
    }
}
