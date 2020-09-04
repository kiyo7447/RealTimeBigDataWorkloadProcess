using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiskQueue;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcGreeter
{
	public class GreeterService : Greeter.GreeterBase
	{
		private readonly ILogger<GreeterService> _logger;

		//		IPersistentQueue queue = new PersistentQueue("queue_a");

		//private object _lock = new object();

		/// <summary>
		/// context用のsingletonサービスを作るべき
		/// </summary>
		public static Queue MessageQueue = new Queue(); 

		public GreeterService(ILogger<GreeterService> logger)
		{
			_logger = logger;
		}

		public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
		{
			//_logger.LogInformation($"Saying hello to {request.Name} sv time:{DateTime.Now.ToString("HH:mm:ss.FFF")}");

			var message = $"Hello {request.Name} sv time:{DateTime.Now.ToString("HH:mm:ss.FFF")}";
			//IPersistentQueue queue = new PersistentQueue("queue_a");

#if true
			//↓この処理が無ければ、、7～8msで処理が完了する。この処理があると70～90msmもかかる。
			//Enqueue(message);
			lock (MessageQueue.SyncRoot)
			{
				MessageQueue.Enqueue(message);
			}

			//クライアントタイムアウトのテスト用
			//120分
			//Thread.Sleep(1000 * 60 * 120);

#else
			//↓この処理はなぜか動作しない。二周目がDiskQueueの中で、System.IO.IOExceptionが発生し、先に進まなくなる。
			//原因は不明。。
			if (_queue == null)
			{
				_queue = PersistentQueue.WaitFor("queue_a", TimeSpan.FromSeconds(30));
				_session = _queue.OpenSession();
			}
			_session.Enqueue(Encoding.UTF8.GetBytes(message));
#endif

			//非同期メソッドを使用しても親のメソッド自体は非同期処理を待つため性能は変わらない。
			return Task.FromResult(new HelloReply
			{
				Message = message
			}); ;
		}

		IPersistentQueue _queue = null;
		IPersistentQueueSession _session = null;

		private async Task Enqueue(string message)
		{
			//			lock(_lock)
			using (var queue = PersistentQueue.WaitFor("queue_a", TimeSpan.FromSeconds(30)))
			using (var session = queue.OpenSession())
			{
				for (int i = 0; i < 100; i++)
				{
					session.Enqueue(Encoding.UTF8.GetBytes(message));
				}
				session.Flush();
			}
			return;
		}
	}
}
