using System.Collections.Generic;

namespace Contest.Model
{
	public class Cell
	{
		public uint Id { get; set; }

		public float Mass { get; set; }

		public Position Position { get; set; }
	}

	public class PlayerCell : Cell
	{
		public uint PlayerId { get; set; }

		public uint IsolatedTurnsRemaining { get; set; }
	}

	public class Virus
	{
		public uint Id { get; set; }

		public Position Position { get; set; }
	}

	public class Player
	{
		public uint PlayerId { get; set; }

		public string PlayerName { get; set; }

		public uint CellCount { get; set; }

		public float Mass { get; set; }

		public ulong Score { get; set; }
	}

    public class TurnInfo
    {
		public uint TurnId { get; set; }

        public uint InitialCellCount { get; set; }

		public List<uint> InitialCellRemainingTurn { get; } = new List<uint>();

		public uint CellCount { get; set; }

		public List<Cell> Cells { get; } = new List<Cell>();

		public uint VirusCount { get; set; }

		public List<Virus> Virus { get; } = new List<Virus>(); 

		public uint PlayerCellCount { get; set; }

		public List<PlayerCell> PlayerCells {get;} = new List<PlayerCell>();

	    public void Read(SocketClient client)
	    {
		    TurnId = client.ReadInt();

		    InitialCellCount = client.ReadInt();
		    for (int i = 0; i < InitialCellCount; ++i)
		    {
			    InitialCellRemainingTurn.Add(client.ReadInt());
		    }

		    CellCount = client.ReadInt();
		    for (int i = 0; i < CellCount; ++i)
		    {
			    Cells.Add(new Cell()
			    {
				    Id =client.ReadInt(),
					Mass = client.ReadFloat(),
					Position = new Position()
					{
						X = client.ReadFloat(),
						Y = client.ReadFloat()
					}
			    });
		    }

		    VirusCount = client.ReadInt();
		    for (int i = 0; i < VirusCount; ++i)
		    {
			    Virus.Add(new Virus()
			    {
				    Id = client.ReadInt(),
					Position = new Position()
					{
						X = client.ReadFloat(),
						Y = client.ReadFloat()
					}
			    });
		    }

		    PlayerCellCount = client.ReadInt();
		    for (int i = 0; i < PlayerCellCount; ++i)
		    {
			    PlayerCells.Add(new PlayerCell()
			    {
				    Id = client.ReadInt(),
					Position = new Position()
					{
						X = client.ReadFloat(),
						Y = client.ReadFloat()
					},
					PlayerId = client.ReadInt(),
					Mass = client.ReadFloat(),
					IsolatedTurnsRemaining = client.ReadInt()
			    });
		    }
	    }
    }
}
