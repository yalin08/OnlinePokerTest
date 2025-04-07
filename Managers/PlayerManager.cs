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
			return new Player(name, connectionId);
		}

	

		public void LogOut(Player player)
		{
			
			var table = TableManager.Instance.Tables.FirstOrDefault(t => t.Players.Contains(player));

			if (table != null)
			{
				
				for (int i = 0; i < table.Players.Length; i++)
				{
					if (table.Players[i] == player)
					{
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
