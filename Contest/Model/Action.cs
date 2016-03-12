namespace Contest.Model
{
    public class Action
    {
		public uint CellId { get; set; }
    }

	public class Position
	{
		public float X { get; set; }

		public float Y { get; set; }
	}

	public class MoveAction : Action
	{
		public Position Position { get; set; }
	}

	public class DivideAction : Action
	{
		public Position Position { get; set; }

		public float Mass { get; set; }
	}

	public class CreateVirusAction : Action
	{
		public Position Position { get; set; }
	}
}
