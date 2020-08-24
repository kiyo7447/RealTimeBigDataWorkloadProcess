
この処理は例外所外が酷い。

あと、長時間処理中の停止処理がテストが必要。

	DequeueされたあとにRDMSに書き込みで遅延が発生した場合の動きを明確化する必要がある。

	また、それが失敗したときの動きを明確化する必要がある。



 ■エラー集
１）Queueのオープンでタイムアウトエラーが発生した。




２）session.Dequeue();でエラーが発生した。
System.IO.IOException
  HResult=0x80070020
  Message=The process cannot access the file because it is being used by another process.
  Source=System.IO.FileSystem
  スタック トレース:
   at System.IO.FileSystem.MoveFile(String sourceFullPath, String destFullPath, Boolean overwrite)
   at DiskQueue.Implementation.Atomic.Write(String path, Action`1 action)
   at DiskQueue.Implementation.PersistentQueueImpl.CommitTransaction(ICollection`1 operations)
   at DiskQueue.Implementation.PersistentQueueSession.Flush()
   at GrpcGreeter.HostedServices.DequeueQueueHostedService.DoWork(Object state) in D:\dev\sample_gomi\20200819_.NETCore_gRPC\RealTimeBigDataWorkloadProcess\DequeueClientConsoleApp\HostedServices\DequeueQueueHostedService.cs:line 63
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Threading.TimerQueueTimer.CallCallback(Boolean isThreadPool)
   at System.Threading.TimerQueueTimer.Fire(Boolean isThreadPool)
   at System.Threading.TimerQueue.FireNextTimers()

  この例外は、最初にこの呼び出し履歴 
    System.IO.FileSystem.MoveFile(string, string, bool)
    DiskQueue.Implementation.Atomic.Write(string, System.Action<System.IO.Stream>)
    DiskQueue.Implementation.PersistentQueueImpl.CommitTransaction(System.Collections.Generic.ICollection<DiskQueue.Implementation.Operation>)
    DiskQueue.Implementation.PersistentQueueSession.Flush()
    GrpcGreeter.HostedServices.DequeueQueueHostedService.DoWork(object) (DequeueQueueHostedService.cs 内)
    System.Threading.ExecutionContext.RunInternal(System.Threading.ExecutionContext, System.Threading.ContextCallback, object)
    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
    System.Threading.TimerQueueTimer.CallCallback(bool)
    System.Threading.TimerQueueTimer.Fire(bool)
    System.Threading.TimerQueue.FireNextTimers() でスローされました
