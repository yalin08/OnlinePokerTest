using Entities.Game;
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
		public Player[] Players { get; set; } = new Player[4];

		public List<Card> Deck;

        public Table()
        {
			ResetDeck();
        }

		//kartları karıştırmak için
		public void ShuffleDeck()
		{
			Random rng = new Random();
			int n = Deck.Count;
			for (int i = n - 1; i > 0; i--)
			{
				int j = rng.Next(i + 1); // 0 ile i arasında rastgele bir indeks seç
				(Deck[i], Deck[j]) = (Deck[j], Deck[i]); // Swap işlemi (yer değiştir)
			}
		}

		//Oyundaki kartları tekrar karıştırıyor,yeni oyuna başlarken kullanılması için
		public void ResetDeck()
		{
			var newDeck = new List<Card>();
			for (int i = 0; i < 4; i++)
			{
				for (int j = 1; j < 14; j++)
				{
					newDeck.Add(new Card() { Number = (Number)j, Suit = (Suit)i });
				}
			}

			Deck = newDeck;

			foreach (Player p in Players)
			{
				if(p!=null)
				p.Cards.Clear();
			}

			ShuffleDeck();

		}

		//Oyunculara Kartları dağıtıyor.
		public void DealCards()
		{
			foreach (Player player in Players)
			{
				if (player == null)
					return;

				player.Cards.Clear();

				for (int i = 0; i < 2; i++)
				{
					var card = Deck[0];
					player.Cards.Add(card);
					Deck.Remove(card);
				}

			}
		}



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
