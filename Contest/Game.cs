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
			_client.ReadInitGameData();

			ProcessInitData();

			while (true)
			{
				_client.ReadTurnGameData();

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