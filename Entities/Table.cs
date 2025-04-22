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


		//Fold etmemiş veya oyundan çıkmamış oyuncular
		public List<Player> PlayersInGame { get; set; }=new List<Player>();
		//şu andaki oyuncuların vermesi gereken en düşük bahis
		public float CurrentBet { get; set; }

		public Pot MainPot { get; set; }
		public List<Pot> SidePots { get; set; }= new List<Pot>();


		//oyun başladı mı?
		public bool isGameStarted { get; set; }

		//Şu anda sırası olan oyuncu
		public Player currentPlayer { get; set; }



		public List<Card> Deck;
		public List<Card> CommunityCards {  get; set; }=new List<Card>();

		public Table()
		{
			ResetDeck();
		}


		#region oyun mekanikleriyle ilgili metodlar

		public Player NextPlayer() // sıradaki oyuncuyu bulacak
		{
			if (PlayersInGame == null || PlayersInGame.Count == 0)
				return null;


			int currentIndex = PlayersInGame.IndexOf(currentPlayer);
			int total = Players.Length;

			if (currentIndex == -1)
				return PlayersInGame[0]; // currentPlayer oyunda değilse, ilk oyuncudan başla

			int nextIndex = (currentIndex + 1) % PlayersInGame.Count;
			return PlayersInGame[nextIndex];
		}

		public void AdvanceTurn()
		{
			// işte asıl mesela burayı yazmak
		}

		#endregion








		#region Masa içerisinde kartlarla ilgili olan metodlar.

		public void DealCommunityCard()
		{
			// Community cardları dağıtma işlemi burada yapılır (Flop, Turn, River)
			if (CommunityCards.Count == 0) // Flop
			{
				for (int i = 0; i < 3; i++)
				{
					CommunityCards.Add(DrawCard());
				}
			}
			else if (CommunityCards.Count == 3) // Turn
			{
				CommunityCards.Add(DrawCard());
			}
			else if (CommunityCards.Count == 4) // River
			{
				CommunityCards.Add(DrawCard());
			}
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
				if (p != null)
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
					player.Cards.Add(DrawCard());
				}

			}
		}
		#endregion

		public Card DrawCard(bool Remove=true)
		{
			var card = Deck[0];
			if(Remove) Deck.Remove(card);
			return card;
		}


		#region Kullanıcı kayıt-çıkarma işlemleri
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
				if (currentPlayer == player)
					currentPlayer = NextPlayer();
				PlayersInGame.Remove(player);

				Players[index] = null; // Oyuncuyu null ile işaretle
			}
		}
		#endregion

	}
}
