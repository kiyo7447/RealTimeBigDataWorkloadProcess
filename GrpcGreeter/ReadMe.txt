











■エラー集
１）MessageQueue.Enqueue(message);でエラーが発生。

System.Collections.Queueは、スレッドセーフではないので、SyncRootを使用して対応
https://docs.microsoft.com/ja-jp/dotnet/api/system.collections.queue.synchronized?view=netcore-3.1

fail: Grpc.AspNetCore.Server.ServerCallHandler[6]
      Error when executing service method 'SayHello'.
System.ArgumentException: Source array was not long enough. Check the source index, length, and the array's lower bounds. (Parameter 'sourceArray')
   at System.Array.Copy(Array sourceArray, Int32 sourceIndex, Array destinationArray, Int32 destinationIndex, Int32 length, Boolean reliable)
   at System.Array.Copy(Array sourceArray, Int32 sourceIndex, Array destinationArray, Int32 destinationIndex, Int32 length)
   at System.Collections.Queue.SetCapacity(Int32 capacity)
   at System.Collections.Queue.Enqueue(Object obj)
   at GrpcGreeter.GreeterService.SayHello(HelloRequest request, ServerCallContext context) in D:\dev\sample_gomi\20200819_.NETCore_gRPC\RealTimeBigDataWorkloadProcess\GrpcGreeter\Services\GreeterService.cs:line 43
   at Grpc.Shared.Server.UnaryServerMethodInvoker`3.Invoke(HttpContext httpContext, ServerCallContext serverCallContext, TRequest request)
   at Grpc.Shared.Server.UnaryServerMethodInvoker`3.Invoke(HttpContext httpContext, ServerCallContext serverCallContext, TRequest request)
   at Grpc.AspNetCore.Server.Internal.CallHandlers.UnaryServerCallHandler`3.HandleCallAsyncCore(HttpContext httpContext, HttpContextServerCallContext serverCallContext)
   at Grpc.AspNetCore.Server.Internal.CallHandlers.ServerCallHandlerBase`3.<HandleCallAsync>g__AwaitHandleCall|8_0(HttpContextServerCallContext serverCallContext, Method`2 method, Task handleCall)
   
   
   
   
