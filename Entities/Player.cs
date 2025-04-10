﻿using Entities.Game;

namespace Entities
{
	public class Player
	{
		public string Name { get; }



		public List<Card> Cards = new List<Card>(); // Oyuncunun elindeki kartlar





		public string ConnectionId { get;  }
		public DateTime LastInteractionTime { get; set; }=DateTime.Now;
		public Table Table { get; set; }


		public Player(string name, string connectionId)
		{
			Name = name;
			ConnectionId = connectionId;
		}

		// Son etkileşim zamanını güncelleyen metot
		public void UpdateInteractionTime()
		{
			LastInteractionTime = DateTime.Now;
		}
	}
}

