using System.Collections.Generic;

namespace Contest.Model
{
    public class GameInfo
    {
		public double Width { get; set; }

		public double Height { get; set; }

		public uint MinPlayers { get; set; }

		public uint MaxPlayers { get; set; }

		public float MassAbsorption { get; set; }

		public float MassRatioToAbsorb { get; set; }

		public float MinimumCellMass { get; set; }

		public float MaximumCellMass { get; set; }

		public float RadiusFactor { get; set; }

		public uint MaxCellsCountByPlayer { get; set; }

		public float MassLossPerFrame { get; set; }

		public float CellSpeed { get; set; }

	    public float SpeedLossFactor { get; set; }

		public float VirusMass { get; set; }

		public float VirusCreationMass { get; set; }

		public uint VirusMaxSplit { get; set; }

		public uint StartingCellPerPlayer { get; set; }

		public float CellStartingMass { get; set; }

		public float InitialNeutralCellMass { get; set; }

		public uint InitialNeutralCellRepopTime { get; set; }

		public uint TurnsCount { get; set; }

		public uint InitialCellCount { get; set; }

	    public List<Position> InitialPositions { get; } = new List<Position>();

	    public void Read(SocketClient client)
	    {
		    Width = client.ReadFloat();
		    Height = client.ReadFloat();
		    MinPlayers = client.ReadInt();
		    MaxPlayers = client.ReadInt();
		    MassAbsorption = client.ReadFloat();
		    MassRatioToAbsorb = client.ReadFloat();
		    MinimumCellMass = client.ReadFloat();
		    MaximumCellMass = client.ReadFloat();
		    RadiusFactor = client.ReadFloat();
		    MaxCellsCountByPlayer = client.ReadInt();
		    MassLossPerFrame = client.ReadFloat();
		    CellSpeed = client.ReadFloat();
		    SpeedLossFactor = client.ReadFloat();
		    VirusMass = client.ReadFloat();
		    VirusCreationMass = client.ReadFloat();
		    VirusMaxSplit = client.ReadInt();
		    StartingCellPerPlayer = client.ReadInt();
		    CellStartingMass = client.ReadFloat();
		    InitialNeutralCellMass = client.ReadFloat();
		    InitialNeutralCellRepopTime = client.ReadInt();
		    TurnsCount = client.ReadInt();
		    InitialCellCount = client.ReadInt();

		    for (int i = 0; i < InitialCellCount; ++i)
		    {
			    InitialPositions.Add(new Position()
			    {
				    X = client.ReadFloat(),
				    Y = client.ReadFloat(),
			    });
		    }
	    }
    }
}
