using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNA8DFramework.Animations;

namespace XNA8DFramework.Particles
{
    public class ParticlesManager : DrawableGameComponent
    {
        private readonly List<Particle> _particles;

        private static readonly Queue<Particle> FreeItems;

        public static int UsedItemsCount
        {
            get { return FreeItems.Count; }
        }

        private Animator _animator;

        static ParticlesManager()
        {
            FreeItems = new Queue<Particle>();
        }

        public ParticlesManager(Game game)
            : base(game)
        {
            _particles = new List<Particle>();
        }

        public override void Initialize()
        {
            _animator = new Animator(Game) { AutoRemove = true };
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _animator.Update(gameTime);

            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                _particles[i].Update(gameTime);
                if (_particles[i].Finished)
                {
                    var particle = _particles[i];
                    _particles.RemoveAt(i);
                    FreeItems.Enqueue(particle);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (var particle in _particles)
            {
                particle.Draw(gameTime);
            }
        }

        public Particle CreateParticle(IDrawable8D drawableTex)
        {
            if (FreeItems.Count == 0)
            {
                return new Particle(Game, drawableTex);
            }
            Particle particle = FreeItems.Dequeue();
            particle.DrawableTex = drawableTex;
            particle.Init();
            return particle;
        }

        public Particle CreateParticle(Texture2D texture, Vector2 position)
        {
            if (FreeItems.Count == 0)
            {
                return new Particle(Game, texture, position);
            }
            Particle particle = FreeItems.Dequeue();
            particle.DrawableTex = new AnimatableTexture(Game, texture, position);
            particle.Init();
            particle._shouldInitialize = true;
            return particle;
        }

        [Obsolete]
        public Particle CreateParticle(string texturePath, Vector2 position)
        {
            if (FreeItems.Count == 0)
            {
                return new Particle(Game, texturePath, position);
            }
            Particle particle = FreeItems.Dequeue();
            particle.DrawableTex = new AnimatableTexture(Game, texturePath, position);
            particle.Init();
            particle._shouldInitialize = true;
            return particle;
        }

        public void AddParticle(Particle particle)
        {
            particle.StartAnims(_animator);
            _particles.Add(particle);
        }

        public void RemoveParticle(Particle particle)
        {
            particle.RemoveAnims(_animator);
            _particles.Remove(particle);
            FreeItems.Enqueue(particle);
        }

        public int Count()
        {
            return _particles.Count;
        }

        public void Clear()
        {
            _animator.Clear();
            _particles.Clear();
            FreeItems.Clear();
        }
    }
}