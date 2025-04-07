using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	public class Table
	{
		public int Id { get; set; }
		public Player[] Players { get; set; }= new Player[4];

		public void AddPlayer(Player player)
		{
			for (int i = 0; i < Players.Length; i++)
			{
				if (Players[i] == null)
				{
					Players[i] = player;
					break;
				}
			}
		}

		public void RemovePlayer(Player player)
		{
			// Oyuncuyu bulup, listeden çıkar
			var index = Array.FindIndex(Players, p => p != null && p.ConnectionId == player.ConnectionId);

			if (index >= 0)
			{
				Players[index] = null; // Oyuncuyu null ile işaretle
			}
		}
	}
}
