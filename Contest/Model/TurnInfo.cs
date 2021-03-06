﻿using System;
using System.Collections.Generic;
using System.Linq;
using Contest;
using Contest.Model;

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

		public float CurrentSpeed => Game.gameInfo.CellSpeed - Mass * Game.gameInfo.SpeedLossFactor;

		public bool IsMangeable(PlayerCell ennemyCell, float ratio)
		{
			//todo à changer si ratio à l'envers
			return Mass / ennemyCell.Mass > (ratio * 1.1);
		}
        
		public bool Collision(Position cible, List<PlayerCell> othercells)
		{
			var test = false;

			foreach (var ennemy in othercells)
			{
                test |= Collision(cible, ennemy);
				if (test) return true;
			}
			return false;
		}

		public bool CollisionEatable(Position cible, List<PlayerCell> othercells)
		{
			var test = false;

			foreach (var ennemy in othercells.Where(x => !IsMangeable(x, Game.gameInfo.MassRatioToAbsorb)))
			{
				test |= VoiceCollision(cible, ennemy) || VoiceCollision(new Position()
				{
					X = Math.Abs(cible.X + ennemy.Position.X) / 2,
                    Y = Math.Abs(cible.Y + ennemy.Position.Y) / 2
				}, ennemy);
				if (test) return true;
			}
			return false;
		}

        public bool Collision(Position cible, PlayerCell enemy)
		{
            if (Math.Sqrt((cible.X-enemy.Position.X)*(cible.X-enemy.Position.X)
                + (cible.Y-enemy.Position.Y)*(cible.Y-enemy.Position.Y))
                >(this.Mass+enemy.CurrentSpeed))

				return true;
			return false;
		}

		public bool VoiceCollision(Position cible, PlayerCell enemy)
		{
			if (Math.Sqrt((cible.X - enemy.Position.X) * (cible.X - enemy.Position.X)
				+ (cible.Y - enemy.Position.Y) * (cible.Y - enemy.Position.Y))
				> (Game.gameInfo.CellSpeed - Math.Max(Mass, enemy.Mass) * 
				Game.gameInfo.SpeedLossFactor) *2)
				

				return true;
			return false;
		}

	    public bool YoloColl(Position c, PlayerCell e)
	    {
	        if (Math.Max(Mass, e.Mass) > Math.Abs(
                    (((c.Y-this.Position.Y)*(e.Position.X-Position.X))
                    /(c.X-Position.X))-e.Position.X-Position.X
	            ))
	            return true;
	        return false;
	    }
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

	public List<PlayerCell> PlayerCells { get; } = new List<PlayerCell>();

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
				Id = client.ReadInt(),
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

