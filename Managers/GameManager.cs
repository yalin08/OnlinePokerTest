using Entities;
using Entities.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
	public class GameManager
	{
		public static GameManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new GameManager();
				return _instance;
			}
		}
		private static GameManager _instance;



		public void ResetDeck(Table table)
		{
			table.ResetDeck();
		}
		public void DealCards(Table table)
		{
			table.DealCards();
		}

		public List<Card> GetPlayerCards(Player player)
		{
			return player.Cards;
		}

		public List<Card> GetDeck(Table table)
		{
			return table.Deck;
		}



	}
}
