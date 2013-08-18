using System;
using System.Windows.Forms;

namespace DragonSpire
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			ConsoleLogger.InitLogTypes(); //Initialize the LOG Types (this needs to be set before we do ANYTHING else as Server.Log relies on this!

			TestMethod();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			new Server(); //Start a new Server instance

			while (!Server.shouldShutdown)
			{
				Console.ReadLine();
			}
			//Application.Run(new Form1()); //Start the GUI ones everything else is done
		}
		static void TestMethod()
		{
			
		}
	}
}
