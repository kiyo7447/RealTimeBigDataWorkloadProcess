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
		/// context�p��singleton�T�[�r�X�����ׂ�
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
			//�����̏�����������΁A�A7�`8ms�ŏ�������������B���̏����������70�`90msm��������B
			//Enqueue(message);
			lock (MessageQueue.SyncRoot)
			{
				MessageQueue.Enqueue(message);
			}

			//�N���C�A���g�^�C���A�E�g�̃e�X�g�p
			//120��
			//Thread.Sleep(1000 * 60 * 120);

#else
			//�����̏����͂Ȃ������삵�Ȃ��B����ڂ�DiskQueue�̒��ŁASystem.IO.IOException���������A��ɐi�܂Ȃ��Ȃ�B
			//�����͕s���B�B
			if (_queue == null)
			{
				_queue = PersistentQueue.WaitFor("queue_a", TimeSpan.FromSeconds(30));
				_session = _queue.OpenSession();
			}
			_session.Enqueue(Encoding.UTF8.GetBytes(message));
#endif

			//�񓯊����\�b�h���g�p���Ă��e�̃��\�b�h���͔̂񓯊�������҂��ߐ��\�͕ς��Ȃ��B
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
