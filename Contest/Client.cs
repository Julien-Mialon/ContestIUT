using System;
using Contest.Model;

namespace Contest
{
	public class Client
	{
		private readonly SocketClient _client;

		public Client(string host, int port)
		{
			_client = new SocketClient(host, port);
		}

		public void Connect()
		{
			_client.ConnectAsync();
			_client.WriteInt8(MessageHeader.LOGIN_PLAYER);
			_client.WriteString("Tower Dive");

			int ack = _client.ReadChar();
			if (ack != MessageHeader.LOGIN_ACK)
			{
				throw new Exception("Didnt receive Login ACK, got " + ack);
			}

			Logger.Info("Login ACK");


			//TODO : check server result to check if login has been accepted
		}

		public GameInfo ReadServerWelcomeData()
		{
			int header = _client.ReadChar();

			if (header == MessageHeader.WELCOME)
			{
				GameInfo game = new GameInfo();
				game.Read(_client);

				return game;
			}
			throw new Exception("Error welcome message");
		}

		public uint ReadInitGameData()
		{
			int header = _client.ReadChar();

			if (header != MessageHeader.GAME_STARTS)
			{
				throw new Exception("Expect game start");
			}

			uint playerId = _client.ReadInt();
			return playerId;
		}

		public void ReadTurnGameData()
		{
			int header = _client.ReadChar();
			if (header != MessageHeader.TURN)
			{
				throw new Exception("Expect turn begin");
			}


		}

		public void SendTurnInstruction()
		{
			//TODO
		}
	}
}