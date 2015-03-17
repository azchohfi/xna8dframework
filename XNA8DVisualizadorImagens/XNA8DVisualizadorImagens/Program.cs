using System;
using System.Windows.Forms;

namespace XNA8DVisualizadorImagens
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("Entre com algum arquivo.");
                return;
            }
            using (Game1 game = new Game1(args))
            {
                game.Run();
            }
        }
    }
#endif
}

