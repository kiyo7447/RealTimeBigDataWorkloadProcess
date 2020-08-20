using Grpc.Net.Client;
using GrpcGreeter;
using System;
using System.Threading.Tasks;

namespace GrpcClinetConsoleApp
{
	/// <summary>
	/// https://docs.microsoft.com/ja-jp/aspnet/core/grpc/client?view=aspnetcore-3.1
	/// 
	/// https://docs.microsoft.com/ja-jp/aspnet/core/grpc/basics?view=aspnetcore-3.1#generated-c-assets
	/// </summary>
	class Program
	{
		static async Task Main(string[] args)
		{
			var tid = Environment.TickCount % 1000;

			Console.WriteLine("Grpc Client Start!");
			var channel = GrpcChannel.ForAddress("https://localhost:5001");
			var client = new Greeter.GreeterClient(channel);

		start:

			HelloReply response = null;
			for (int c =0; c < 100;c++)
			{
				 response = await client.SayHelloAsync(
				new HelloRequest { Name = $"World tid:{tid:D3}, cl time:{DateTime.Now.ToString("HH:mm:ss.FFF")}" });
			}
			Console.WriteLine(response.Message);

			var key = Console.ReadKey();
			//if (key.Key == ConsoleKey.R ) 
				goto start;
		}
	}
}
