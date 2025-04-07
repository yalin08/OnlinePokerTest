using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
	public class TableManager
	{
		private static TableManager _instance;
		public static TableManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new TableManager();

				}

				return _instance;
			}
		}
		int counter = 0;

		PlayerManager playerManager;


		public List<Table> Tables = new List<Table>();

		public TableManager()
		{
			playerManager = PlayerManager.Instance;
		}


		public Table GetAvailableTable()
		{

			var table = Tables.FirstOrDefault(t => t.Players.Count(p => p != null) < 4);

			if (table == null)
			{
				table = new Table();
				
				table.Id= counter++;
		
				Tables.Add(table);

			}

			return table;

		}

		public List<string> GetPlayersInTable(int tableId)
		{
			var table = Tables.FirstOrDefault(t => t.Id == tableId);
			if (table != null)
			{
				return table.Players.Where(p => p != null).Select(p => p.Name).ToList();
			}
			return new List<string>();
		}


		public void CloseTable(int id)
		{
			var table=Tables.Find(t => t.Id == id);
		
			if (table != null)
			{
				Tables.Remove(table);
			}

		}

	}
}
