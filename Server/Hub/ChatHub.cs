using Entities;
using Managers;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

public class ChatHub : Hub
{

	public ChatHub()
	{
		
	}

	// Her 5 saniyede bir bu metod çalışacak
	private async void IdleKicker(object state)
	{
		var players = TableManager.Instance.Tables.SelectMany(t => t.Players).Where(x=>x!=null).ToList();

		// Tüm oyuncuları kontrol et
		foreach (var player in players)
		{
			if (player.LastInteractionTime < DateTime.Now.AddSeconds(-5)) // 5 saniyeden fazla idle kalan oyuncu
			{
				var table = TableManager.Instance.GetTableById(player.Table.Id);
				if (table != null)
				{
					// Oyuncuyu bağlantıdan çıkar
					await SendMessageToPlayer("Idle olduğunuz için bağlantınız kesildi.",player.ConnectionId);
					//await DisconnectPlayer(player);
					Debug.WriteLine($"{player.Name} adlı oyuncu idle'dan dolayı masadan çıkarıldı.");
				}
			}
		}
	}

	// Oyuncuyu masadan çıkaran metod
	private async Task DisconnectPlayer(Player player)
	{
		PlayerManager.Instance.LogOut(player);


		// Oyuncuya bağlı olan tüm istemcilere, oyuncunun ayrıldığını bildir
		await SendMessage($"{player.Name} masadan ayrıldı!");
		// Bağlantısını kes
		await Clients.Client(player.ConnectionId).SendAsync("Disconnect");
		// Oyuncunun bağlantısını kes
		await base.OnDisconnectedAsync(null);
	}

	// Bu metod, istemciden gelen mesajları alır ve tüm bağlı istemcilere iletir.
	public async Task SendMessage(string message, string user=null)
	{
		Debug.WriteLine("???");
		var Player = PlayerManager.Instance.GetPlayer(Context.ConnectionId);
		var table = TableManager.Instance.GetTableById(Player.Table.Id);
		if (Player != null)
		{
			Player.UpdateInteractionTime(); // Mesaj gönderildiğinde idleTime'ı günceller
		}

		Debug.WriteLine($"");
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

	public  async Task SendMessageToPlayer(string message, string connectionId)
	{
		var Player = PlayerManager.Instance.GetPlayer(Context.ConnectionId);
		var table = TableManager.Instance.GetTableById(Player.Table.Id);

		Debug.WriteLine($"");
		if (table == null)
		{
			await Clients.Client(connectionId).SendAsync("ReceiveMessage", null, "Masa bulunamadı.");
			return;
		}
		if (Player == null)
		{
			return;
		}

			await Clients.Client(connectionId).SendAsync("ReceiveMessage", null, message);
		
	}


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
		Debug.WriteLine($"{player.Name} adlı oyuncu {tableId}. masaya katıldı.");

		await Clients.Caller.SendAsync("ReceiveConnection", player.ConnectionId, tableId);


		List<string> players=TableManager.Instance.GetPlayersInTable(tableId);
		
		// Odaya katıldığını bildir
		await SendMessage($"{playerName} masaya katıldı!");

		string a = string.Join("\n", players);

		await SendMessageToPlayer($"Masadaki oyuncular:\n{a}",player.ConnectionId);
		await base.OnConnectedAsync();
	}

	// Bağlantı kopma işleminde bu metod devreye girecek.
	public override async Task OnDisconnectedAsync(System.Exception exception)
	{
		var user = PlayerManager.Instance.GetPlayer(Context.ConnectionId);

		Debug.WriteLine($"{user.Name} adlı oyuncu {user.Table.Id}. masadan ayrıldı.");
		// Bağlantısı kopan kullanıcıyı bilgilendiriyoruz
		await SendMessage($"{user.Name} masadan ayrıldı!");

		await base.OnDisconnectedAsync(exception);
	}
}
