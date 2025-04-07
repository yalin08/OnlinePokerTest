using Microsoft.AspNetCore.SignalR.Client;

Console.Write("Adınızı girin: ");
string username = Console.ReadLine();

var connection = new HubConnectionBuilder()
	.WithUrl($"http://localhost:5246/chatHub?playerName={username}")  // URL'ye oyuncu adı parametresi ekleniyor
	.WithAutomaticReconnect()
	.Build();

connection.On<string, string>("ReceiveMessage", (user, message) =>
{

	Console.WriteLine($"{user}: {message}");
});

await connection.StartAsync();


while (true)
{
	string message = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(message)) continue;

	try
	{
		await connection.InvokeAsync("SendMessage", username, message);
	}
	catch (Exception ex)
	{
		Console.WriteLine("Hata: " + ex.Message);
	}
}