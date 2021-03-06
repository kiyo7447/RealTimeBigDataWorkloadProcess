﻿using DiskQueue;
using Microsoft.AspNetCore.Mvc.Routing;
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
	public class PersistentQueueHostedService : IHostedService, IDisposable
	{
		private int executionCount = 0;
		private readonly ILogger<PersistentQueueHostedService> _logger;
		private Timer _timer;

		public PersistentQueueHostedService(ILogger<PersistentQueueHostedService> logger)
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

			if (GreeterService.MessageQueue.Count > 0)
			{
				var start = Environment.TickCount;
				var cnt = 0;
				var byteCount = 0;
				using (var queue = PersistentQueue.WaitFor("queue_a", TimeSpan.FromSeconds(30)))
				using (var session = queue.OpenSession())
				{
					//スレッドセーフではないので、
					lock (GreeterService.MessageQueue.SyncRoot)
					{
						while (GreeterService.MessageQueue.Count > 0)
						{
							var data = Encoding.UTF8.GetBytes((string)GreeterService.MessageQueue.Dequeue());
							byteCount += data.Length;
							session.Enqueue(data); ;
							cnt++;
						}
					}
					session.Flush();
					_logger.LogInformation($"データの永続化, 処理メッセージ数={cnt:N0}, データ量={byteCount:N0}byte, 処理時間={Environment.TickCount - start:N0}ms");

				}

			}
			_logger.LogInformation(
				"Timed Hosted Service is working. Count: {Count}", count);
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
