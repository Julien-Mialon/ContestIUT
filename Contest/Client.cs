using System;
using System.Collections.Generic;
using System.Linq;
using Contest.Model;
using Action = Contest.Model.Action;

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
				uint size = _client.ReadInt();
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
			uint size = _client.ReadInt();
			uint playerId = _client.ReadInt();
			return playerId;
		}

		public TurnInfo ReadTurnGameData(bool first)
		{
			if (!first)
			{
				int header = _client.ReadChar();
				if (header != MessageHeader.TURN)
				{
					throw new Exception("Expect turn begin = " + header);
				}
			}
			_client.ReadInt();
			TurnInfo turn = new TurnInfo();
			turn.Read(_client);
			return turn;
		}

		public void SendTurnInstruction(TurnInfo turn, List<Action> actions, bool surrender)
		{
			List<MoveAction> moves = actions.OfType<MoveAction>().ToList();
			List<DivideAction> divides = actions.OfType<DivideAction>().ToList();
			List<CreateVirusAction> createVirusAction = actions.OfType<CreateVirusAction>().ToList();

			uint size = 3*4 + 1;
			size += ((uint)moves.Count)*(4*3);
			size += ((uint)divides.Count)*(4*4);
			size += ((uint)createVirusAction.Count)*(4*3);

			_client.WriteInt8(MessageHeader.TURN_ACK);
			_client.WriteInt(size);
			_client.WriteInt(turn.TurnId);

			_client.WriteInt((uint)moves.Count);
			foreach (MoveAction action in moves)
			{
				_client.WriteInt(action.CellId);
				_client.WriteFloat(action.Position.X);
				_client.WriteFloat(action.Position.Y);
			}

			_client.WriteInt((uint)divides.Count);
			foreach (DivideAction action in divides)
			{
				_client.WriteInt(action.CellId);
				_client.WriteFloat(action.Position.X);
				_client.WriteFloat(action.Position.Y);
				_client.WriteFloat(action.Mass);
			}

			_client.WriteInt((uint)createVirusAction.Count);
			foreach (CreateVirusAction action in createVirusAction)
			{
				_client.WriteInt(action.CellId);
				_client.WriteFloat(action.Position.X);
				_client.WriteFloat(action.Position.Y);
			}

			_client.WriteInt8(surrender ? 1 : 0);
		}
	}
}
