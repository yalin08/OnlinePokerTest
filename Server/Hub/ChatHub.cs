using Managers;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
	// Bu metod, istemciden gelen mesajları alır ve tüm bağlı istemcilere iletir.
	public async Task SendMessage(string user, string message)
	{
		// Sunucuda tüm bağlı istemcilere mesaj gönderiyoruz
		await Clients.All.SendAsync("ReceiveMessage", user, message);
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
		var player=PlayerManager.Instance.CreatePlayer(Context.ConnectionId, playerName);


		// Odaya katıldığını bildir
		await Clients.All.SendAsync("ReceiveMessage", "Sunucu", $"{playerName} masaya katıldı!");
		await base.OnConnectedAsync();
	}

	// Bağlantı kopma işleminde bu metod devreye girecek.
	public override async Task OnDisconnectedAsync(System.Exception exception)
	{
		var user = Context.UserIdentifier;

		// Bağlantısı kopan kullanıcıyı bilgilendiriyoruz
		await Clients.All.SendAsync("ReceiveMessage", "Sunucu", $"{user} masadan ayrıldı!");

		await base.OnDisconnectedAsync(exception);
	}
}
