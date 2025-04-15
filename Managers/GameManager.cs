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

		public void StartGame(Table table)
		{
			ResetDeck(table);
			DealCards(table);
			//	InitializeBetting(table);
			//StartBettingRound(table);
		}


		void ResetDeck(Table table)
		{
			table.ResetDeck();
		}
		void DealCards(Table table)
		{
			table.DealCards();
		}





	}
}
