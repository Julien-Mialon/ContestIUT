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
            gameInfo = _client.ReadServerWelcomeData();
            playerId = _client.ReadInitGameData();

			ProcessInitData();

			bool first = true;
			while (true)
			{
				TurnInfo turn = _client.ReadTurnGameData(first);
				first = false;

                

				_client.SendTurnInstruction(turn, Turn(turn).ToList(), true);
			}
		}

		protected virtual void ProcessInitData()
		{

        }

        protected virtual IEnumerable<Action> Turn(TurnInfo turn)
        {
            return RandomTurn(turn);
        }

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

	}
}