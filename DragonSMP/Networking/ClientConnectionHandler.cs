using System;
using System.Net;
using System.Net.Sockets;

namespace DragonSpire
{
	public class ClientConnectionHandler
	{
		private static TcpListener _listener;

		internal static void Initialize()
		{
			Server.Log("Starting ConnectionHandler...", LogTypesEnum.System);
			new ClientConnectionHandler();
		}

		internal ClientConnectionHandler()
		{
			StartListening();
		}

		private static void StartListening()
		{
			while (!Server.shouldShutdown)
			{
				try
				{
					_listener = new TcpListener(IPAddress.Any, Config.port);
					_listener.Start();
					_listener.BeginAcceptTcpClient(AcceptCallback, _listener);
					break;
				}
				catch (SocketException e)
				{
					Console.WriteLine("e1");
					Server.Log(e.Message, LogTypesEnum.Error);
					break;
				}
				catch (Exception e)
				{
					Console.WriteLine("e2");
					Server.Log(e.Message, LogTypesEnum.Error);
				}
			}
		}

		private static void AcceptCallback(IAsyncResult ar)
		{
			var listener2 = (TcpListener)ar.AsyncState;
			try
			{
				TcpClient clientSocket = listener2.EndAcceptTcpClient(ar);
				new Client(clientSocket, Server.MainWorld);
			}
			catch (Exception e)
			{
				Server.Log(e.Message, LogTypesEnum.Error);
				Server.Log(e.StackTrace, LogTypesEnum.Error);
			}

			if (!Server.shouldShutdown)
			{
				_listener.BeginAcceptTcpClient(AcceptCallback, _listener);
			}
		}
	}
}
