

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
			builder.Services.AddSingleton<IdleKickerService>();  // Servis kayd�n� yap�yoruz
			builder.Services.AddHostedService<IdleKickerService>();  // Arka plan servisini �al��t�rmak i�in

			var app = builder.Build();

			// Hub'� ba�latmak i�in IHubContext<ChatHub> kullan�yoruz
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

			// SignalR Hub'� i�in endpoint tan�ml�yoruz
			app.MapHub<GameHub>("/gameHub");

			app.Run();
		}
	}
}
