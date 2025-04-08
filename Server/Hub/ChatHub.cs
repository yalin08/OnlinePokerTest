using Entities;
using Managers;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Threading.Tasks;

public class ChatHub : Hub
{
	// Bu metod, istemciden gelen mesajları alır ve tüm bağlı istemcilere iletir.
	public async Task SendMessage(int tableid,string user, string message)
	{
		var table = TableManager.Instance.GetTableById(tableid);


		Debug.WriteLine(table.Players[0].Name);
		if (table == null)
		{
			await Clients.Caller.SendAsync("ReceiveMessage", "Sunucu", "Masa bulunamadı.");
			return;
		}

		foreach (var player in table.Players.Where(p => p != null))
		{
			await Clients.Client(player.ConnectionId).SendAsync("ReceiveMessage", user, message);
		}
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
		var player=PlayerManager.Instance.CreatePlayer(playerName, Context.ConnectionId);
		var tableId = player.Table.Id;
		Debug.WriteLine(player.Name+" "+tableId);
		await Clients.Caller.SendAsync("ReceiveConnection", player.ConnectionId, tableId);


		// Odaya katıldığını bildir
		await SendMessage(tableId, "Sunucu", $"{playerName} masaya katıldı!");
		await base.OnConnectedAsync();
	}

	// Bağlantı kopma işleminde bu metod devreye girecek.
	public override async Task OnDisconnectedAsync(System.Exception exception)
	{
		var user = PlayerManager.Instance.GetPlayer(Context.ConnectionId);
		
		// Bağlantısı kopan kullanıcıyı bilgilendiriyoruz
		await SendMessage(user.Table.Id, "Sunucu", $"{user.Name} masadan ayrıldı!");

		await base.OnDisconnectedAsync(exception);
	}
}
