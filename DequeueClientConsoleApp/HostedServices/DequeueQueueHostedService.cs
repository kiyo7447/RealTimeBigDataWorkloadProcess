using DiskQueue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcGreeter.HostedServices
{
	public class DequeueQueueHostedService : IHostedService, IDisposable
	{
		private int executionCount = 0;
		private readonly ILogger<DequeueQueueHostedService> _logger;
		private Timer _timer;

		public DequeueQueueHostedService(ILogger<DequeueQueueHostedService> logger)
		{
			_logger = logger;
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Timed Hosted Service running.");

			_timer = new Timer(DoWork, null, TimeSpan.Zero,
				TimeSpan.FromSeconds(3));

			return Task.CompletedTask;
		}

		private void DoWork(object state)
		{
			var count = Interlocked.Increment(ref executionCount);
			_logger.LogInformation(
				"Timed Hosted Service is working. Count: {Count}", count);

			Console.Write(".");
#if DEBUG
			using (var queue = PersistentQueue.WaitFor(@"..\..\..\..\GrpcGreeter\queue_a", TimeSpan.FromSeconds(30)))

#else
			using (var queue = PersistentQueue.WaitFor(@"queue_a", TimeSpan.FromSeconds(30)))
#endif

			using (var session = queue.OpenSession())
			{
				var start = Environment.TickCount;
				var cnt = 0;
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

		public Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Timed Hosted Service is stopping.");

			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}

}
