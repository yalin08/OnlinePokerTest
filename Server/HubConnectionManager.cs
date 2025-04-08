using Entities;
using Managers;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public static class HubConnectionManager
{
    private static IHubContext<ChatHub> _hubContext;

    // Hub'ı başlatmak için bu fonksiyon kullanılacak
    public static void Initialize(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    // Mesaj gönderme
    public static async Task SendMessage(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", null, message);
    }

    // Bağlantıyı kesme
    public static async Task DisconnectPlayer(Player player)
    {
        await _hubContext.Clients.Client(player.ConnectionId).SendAsync("ReceiveMessage", null, "Uzun süre işlem yapmadığınız için bağlantınız kesildi.");
        await _hubContext.Clients.Client(player.ConnectionId).SendAsync("Disconnect");

        Console.WriteLine(player.Name);

		await PlayerManager.Instance.LogOut(player);
	}
}
