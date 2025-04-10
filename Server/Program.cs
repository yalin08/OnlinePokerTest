

using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Server
{
	public class Program
	{


		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// SignalR servisini ekliyoruz
			builder.Services.AddSignalR();

			// IdleKickerService'i kaydediyoruz
			builder.Services.AddSingleton<IdleKickerService>();  // Servis kaydýný yapýyoruz
			builder.Services.AddHostedService<IdleKickerService>();  // Arka plan servisini çalýþtýrmak için

			var app = builder.Build();

			// Hub'ý baþlatmak için IHubContext<ChatHub> kullanýyoruz
			HubConnectionManager.Initialize(app.Services.GetRequiredService<IHubContext<GameHub>>());

			app.UseRouting();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();

			// SignalR Hub'ý için endpoint tanýmlýyoruz
			app.MapHub<GameHub>("/gameHub");

			app.Run();
		}
	}
}
