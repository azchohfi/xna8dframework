using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace XNA8DFramework.Tests
{
    public static class Helpers
    {
        public static Texture2D Load(Game game, string path)
        {
            Texture2D ret;
            using (var titleStream = File.OpenRead(path))
            {
                ret = Texture2D.FromStream(game.GraphicsDevice, titleStream);
            }
            return ret;
        }
    }
}
