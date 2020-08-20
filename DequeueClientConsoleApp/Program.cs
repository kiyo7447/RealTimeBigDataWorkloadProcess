using DiskQueue;
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

		start:
			while (true)
			{
				Console.Write(".");
				Thread.Sleep(1000);

				var start = Environment.TickCount;
				var cnt = 0;

				//using (var queue = PersistentQueue.WaitFor(@"..\..\..\..\GrpcGreeter\queue_a", TimeSpan.FromSeconds(30)))
				using (var queue = PersistentQueue.WaitFor(@"queue_a", TimeSpan.FromSeconds(30)))
				using (var session = queue.OpenSession())
				{
					while (true)
					{
						var data = session.Dequeue();
						if (data == null)
						{
							session.Flush();
							break;
						}
						else
						{
							cnt++;
							Console.WriteLine($"{Encoding.UTF8.GetString(data)}, cnt={cnt}");
						}
					}
					if (cnt > 0)
						Console.WriteLine($"一括Dequeue処理, cnt={cnt}, 処理時間={Environment.TickCount - start}ms");
				}
			}
			var key = Console.ReadKey();
			if (key.Key == ConsoleKey.R) goto start;
		}
	}
}
