
namespace Server
{
	public class Program
	{


		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();


			// SignalR servisini ekliyoruz
			builder.Services.AddSignalR();

			var app = builder.Build();
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

			// Endpoint'leri tan�ml�yoruz
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<ChatHub>("/chatHub");
			});
			

			app.Run();
		}
	}
}
