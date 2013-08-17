using System;
using System.Reflection;
using System.Threading;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;

namespace DragonSpire
{
	public class Server
	{
		internal static RSACryptoServiceProvider CryptoServiceProvider { get; set; }
		internal static RSAParameters ServerKey { get; set; }
		internal static World MainWorld;
		internal static Random random = new Random();

		internal static List<World> worlds = new List<World>();

		internal static long TimeTicks = 0;

		internal static bool shouldShutdown = false;

		internal Server() //This method HAS to return to start the GUI, do not put any loops on this thread!
		{
			Console.Title = "DragonSpire Version " + Assembly.GetEntryAssembly().GetName().Version;
			CryptoServiceProvider = new RSACryptoServiceProvider(1024);
			ServerKey = CryptoServiceProvider.ExportParameters(true);

			MaterialManager.LoadMaterials();
			CommandManager.LoadCommands();

			MainWorld = new World("Main", WorldGeneratorType.Flat);

			TickTimer.Initialize(); //The main tick loop!
			new Thread(ClientConnectionHandler.Initialize).Start(); //The Client Connection handler!
			//new Thread(PlayerPOSLoop).Start();

			Log("Server Startup Executed Succesfully.", LogTypesEnum.System);
		}

		//void PlayerPOSLoop()
		//{
		//    while (!Server.shouldShutdown)
		//    {
		//        foreach (Player p in Player.players.Values.ToArray())
		//        {
		//            if (!p.client.loggedin) continue;
		//            foreach (Player p2 in Player.players.Values.ToArray())
		//            {
		//                if (p2 == null) continue;
		//                if (!p2.client.loggedin || !p.HasSpawned.Contains(p2)) continue;
		//                if (p2 != p)
		//                {
		//                    p.client.SendEntityTeleport(p2);
		//                    p.client.SendEntityHeadLook(p2);
		//                }
		//            }
		//            p.client.SendPlayerListItem();
		//        }
		//        Thread.Sleep(10);
		//    }
		//}

		public static void Log(string s, LogTypesEnum LogLevel)
		{
			ConsoleLogger.Log(s, LogLevel);
		}
	}
}
