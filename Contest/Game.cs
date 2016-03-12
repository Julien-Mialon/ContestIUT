using Contest.Model;

namespace Contest
{
	public class Game
	{
		private readonly Client _client;

		public Game(Client client)
		{
			_client = client;
		}

		public void Run()
		{
			GameInfo gameInfo = _client.ReadServerWelcomeData();
			uint playerId = _client.ReadInitGameData();

			ProcessInitData();

			bool first = true;
			while (true)
			{
				TurnInfo turn = _client.ReadTurnGameData(first);
				first = false;

				Turn();

				_client.SendTurnInstruction();
			}
		}

		protected virtual void ProcessInitData()
		{
			
		}

		protected virtual void Turn()
		{
			
		}
	}
}