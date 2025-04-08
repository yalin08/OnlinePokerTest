using Microsoft.AspNetCore.SignalR.Client;

Console.Write("Adınızı girin: ");
string username = Console.ReadLine();
string connectionId = string.Empty;
int table = 0;

var connection = new HubConnectionBuilder()
	.WithUrl($"http://localhost:5246/chatHub?playerName={username}")  // oyuncu adı URL üzerinden gönderiliyor
	.WithAutomaticReconnect()
	.Build();

// Olaylar önce tanımlanmalı
connection.On<string, string>("ReceiveMessage", (user, message) =>
{
	Console.WriteLine($"{user}: {message}");
});

connection.On<string, int>("ReceiveConnection", (connId, tableId) =>
{
	connectionId = connId;
	table = tableId;
	Console.WriteLine($"Sunucudan gelen ConnectionId: {connectionId} \nMasa Id={table}");
});

// Bağlantı en son başlatılmalı
await connection.StartAsync();


// Ana döngü
while (true)
{
	string message = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(message)) continue;

	try
	{
		await connection.InvokeAsync("SendMessage", table, username, message);
	}
	catch (Exception ex)
	{
		Console.WriteLine("Hata: " + ex.Message);
	}
}
