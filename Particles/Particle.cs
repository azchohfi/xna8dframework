using System;
using Microsoft.Xna.Framework;
using XNA8DFramework.Animations;
using Microsoft.Xna.Framework.Graphics;

namespace XNA8DFramework.Particles
{
    public class Particle : DrawableGameComponent
    {
        public IDrawable8D DrawableTex;

        public double ParticleAlpha { get; set; }
        public float ParticleScale { get; set; }
        public float ParticleAngle { get; set; }
        public float ParticleDistance { get; set; }        
        public Vector2 ParticleSpread { get; set; }
        public Vector2 ParticleGravity { get; set; }

        public TimeSpan DurationAlpha { get; set; }
        public TimeSpan DurationWaitAlpha { get; set; }

        public TimeSpan DurationPos { get; set; }
        public TimeSpan DurationAngle { get; set; }
        public TimeSpan DurationScale { get; set; }

        public event EventHandler EventoTerminouAnim;

        internal bool _shouldInitialize;
        protected Texture2D Texture2D;
        protected Vector2 Vector2;

        public object Tag;

        protected bool _Finished;
        public virtual bool Finished
        {
            get
            {
                return _Finished;
            }
        }

        [Obsolete]
        protected internal Particle(Game game, string texturePath, Vector2 position)
            : this(game, new AnimatableTexture(game, texturePath, position))
        {
            _shouldInitialize = true;
        }

        protected internal Particle(Game game, Texture2D texture, Vector2 position)
            : this(game, new AnimatableTexture(game, texture, position))
        {
            _shouldInitialize = true;
        }

        protected internal Particle(Game game, IDrawable8D drawableTex)
            : base(game)
        {
            DrawableTex = drawableTex;
            Init();
        }

        internal void Init()
        {
            ParticleAlpha = 0;
            ParticleAngle = DrawableTex.Angle;
            ParticleScale = DrawableTex.Scale;
            ParticleDistance = 0;
            ParticleSpread = Vector2.Zero;
            ParticleGravity = Vector2.Zero;
            DurationPos = TimeSpan.FromMilliseconds(1500);
            DurationAngle = TimeSpan.FromMilliseconds(1500);
            DurationScale = TimeSpan.FromMilliseconds(800);
            DurationAlpha = TimeSpan.FromMilliseconds(800);
            DurationWaitAlpha = TimeSpan.FromMilliseconds(700);
            EventoTerminouAnim = null;

            _Finished = false;
        }

        public override void Initialize()
        {
            if (_shouldInitialize)
                DrawableTex.Initialize();
            base.Initialize();

            DrawableTex.Origin = new Vector2(DrawableTex.Width, DrawableTex.Height) / 2;
            DrawableTex.Position -= DrawableTex.Origin;
        }

        internal virtual void StartAnims(Animator animator)
        {
            Animation animPos = CreateAnimPosition(animator);
            animPos.Start();
            animator.AddAnimation(animPos);

            Animation animAlpha = CreateAnimAlpha(animator);
            animAlpha.Start();
            animator.AddAnimation(animAlpha);

            if (DrawableTex.Angle != ParticleAngle)
            {
                Animation animAngle = CreateAnimAngle(animator);
                animAngle.Start();
                animator.AddAnimation(animAngle);
            }

            if (DrawableTex.Scale != ParticleScale)
            {
                Animation animScale = CreateAnimScale(animator);
                animScale.Start();
                animator.AddAnimation(animScale);
            }
        }

        internal virtual void RemoveAnims(Animator animator)
        {
            animator.RemoveAllFrom(DrawableTex);
        }

        protected virtual Animation CreateAnimPosition(Animator animator)
        {
            double angle = ScrollableGame.Rnd.NextDouble() * Math.PI * 2;

            var spreadX = (float)(ParticleSpread.X * (1 + ScrollableGame.Rnd.NextDouble()));
            var spreadY = (float)(ParticleSpread.Y * (1 + ScrollableGame.Rnd.NextDouble()));
            Vector2 posFinal = DrawableTex.Position + new Vector2((float)Math.Cos(angle) * ParticleDistance + (spreadX * 2), (float)Math.Sin(angle) * ParticleDistance + (spreadY * 2));

            Animation animPos = new SimpleVector2Animation(DrawableTex, DrawableTex.Position, posFinal, DurationPos);
            if (ParticleGravity.Length() > 0)
                animPos.EventoAnimFrame += animPos_EventoAnimFrame;
            return animPos;
        }

        protected virtual Animation CreateAnimAlpha(Animator animator)
        {
            Animation animAlpha = new AlphaAnimation(DrawableTex, DrawableTex.Alpha, ParticleAlpha, DurationAlpha, DurationWaitAlpha);
            animAlpha.EventoTerminouAnim += (sender, e) =>
            {
                _Finished = true;
                OnEventoTerminouAnim();
            };
            return animAlpha;
        }

        protected void OnEventoTerminouAnim()
        {
            if (EventoTerminouAnim != null)
                EventoTerminouAnim(this, EventArgs.Empty);
        }

        protected virtual Animation CreateAnimAngle(Animator animator)
        {
            return new AngleAnimation(DrawableTex, DrawableTex.Angle, ParticleAngle, DurationAngle);
        }

        protected virtual Animation CreateAnimScale(Animator animator)
        {
            return new ScaleAnimation(DrawableTex, DrawableTex.Scale, ParticleScale, DurationScale);
        }

        private void animPos_EventoAnimFrame(object sender, EventArgs e)
        {
            var anim = sender as SimpleVector2Animation;
            if (anim != null)
                anim.PosFinal += ParticleGravity;
        }

        public override void Update(GameTime gameTime)
        {
            DrawableTex.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawableTex.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
