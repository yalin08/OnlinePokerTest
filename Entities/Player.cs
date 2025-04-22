using Entities.Game;

namespace Entities
{
	public class Player
	{
		public string Name { get; }




		private List<Card> _cards=new List<Card>();

		public List<Card> Cards
		{
			get { return _cards; }
			set { _cards = value; }
		}

		public float Balance { get; set; }
		public bool IsFolded {  get; set; }
		public bool IsAllIn {  get; set; }



		public string ConnectionId { get;  }
		public DateTime LastInteractionTime { get; set; }=DateTime.Now;
		public Table Table { get; set; }

		public bool isYourTurn 
		{ 
			get 
			{
				bool turn =Table.currentPlayer == this;
				return turn; 
			} 
		}

		public Player(string name, string connectionId)
		{
			Name = name;
			ConnectionId = connectionId;
		}


		// Check yapmak
		public void Check()
		{
			if (!isYourTurn)
				return; // Eğer oyuncunun sırası değilse, işlem yapma

			// Check yaptığı için herhangi bir işlem yapılmıyor, yalnızca sıra yeni oyuncuya geçecek.

			// Aksiyon bitiminde sırayı ilerlet
			Table.AdvanceTurn();
		}

		// Fold yapmak
		public void Fold()
		{
			if (!isYourTurn)
				return; // Eğer oyuncunun sırası değilse, işlem yapma

			IsFolded = true; // Oyuncu katılmıyor
			Table.PlayersInGame.Remove(this);
			UpdateInteractionTime();

			// Aksiyon bitiminde sırayı ilerlet
			Table.AdvanceTurn();
		}

		// Bet yapmak
		public void Bet(float amount)
		{
			if (!isYourTurn)
				return; // Eğer oyuncunun sırası değilse, işlem yapma

			if (amount <= 0 || amount > Balance)
				return; // Geçersiz bahis

			Balance -= amount;
			Table.MainPot.Amount += amount; // Potu artır

			UpdateInteractionTime();

			// Aksiyon bitiminde sırayı ilerlet
			Table.AdvanceTurn();
		}





		// Son etkileşim zamanını güncelleyen metot
		public void UpdateInteractionTime()
		{
			LastInteractionTime = DateTime.Now;
		}
	}
}

