using System;

namespace Contest
{
	public class Program
	{
		/// <summary>
		/// Main entry point, expect some parameters
		///		- ip/hostname
		///		- port
		///		- true if death mode
		/// </summary>
		/// <param name="args"></param>
		public void Main(string[] args)
		{
			Logger.Initialize(Logger.Level.Warning);

			goto debugMode;

			Logger.Info($"Run with parameters {string.Join(" ", args)}");

			if (args.Length < 2)
			{
				Logger.Critical("Usage : program.cs <host> <port> [<death mode>]");
				return;
			}
			goto prodMode;
debugMode:
			args = new string[] {"192.168.13.212", "4242"};
prodMode:
			Client client;
			bool isDeathMode = false;
			try
			{
				string host = args[0];
				int port = int.Parse(args[1]);

				client = new Client(host, port);

				if (args.Length > 2 && !string.IsNullOrEmpty(args[2]))
				{
					isDeathMode = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Critical("Unable to initialize Client with host and port");
				Logger.Critical(ex.ToString());
				return;
			}

			try
			{
				client.Connect();
			}
			catch (Exception ex)
			{
				Logger.Critical("Unable to connect to server " + ex);
				return;
			}

			try
			{
				Game game = isDeathMode ? new DeathGame(client) : new Game(client);
				game.Run();
			}
			catch (Exception ex)
			{
				Logger.Critical("Exception in game " + ex);
			}

			Console.WriteLine("Game finished");
		}
	}
}
