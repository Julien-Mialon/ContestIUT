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
	        for (int i = 0; i < 3 && i < gameInfo.InitialPositions.Count; ++i)
	        {
		        Position cell = gameInfo.InitialPositions[i];

				Logger.Info($"Cell id {i} : ({cell.X}, {cell.Y})");
	        }

            Logger.Info("Waiting for init game data");
            playerId = _client.ReadInitGameData();

			Logger.Info("Get playerId : " + playerId);
            
			ProcessInitData();

			bool first = true;
			while (true)
			{
				TurnInfo turn = _client.ReadTurnGameData(first);

				Output(turn);

				if(first)
				{
				first = false;
					turn = _client.ReadTurnGameData(false);
				}

				Output(turn);

				_client.SendTurnInstruction(turn, Turn(turn).ToList(), false);
			}
		}

		private void Output(TurnInfo turn)
		{
			Logger.Info("Turn : " + turn.TurnId);
			Logger.Info($"My cells : ({turn.CellCount})");

			foreach (var i in turn.Cells)
			{
				Logger.Info($"(id = {i.Id}, mass={i.Mass}, position={i.Position.X};{i.Position.Y}");
			}
		}

		protected virtual void ProcessInitData()
		{

        }

        protected virtual IEnumerable<Action> Turn(TurnInfo turn)
        {
            return RandomTurn(turn);
        }

        private IEnumerable<Action> FarmIA(TurnInfo turn)
        {
            foreach (var myCell in turn.PlayerCells.Where(x=>x.PlayerId==playerId))
            {
                var toReach = turn.Cells[0];
                var min = compare(myCell, toReach);
                foreach (var neutralCell in turn.Cells)
                {
                    var tmp = compare(myCell, neutralCell);
                    if (tmp < min)
                    {
                        min = tmp;
                        toReach = neutralCell;
                    }
                }
                yield return new MoveAction()
                {
                    CellId = myCell.Id,
                    Position = toReach.Position
                };
            }
        }

	    private float compare(Cell a, Cell b)
	    {
            //TODO faire comparaison
	        var posa = a.Position;
	        var posb = b.Position;
	        return Math.Sqrt((posb.Y-posa.Y)*(posb.Y-posa.Y)
                +(posb.Y-posa.Y)*(posb.Y-posa.Y));
        
        }   
        
        #region Random IA
	    private IEnumerable<Action> RandomTurn(TurnInfo turn)
        {
            foreach (var myCell in turn.PlayerCells.Where(x => x.PlayerId == playerId))
            {
                var rand = new Random();
                Action act = null;
                switch (rand.Next(3))
                {
                    case 0:
                        act = MoveAction(myCell);
                        break;
                    case 1:
                        var mass = (float)(rand.NextDouble() * myCell.Mass);
                        act = myCell.Mass > 2 * gameInfo.MinimumCellMass ? DivideAction(myCell, mass) : MoveAction(myCell);
                        break;
                    case 2:
                        act = turn.VirusCount < VirusMaxCount ? CreateVirus(myCell) : MoveAction(myCell);
                        break;
                }
                yield return act;
            }
	        
	    }

        private Action CreateVirus(Cell cell)
        {
            var pos = GenererPosition();
            return new CreateVirusAction
            {
                CellId = cell.Id,
                Position =
                {
                    X = pos.Item1,
                    Y = pos.Item2
                }
            };
        }
			
        private Action MoveAction(Cell cell)
        {
            var pos = GenererPosition();
            return new MoveAction
            {
                CellId = cell.Id,
                Position =
                {
                    X = pos.Item1,
                    Y = pos.Item2
		}
            };
        }

        private Action DivideAction(Cell cell, float mass)
		{
            var pos = GenererPosition();
            return new DivideAction()
            {
                CellId = cell.Id,
                Mass = mass,
                Position =
                {
                    X = pos.Item1,
                    Y = pos.Item2
                }
            };
        }
			
        private Tuple<float, float> GenererPosition()
        {
            var rand = new Random();
            float x = (float)(rand.NextDouble() * gameInfo.Width);
            float y = (float)(rand.NextDouble() * gameInfo.Height);
            return new Tuple<float, float>(x, y);
		}
        #endregion
	}
}
