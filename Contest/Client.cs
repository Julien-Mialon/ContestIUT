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
			_client.WriteString("Tower Dive");
			//TODO : check server result to check if login has been accepted
		}

		public void ReadInitGameData()
		{
			//TODO : implement
		}

		public void ReadTurnGameData()
		{
			//TODO
		}

		public void SendTurnInstruction()
		{
			//TODO
		}
	}
}