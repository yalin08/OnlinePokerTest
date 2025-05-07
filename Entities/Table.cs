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
		public List<Player> PlayersInGame { get; set; } = new List<Player>();
		//şu andaki oyuncuların vermesi gereken en düşük bahis
		public float CurrentBet { get; set; }

		public Pot MainPot { get; set; } = new Pot(); 

		public List<Pot> SidePots { get; set; } = new List<Pot>();


		//oyun başladı mı?
		public bool isGameStarted { get; set; }

		//Şu anda sırası olan oyuncu
		public Player currentPlayer { get; set; }

		public Player SmallBlind { get; set; }
		public Player BigBlind { get; set; }


		public List<Card> Deck;
		public List<Card> CommunityCards { get; set; } = new List<Card>();

		public Player firstToAct
		{
			get
			{
				return (bettingRound == 0) ? NextPlayer(BigBlind) : NextPlayer(SmallBlind);
			}

		}

		public Table()
		{

			ResetDeck();
		}


		#region oyun mekanikleriyle ilgili metodlar

		public Player NextPlayer(Player player = null) // sıradaki oyuncuyu bulacak
		{
			if (PlayersInGame == null || PlayersInGame.Count == 0)
				return null;

			if (player == null)
				player = currentPlayer;


			int currentIndex = PlayersInGame.IndexOf(player);
			int total = Players.Length;

			if (currentIndex == -1)
				return PlayersInGame[0]; // currentPlayer oyunda değilse, ilk oyuncudan başla

			int nextIndex = (currentIndex + 1) % PlayersInGame.Count;
			return PlayersInGame[nextIndex];
		}

		int bettingRound = 0;
		public void AdvanceTurn()
		{


			// Tek kişi kaldıysa oyun biter
			if (PlayersInGame.Count == 1)
			{
				DetermineWinner();
				return;
			}

			currentPlayer = NextPlayer();

			bool allBetsEqual = PlayersInGame
				.Where(p => !p.IsFolded && !p.IsAllIn)
				.All(p => p.CurrentBet == CurrentBet);

			if (allBetsEqual && currentPlayer == BigBlind)
			{
				if (bettingRound == 3)
					DetermineWinner();
				else
					NextTurn();



			}

		}

		private void NextTurn()
		{
		
				DealCommunityCard();
			currentPlayer = firstToAct;
			bettingRound++;

		}

		private void DetermineWinner()
		{
			DistributePots();
			StartNewGame();
		}

		public void DistributePots()
		{
			// Ana pot kazananı
			var winner = HandEvaluator.DetermineWinningPlayer(PlayersInGame);
			winner.Balance += MainPot.Amount;
			MainPot.Amount = 0;

			// Side potları kazananlara dağıt
			foreach (var sidePot in SidePots)
			{
				var sidePotWinner = HandEvaluator.DetermineWinningPlayer(sidePot.Players);
				sidePotWinner.Balance += sidePot.Amount;

			}

			SidePots.Clear();
		}




		private void StartNewGame()
		{
			CurrentBet = 0;
			bettingRound = 0;


			ResetDeck();
			DealCards();


			PlayersInGame.Clear();
			PlayersInGame = Players.Where(p => p != null).ToList();


			foreach (var player in Players)
			{
				if (player != null)
					player.CurrentBet = 0;
			}

			if (SmallBlind == null)
			{
				SmallBlind = Players.FirstOrDefault(x => x != null);
			}
			else
			{
				SmallBlind = NextPlayer(SmallBlind);
			}
			BigBlind = NextPlayer(SmallBlind);


			currentPlayer = firstToAct;



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
		private void ShuffleDeck()
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
			CommunityCards.Clear();

			ShuffleDeck();

		}

		//Oyunculara Kartları dağıtıyor.
		public void DealCards()
		{
			foreach (Player player in Players)
			{
				if (player == null)
					continue;

				player.Cards.Clear();

				for (int i = 0; i < 2; i++)
				{
					player.Cards.Add(DrawCard());
				}

			}
		}
		#endregion

		public Card DrawCard(bool Remove = true)
		{
			var card = Deck[0];
			if (Remove) Deck.Remove(card);
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
