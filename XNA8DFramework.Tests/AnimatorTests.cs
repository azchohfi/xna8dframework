using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using XNA8DFramework.Animations;
using Microsoft.Xna.Framework;

namespace XNA8DFramework.Tests
{
    [TestClass]
    public class AnimatorTests
    {
        static TestContext context;
        static GameStateManagementGame game;

        AnimatableTexture tex;

        Animator animator;

        [ClassInitialize]
        public static void InitializeStatic(TestContext testContext)
        {
            context = testContext;

            game = new GameStateManagementGame();
        }

        [ClassCleanup]
        public static void EndTests()
        {
            game.Exit();
        }

        [TestInitialize]
        public void Initialize()
        {
            game.RunOneFrame();

            tex = new AnimatableTexture(game, Helpers.Load(game, @"Textures\imagem.png"));
            tex.Initialize();

            animator = new Animator(game);
            animator.AutoRemove = true;
            animator.Initialize();
        }

        private void SimularTempo(TimeSpan time)
        {
            game.RunOneFrame();
            animator.Update(new GameTime(TimeSpan.Zero, time));
        }

        [TestMethod]
        public void SimpleVector2Animation_1000__0_0__100_100()
        {
            animator.AddAnimation(new SimpleVector2Animation(tex, Vector2.Zero, new Vector2(100, 100), 1000), true);
            SimularTempo(TimeSpan.FromMilliseconds(1000));
            Assert.AreEqual(new Vector2(100, 100), tex.Position);
        }

        [TestMethod]
        public void SimpleVector2Animation_2000__0_0__200_200()
        {
            animator.AddAnimation(new SimpleVector2Animation(tex, Vector2.Zero, new Vector2(200, 200), 2000), true);
            SimularTempo(TimeSpan.FromMilliseconds(2000));
            Assert.AreEqual(new Vector2(200, 200), tex.Position);
        }

        [TestMethod]
        public void SimpleVector2Animation_3000__100_0__200_200_NaMetade()
        {
            animator.AddAnimation(new SimpleVector2Animation(tex, new Vector2(100, 0), new Vector2(200, 200), 3000), true);
            SimularTempo(TimeSpan.FromMilliseconds(1500));
            Assert.AreEqual(new Vector2(150, 100), tex.Position);
        }

        [TestMethod]
        public void SimpleVector2Animation_3000__100_0__200_200_Completo()
        {
            animator.AddAnimation(new SimpleVector2Animation(tex, new Vector2(100, 0), new Vector2(200, 200), 3000), true);
            SimularTempo(TimeSpan.FromMilliseconds(3000));
            Assert.AreEqual(new Vector2(200, 200), tex.Position);
        }
    }
}
