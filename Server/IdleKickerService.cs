using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities;
using Managers;
using System.Diagnostics;

public class IdleKickerService : BackgroundService
{
	private readonly IServiceProvider _services;
	float kickTime=180;
	public IdleKickerService(IServiceProvider services)
	{
		_services = services;
	
	}

	// Arka planda her 5 saniyede bir çalışan görev
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
	//Her 90 saniyede bir kontrol ediyor,eğer oyuncu afk ise oyundan atıyor.
			await Task.Delay(TimeSpan.FromSeconds(90), stoppingToken);
			await IdleKicker();
		}
	}

	// Idle kontrolü yapan metod
	private async Task IdleKicker()
	{
		

		var players = TableManager.Instance.Tables.SelectMany(t => t.Players).Where(x => x != null).ToList();

		foreach (var player in players)
		{
			
			if ((DateTime.Now - player.LastInteractionTime) > TimeSpan.FromSeconds(kickTime)) //kicktime saniyeden fazla idle kalan oyuncu
			{

				var table = TableManager.Instance.GetTableById(player.Table.Id);
				if (table != null)
				{
Console.WriteLine(player.Name+" atıldı");
					// Oyuncuyu bağlantıdan çıkar
					await RemovePlayerFromTable(player);
				
				}
			}
		}
	}

	// Oyuncuyu masadan çıkaran metod
	private async Task RemovePlayerFromTable(Player player)
	{
	

		// Oyuncuya bağlı olan tüm istemcilere, oyuncunun ayrıldığını bildir
		await HubConnectionManager.SendMessage($"{player.Name} ATILDI!");
		// Oyuncunun bağlantısını kes
		await HubConnectionManager.DisconnectPlayer(player);


	}
}
