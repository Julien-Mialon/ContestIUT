using System.Collections.Generic;

namespace Contest.Model
{
    /// <summary>
    /// Cellule neutre
    /// </summary>
	public class Cell
	{
		public uint Id { get; set; }

		public float Mass { get; set; }

		public Position Position { get; set; }
	}

    /// <summary>
    /// Cellule des joueurs (amis & ennemis)
    /// </summary>
	public class PlayerCell : Cell
	{
		public uint PlayerId { get; set; }

		public uint IsolatedTurnsRemaining { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myCell">Cellule qui veut manger</param>
        /// <param name="ennemyCell">Cellule à changer</param>
        /// <param name="ratio">Ratio min pour l'absorption</param>
        /// <returns></returns>
        public bool IsMangeable(PlayerCell myCell, PlayerCell ennemyCell,float ratio)
        {
            //todo à changer si ratio à l'envers
            return ennemyCell.Mass/myCell.Mass > ratio;
        }
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

		public List<Cell> Cells { get; set; } = new List<Cell>();

		public uint VirusCount { get; set; }

		public List<Virus> Virus { get; } = new List<Virus>(); 

		public uint PlayerCellCount { get; set; }

		public List<PlayerCell> PlayerCells {get;} = new List<PlayerCell>();

		public uint PlayerCount { get; set; }

		public List<Player> Players { get; } = new List<Player>(); 

	    public void Read(SocketClient client, bool first)
	    {
		    if (!first)
		    {
			    TurnId = client.ReadInt();
		    }

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

		    PlayerCount = client.ReadInt();
		    for (int i = 0; i < PlayerCount; ++i)
		    {
			    Players.Add(new Player()
			    {
				    PlayerId = client.ReadInt(),
					PlayerName = client.ReadString(),
					CellCount = client.ReadInt(),
					Mass = client.ReadFloat(),
					Score = client.ReadInt64()
			    });
		    }
	    }
    }
}
