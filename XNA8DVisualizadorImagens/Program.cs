#region Using Statements

using System;
using System.Windows.Forms;

#endregion

namespace XNA8DVisualizadorImagens
{

#if WINDOWS || LINUX
	/// <summary>
	///     The main class.
	/// </summary>
	public static class Program
	{

		/// <summary>
		///     The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			if(args.Length == 0)
			{
				MessageBox.Show("Entre com algum arquivo.");
				return;
			}
			using(Game1 game = new Game1(args))
			{
				game.Run();
			}
		}

	}
#endif
}
