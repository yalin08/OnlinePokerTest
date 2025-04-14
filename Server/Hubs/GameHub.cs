using Entities;
using Managers;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

public class GameHub : Hub
{

	#region Chat metodları


	// Bu metod, istemciden gelen mesajları alır ve tüm bağlı istemcilere iletir.
	public async Task SendMessage(string message, string user = null)
	{

		var Player = PlayerManager.Instance.GetPlayer(Context.ConnectionId);

		if (Player == null)
			return;
		else

		{
			Player.UpdateInteractionTime(); // Mesaj gönderildiğinde idleTime'ı günceller
		}

		var table = TableManager.Instance.GetTableById(Player.Table.Id);


		if (table == null)
		{
			await Clients.Client(Player.ConnectionId).SendAsync("ReceiveMessage", null, "Masa bulunamadı.");
			return;
		}

		foreach (var player in table.Players.Where(p => p != null))
		{
			await Clients.Client(player.ConnectionId).SendAsync("ReceiveMessage", user, message);
		}
	}

	public async Task SendMessageToPlayer(string message, string connectionId)
	{
		var Player = PlayerManager.Instance.GetPlayer(Context.ConnectionId);
		var table = TableManager.Instance.GetTableById(Player.Table.Id);


		if (Player == null)
		{
			return;
		}

		if (table == null)
		{
			await Clients.Client(connectionId).SendAsync("ReceiveMessage", null, "Masa bulunamadı.");
			return;
		}

		await Clients.Client(connectionId).SendAsync("ReceiveMessage", null, message);

	}

	#endregion

	//kullanıcının elindeki kartları string olarak getiriyor,test amaçlı
	public async Task ShowPlayersCards()
	{

		foreach (Player p in TableManager.Instance.GetPlayersInTable(PlayerManager.Instance.GetPlayer(Context.ConnectionId).Table.Id))
		{
			string text = $"{p.Name} kartları:\n{string.Join("\n", p.Cards)}";
			Console.WriteLine(text);

			await Clients.Client(p.ConnectionId).SendAsync("GetCards", p.Cards);
		}

		
	}

	public async Task GetTableDeck()
	{
		var player = PlayerManager.Instance.GetPlayer(Context.ConnectionId);
		var table = TableManager.Instance.GetTableById(player.Table.Id);

		await Clients.Client(player.ConnectionId).SendAsync("ReceiveMessage", null, table.Deck);
	}

	//denemek için, ileride değişmeli.
	public async Task DealCards()
	{
		var player = PlayerManager.Instance.GetPlayer(Context.ConnectionId);
		var table = TableManager.Instance.GetTableById(player.Table.Id);

		GameManager.Instance.ResetDeck(table);
		GameManager.Instance.DealCards(table);

		string text = $"{table.Id} Masanın destesi:\n{string.Join("\n", table.Deck)}";
		await ShowPlayersCards();
		Console.WriteLine(text);

	}

	#region Bağlantı metodları

	// Oyuncuyu masadan çıkaran metod
	// Bağlantı sağlandığında bu metod çalışacak.
	public override async Task OnConnectedAsync()
	{
		var playerName = Context.GetHttpContext()?.Request.Query["playerName"]; // URL parametrelerinden oyuncu ismini al
		if (string.IsNullOrWhiteSpace(playerName))
		{
			playerName = "Unknown"; // Eğer isim girilmemişse, varsayılan bir isim ver
		}

		// Bağlantıya oyuncu ekleyelim
		var player = PlayerManager.Instance.CreatePlayer(playerName, Context.ConnectionId);
		var tableId = player.Table.Id;
		Console.WriteLine($"{player.Name} adlı oyuncu {tableId}. masaya katıldı.");

		await Clients.Caller.SendAsync("ReceiveConnection", player.ConnectionId, tableId);


		var players = TableManager.Instance.GetPlayersInTable(tableId);

		// Odaya katıldığını bildir
		await SendMessage($"{playerName} masaya katıldı!");

		string a = string.Join("\n", players.Select(x=>x.Name));

		await SendMessageToPlayer($"Masadaki oyuncular:\n{a}", player.ConnectionId);



		if (TableManager.Instance.GetPlayerCount(tableId) != 1)
		{
			await DealCards();
		}



		await base.OnConnectedAsync();
	}
	private async Task DisconnectPlayer(string connectionId)
	{
		Player player = PlayerManager.Instance.GetPlayer(connectionId);

		// Oyuncuya bağlı olan tüm istemcilere, oyuncunun ayrıldığını bildir
		await SendMessage($"{player.Name} masadan ayrıldı!");

		// Oyuncunun bağlantısını kes
		await base.OnDisconnectedAsync(null);

		// Bağlantısını kes
		Context.Abort();
	}



	// Bağlantı kopma işleminde bu metod devreye girecek.
	public override async Task OnDisconnectedAsync(System.Exception exception)
	{
		var user = PlayerManager.Instance.GetPlayer(Context.ConnectionId);

		Console.WriteLine($"{user.Name} adlı oyuncu {user.Table.Id}. masadan ayrıldı.");
		// Bağlantısı kopan kullanıcıyı bilgilendiriyoruz
		await SendMessage($"{user.Name} masadan ayrıldı!");


		PlayerManager.Instance.LogOut(user);


		await base.OnDisconnectedAsync(exception);
	}

	#endregion
}
