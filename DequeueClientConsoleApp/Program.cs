using DiskQueue;
using GrpcGreeter.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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
				.ConfigureServices((hostContext, service) =>
				{
				service.AddHostedService<DequeueQueueHostedService>();
				})
				.Build();

			host.Run();
		}
	}
}
