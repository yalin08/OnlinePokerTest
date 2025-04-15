using Entities.Game;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

Console.Write("Adınızı girin: ");
string username = Console.ReadLine();
string connectionId = string.Empty;
int table = 0;

var connection = new HubConnectionBuilder()
	.WithUrl($"http://localhost:5246/gameHub?playerName={username}")  // oyuncu adı URL üzerinden gönderiliyor
	.WithAutomaticReconnect()
	.Build();



// Olaylar önce tanımlanmalı
connection.On<string, string>("ReceiveMessage", (user, message) =>
{
	Console.WriteLine(user == null ? $"{message}" : $"{user}:{message}");
});

connection.On<string>("GetCards", (json) =>
{
	var cards = JsonSerializer.Deserialize<List<Card>>(json);
	if (cards != null)
	{
		Console.WriteLine("Elindeki kartlar:");
		foreach (Card card in cards)
		{
			Console.WriteLine(card.ToString());
		}
	}
});



connection.On<string, int>("ReceiveConnection", (connId, tableId) =>
{
	connectionId = connId;
	table = tableId;
	Console.Clear();
	Console.WriteLine($"Masaya başarıyla bağlanıldı!\nHoşgeldin {username}\nSunucudan gelen ConnectionId: {connectionId} \nMasa Id={table}\n");
});

connection.On("Disconnect", () =>
{
	// Bağlantıyı istemci tarafında kapatıyoruz
	Console.WriteLine("Bağlantınız kesildi.");
	connection.InvokeAsync("DisconnectPlayer", connectionId);
});

// Bağlantı en son başlatılmalı
await connection.StartAsync();

// Ana döngü
while (true)
{
	string message = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(message)) continue;



	int currentLineCursor = Console.CursorTop;
	Console.SetCursorPosition(0, currentLineCursor - 1);
	Console.Write(new string(' ', Console.WindowWidth));
	Console.SetCursorPosition(0, currentLineCursor - 1);


	try
	{
		await connection.InvokeAsync("SendMessage", message, username);
	}
	catch (Exception ex)
	{
		Console.WriteLine("Hata: " + ex.Message);
	}
}
