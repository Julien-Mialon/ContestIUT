using System;
using System.Collections.Generic;
using System.Linq;
using Contest.Model;
using Action = Contest.Model.Action;

namespace Contest
{
	public class Game
	{
		private const uint VirusMaxCount = 64;
		private readonly Client _client;
		private uint playerId;
		private GameInfo gameInfo;
		private Random random = new Random(DateTime.Now.Millisecond);

		public Game(Client client)
		{
			_client = client;
		}

		public void Run()
		{
			Logger.Info("Waiting server welcome data");
			gameInfo = _client.ReadServerWelcomeData();

			Logger.Info($"GameInfo : dimensions = {gameInfo.Width} ; {gameInfo.Height}");
			Logger.Info($"GameInfo : turns = {gameInfo.TurnsCount}");
			Logger.Info($"GameInfo : player = {gameInfo.MinPlayers} ; {gameInfo.MaxPlayers}");
			Logger.Info($"GameInfo : mass absorption = {gameInfo.MassAbsorption}");
			Logger.Info($"GameInfo : mass ratio absorption = {gameInfo.MassRatioToAbsorb}");
			Logger.Info($"GameInfo : cells mass = {gameInfo.MinimumCellMass} ; {gameInfo.MaximumCellMass}");
			Logger.Info($"GameInfo : radius = {gameInfo.RadiusFactor}");
			Logger.Info($"GameInfo : cellcount = {gameInfo.MaxCellsCountByPlayer}");
			Logger.Info($"GameInfo : loss per frame  = {gameInfo.MassLossPerFrame}");
			Logger.Info($"GameInfo : cell speed = {gameInfo.CellSpeed}");
			Logger.Info($"GameInfo : speed loss = {gameInfo.SpeedLossFactor}");
			Logger.Info($"GameInfo : viruses mass = {gameInfo.VirusMass}");
			Logger.Info($"GameInfo : virus create mass = {gameInfo.VirusCreationMass}");
			Logger.Info($"GameInfo : virus split = {gameInfo.VirusMaxSplit}");
			Logger.Info($"GameInfo : initial cells = {gameInfo.StartingCellPerPlayer} mass {gameInfo.CellStartingMass}");
			Logger.Info($"GameInfo : initial neutral cell mass = {gameInfo.InitialNeutralCellMass}");
			Logger.Info($"GameInfo : initial repop time = {gameInfo.InitialNeutralCellRepopTime}");
			Logger.Info($"GameInfo : initial neutral count = {gameInfo.InitialCellCount}");
			for (int i = 0; i < gameInfo.InitialPositions.Count; ++i)
			{
				Position cell = gameInfo.InitialPositions[i];

				Logger.Info($"Cell id {i} : ({cell.X}, {cell.Y})");
			}

			Logger.Info("Waiting for init game data");
			playerId = _client.ReadInitGameData();

			Logger.Info("Get playerId : " + playerId);

			ProcessInitData();

			bool first = true;
			int dieCount = 0;
			while (true)
			{
				TurnInfo turn = _client.ReadTurnGameData(first);

				Output(turn);

				if (first)
				{
					first = false;
					turn = _client.ReadTurnGameData(false);
				}

				Output(turn);

				float mass = turn.PlayerCells.Where(x => x.PlayerId == playerId).Sum(x => x.Mass);
                if (dieCount < 40 || mass < gameInfo.CellStartingMass * 0.5)
                {
	                dieCount++;
					Logger.Error("Sepuku : " + mass);
					_client.SendTurnInstruction(turn, new List<Action>(), true);
				}
				else
				{
					_client.SendTurnInstruction(turn, Turn(turn).ToList(), false);
				}
			}
		}

		private void Output(TurnInfo turn)
		{
			Logger.Error("Turn : " + turn.TurnId);
			Logger.Info("Initial cells " + turn.InitialCellCount);
			/*
			foreach (var i in turn.InitialCellRemainingTurn)
			{
				Logger.Info($"(remaining = {i})");
			}
			// */
			Logger.Info($"My cells : ({turn.CellCount})");
			foreach (var i in turn.Cells)
			{
				Logger.Info($"(id = {i.Id}, mass={i.Mass}, position={i.Position.X};{i.Position.Y}");
			}

			Logger.Info($"Viruses : " + turn.VirusCount);
			foreach (var i in turn.Virus)
			{
				Logger.Info($"(id={i.Id}, ({i.Position.X} ; {i.Position.Y})");
			}

			Logger.Error($"player : " + turn.PlayerCellCount);
			foreach (var i in turn.PlayerCells)
			{
				Logger.Error($"playerId = {i.PlayerId}, id={i.Id}, pos={i.Position.X},{i.Position.Y}, mass={i.Mass}, isolated={i.IsolatedTurnsRemaining}");
			}

			Logger.Info($"players : " + turn.PlayerCount);
			foreach (var i in turn.Players)
			{
				Logger.Info($"id({i.PlayerId}) name({i.PlayerName}) cells({i.CellCount}) mass({i.Mass}) score({i.Score})");
			}

		}

		protected virtual void ProcessInitData()
		{

		}

