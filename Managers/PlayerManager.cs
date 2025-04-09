using Entities;

namespace Managers
{
	public class PlayerManager
	{
		private static PlayerManager _instance;
		public static PlayerManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new PlayerManager();
				return _instance;
			}
		}

		public Player CreatePlayer(string name, string connectionId)
		{
			Player player = new Player(name, connectionId);
			player.Table = TableManager.Instance.GetAvailableTable();

			for (int i = 0; i < player.Table.Players.Length; i++)
			{
				if (player.Table.Players[i] == null)
				{
					player.Table.Players[i] = player;
					break;
				}
			}

			return player;
		}

		public Player GetPlayer(string connectionId)
		{

			var player = TableManager.Instance.Tables
		.SelectMany(t => t.Players)                  // Tüm masalardaki oyuncuları  liste haline getirir
		.FirstOrDefault(p => p != null && p.ConnectionId == connectionId);
			return player;
		}



		public async Task LogOut(Player player)
		{

			var table = TableManager.Instance.Tables.FirstOrDefault(t => t.Players.Contains(player));

			Console.WriteLine($"{table.Id} odasındaki {player.Name} çıkış yapmak üzere");

			if (table != null)
			{

				for (int i = 0; i < table.Players.Length; i++)
				{
					if (table.Players[i] == player)
					{
						Console.WriteLine($"{table.Id} odasından {table.Players[i].Name} silindi");

						table.Players[i] = null;
						break;
					}
				}


				if (table.Players.All(p => p == null))
				{

					TableManager.Instance.CloseTable(table.Id);

				}
			}
		}



	}
}
