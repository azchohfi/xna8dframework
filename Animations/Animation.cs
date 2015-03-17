using System;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Animations
{
    public abstract class Animation
    {
        public event EventHandler EventoTerminouAnim;
        public event EventHandler EventoAnimFrame;

        /// <summary>
        /// Indica de 0.0 a 1.0 o progresso da animação.
        /// </summary>
        public float Progresso { get; protected set; }

        /// <summary>
        /// Objeto a ser animado, com referencia do tipo Object
        /// </summary>
        public object AnimatableG { get; protected set; }

        /// <summary>
        /// Tempo que demorará até iniciar a animação, em milisegundos.
        /// </summary>
        public int Aguardar { get; set; }

        /// <summary>
        /// Indica se a animação esta aguardando para começar
        /// </summary>
        public bool Aguardando
        {
            get;
            protected set;
        }

        /// <summary>
        /// Tempo total da animação, em milisegundos.
        /// </summary>
        public int Duracao { get; protected set; }

        /// <summary>
        /// Tempo corrente da animação.
        /// </summary>
        public TimeSpan TotalAnimTime { get; protected set; }

        /// <summary>
        /// Tempo corrente da animação.
        /// </summary>
        public int RepeatCount { get; set; }

        public bool Terminou
        {
            get;
            private set;
        }

        protected Animation(object animatableG, int duracao, int aguardar)
        {
            RepeatCount = 0;
            AnimatableG = animatableG;
            Duracao = duracao;
            Aguardar = aguardar;
        }

        public virtual void Update(GameTime gameTime)
        {
            TotalAnimTime += gameTime.ElapsedGameTime;

            if (TotalAnimTime.TotalMilliseconds < Aguardar)
                return;

            Aguardando = false;

            Progresso = (float)((TotalAnimTime.TotalMilliseconds - Aguardar) / Duracao);
            if (Progresso > 1)
            {
                if (RepeatCount == -1 || RepeatCount > 0)
                {
                    if (RepeatCount > 0)
                        RepeatCount--;
                    Restart();
                }
                else
                {
                    Progresso = 1;
                    Terminou = true;
                }
            }
        }

        internal void Update2(GameTime gameTime)
        {
            if (TotalAnimTime.TotalMilliseconds < Aguardar)
                return;

            if (EventoAnimFrame != null)
                EventoAnimFrame(this, EventArgs.Empty);
            if (Terminou)
            {
                if (EventoTerminouAnim != null)
                    EventoTerminouAnim(this, EventArgs.Empty);
            }
        }

        public virtual void Start()
        {
            Aguardando = true;
            TotalAnimTime = TimeSpan.Zero;
        }

        public virtual void Restart()
        {
            Start();
            Terminou = false;
        }
    }
}
