namespace Entities
{
	public class Player
	{
		public string Name { get; }
		public string ConnectionId { get;  }
		public Table Table { get; set; }


		public Player(string name, string connectionId)
		{
			Name = name;
			ConnectionId = connectionId;
		}
	}
}