		protected virtual IEnumerable<Action> Turn(TurnInfo turn)
		{
			return FarmIA(turn);
		}


		//farm
		private IEnumerable<Action> FarmIA(TurnInfo turn)
		{
			turn.Cells.AddRange(gameInfo.InitialPositions.Where((x, index) => (turn.InitialCellRemainingTurn[index] == 0)).Select((x, index) => new Cell()
			{
				Position = x,
				Id = (uint)index,
				Mass = gameInfo.InitialNeutralCellMass
			}));

			turn.Cells = turn.Cells.OrderByDescending(x => x.Mass).ToList();

			List<bool> cellTarget = new List<bool>();
			turn.Cells.ForEach(x => cellTarget.Add(true));

			List<PlayerCell> myCells = turn.PlayerCells.Where(x => x.PlayerId == playerId).ToList();
			int cellCount = myCells.Count;
			foreach (var myCell in myCells)
			{
				if (cellCount + 1 < gameInfo.MaxCellsCountByPlayer && myCell.Mass > 0.9 * gameInfo.MaximumCellMass)
				{
					Logger.Error("Splitting");
					yield return FarmDivideAction(turn, myCell, cellTarget);
					cellCount++;
				}
				else
				{
					var action = FarmMoveAction(turn, myCell, cellTarget);

					Logger.Error($"Moving to {action.Position.X}, {action.Position.Y} from {myCell.Position.X}, {myCell.Position.Y}");

					yield return action;
				}
			}
		}

		private Action FarmDivideAction(TurnInfo turn, PlayerCell myCell, List<bool> cellTarget)
		{
			return new DivideAction()
			{
				CellId = myCell.Id,
				Position = NextCelltoReach(turn, myCell, cellTarget).Position,
				Mass = myCell.Mass / 2
			};
		}

		private Cell NextCelltoReach(TurnInfo turn, Cell myCurrentCell, List<bool> cellTarget)
		{
			Cell toReach = turn.Cells[0];
			float availableDistance = (gameInfo.CellSpeed - myCurrentCell.Mass*gameInfo.SpeedLossFactor);

			float distance = Compare(myCurrentCell, toReach);
			if (distance/availableDistance < 5)
			{
				return toReach;
			}

			foreach (var neutralCell in turn.Cells.Where((x, index) => cellTarget[index]))
			{
				//var tmp = Compare(myCurrentCell, neutralCell);

				distance = Compare(myCurrentCell, toReach);
				if (distance / availableDistance < 5)
				{
					return toReach;
				}
			}
			cellTarget[turn.Cells.IndexOf(toReach)] = false;
			return toReach;
		}

		private MoveAction FarmMoveAction(TurnInfo turn, Cell myCurrentCell, List<bool> cellTarget)
		{
			return new MoveAction()
			{
				CellId = myCurrentCell.Id,
				Position = NextCelltoReach(turn, myCurrentCell, cellTarget).Position
			};

		}
		private float Compare(Cell a, Cell b)
		{
			var posa = a.Position;
			var posb = b.Position;
			return (float)Math.Sqrt((posb.Y - posa.Y) * (posb.Y - posa.Y)
				+ (posb.X - posa.X) * (posb.X - posa.X));

		}


		private bool IsInCorner(Cell me)
		{
			return IsInCorner(me.Position.X, me.Position.Y);
		}
		private bool IsInCorner(float x, float y)
		{
			if (x < 0.15 * gameInfo.Width || x > 0.85 * gameInfo.Width || y < 0.15 * gameInfo.Height || y > 0.85 * gameInfo.Height)
				return true;
			return false;
		}


		//random
		private IEnumerable<Action> RandomTurn(TurnInfo turn)
		{
			foreach (var myCell in turn.PlayerCells.Where(x => x.PlayerId == playerId))
			{
				Action act = null;
				switch (random.Next(3))
				{
					case 1:
						var mass = (float)(random.NextDouble() * myCell.Mass);
						act = myCell.Mass > 2 * gameInfo.MinimumCellMass ? DivideAction(myCell, mass) : MoveAction(myCell);
						break;
					case 2:
						act = turn.VirusCount < VirusMaxCount ? CreateVirus(myCell) : MoveAction(myCell);
						break;
					default:
						act = MoveAction(myCell);
						break;
				}
				if (act != null)
				{
					yield return act;
				}
			}

		}

		private Action CreateVirus(Cell cell)
		{
			return new CreateVirusAction
			{
				CellId = cell.Id,
				Position = GenererPosition()
			};
		}

		private Action MoveAction(Cell cell)
		{
			return new MoveAction
			{
				CellId = cell.Id,
				Position = GenererPosition()
			};
		}

		private Action DivideAction(Cell cell, float mass)
		{
			return new DivideAction
			{
				CellId = cell.Id,
				Mass = mass,
				Position = GenererPosition()
			};
		}

		private Position GenererPosition()
		{
			float x = (float)(random.NextDouble() * gameInfo.Width);
			float y = (float)(random.NextDouble() * gameInfo.Height);
			return new Position()
			{
				X = x,
				Y = y,
			};
		}
	}
}

