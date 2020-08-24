using DequeueClientConsoleApp;
using DiskQueue;
using GrpcGreeter.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace InsertDbConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Dequeue Client Start!");
			var host = new HostBuilder()
				.ConfigureHostConfiguration(config =>
				{
					config.SetBasePath(Directory.GetCurrentDirectory());
					config.AddJsonFile("appsettings.json", optional: true);
					config.AddCommandLine(args);
				})
				.ConfigureServices((hostContext, services) =>
				{
					services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));
					services.AddHostedService<DequeueQueueHostedService>();
				})
				.ConfigureLogging((hostContext, config) => 
				{
					config.AddConsole();
					config.AddDebug();
				})
				.Build();

			host.Run();
		}
	}
}
