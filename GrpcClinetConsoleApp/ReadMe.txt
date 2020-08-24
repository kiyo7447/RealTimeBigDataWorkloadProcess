
この処理は例外所外が酷い。

正しいバッチ処理ではない。ホステッド化は必要。

設定ファイルの対応がない。Json化された設定ファイル。

サーバ処理要求のタイムアウトの設定がない。

コネクションタイムアウトとコマンドタイムアウトの疑似分離

    private async void SampleCode()
     {
          var client = await GetClient();
          var data = await client.GetAllTemplatesAsync(request, new 
                CallOptions().WithDeadline(DateTime.UtcNow.AddSeconds(7)));

     }

    private async Task<MyGrpcClient> GetClient()
    {
        var channel = new Channel("somehost",23456, ChannelCredentials.Insecure);
        await channel.ConnectAsync(deadline: DateTime.UtcNow.AddSeconds(2));
        return new MyGrpcClient(channel);
    }


インスタンス管理の適正化、リトライ処理

    https://user-first.ikyu.co.jp/entry/2019/08/02/190000
    チェネルはインスタンスを使い回すほうが良さそう

    Pollyを使ったリトライ処理を実装したほうが良さそう


デフォルトのタイムアウト値

    ・・・実験してみたが、、、タイムアウトは発生しないようだ。とりあえずは、１時間は待ってみようと思う。
    まってみましたが、タイムアウトしない。。。gRPCのデフォルトはタイムアウトしない。

    と思って、ここはクローズする。


gRPCはめちゃめちゃ早いけど、クライアント側の実装には、癖がある。
    コネクション、デッドライン、タイムアウト、障害処理、色々と考慮が必要となります。

    Webブラウザではクライアントが使えないけど（grpc-webがある）、性能を見たら私用一択です。




■エラー集
１）処理中に強制的にサーバから切断された場合
System.SystemException
  HResult=0x80131501
  Message=メッセージの送信xxx、処理時間=13343ms
  Source=GrpcClinetConsoleApp
  スタック トレース:
   at GrpcClinetConsoleApp.Program.<Main>d__0.MoveNext() in D:\dev\sample_gomi\20200819_.NETCore_gRPC\RealTimeBigDataWorkloadProcess\GrpcClinetConsoleApp\Program.cs:line 45
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at GrpcClinetConsoleApp.Program.<Main>(String[] args)

内部例外 1:
RpcException: Status(StatusCode="Internal", Detail="Error starting gRPC call. HttpRequestException: An error occurred while sending the request. IOException: The request was aborted. IOException: The response ended prematurely, with at least 9 additional bytes expected.", DebugException="System.Net.Http.HttpRequestException: An error occurred while sending the request.
 ---> System.IO.IOException: The request was aborted.
 ---> System.IO.IOException: The response ended prematurely, with at least 9 additional bytes expected.
   at System.Net.Http.Http2Connection.ReadAtLeastAsync(Stream stream, Memory`1 buffer, Int32 minReadBytes)
   at System.Net.Http.Http2Connection.EnsureIncomingBytesAsync(Int32 minReadBytes)
   at System.Net.Http.Http2Connection.ReadFrameAsync(Boolean initialFrame)
   at System.Net.Http.Http2Connection.ProcessIncomingFramesAsync()
   --- End of inner exception stack trace ---
   at System.Net.Http.Http2Connection.Http2Stream.CheckResponseBodyState()
   at System.Net.Http.Http2Connection.Http2Stream.TryEnsureHeaders()
   at System.Net.Http.Http2Connection.Http2Stream.ReadResponseHeadersAsync(CancellationToken cancellationToken)
   at System.Net.Http.Http2Connection.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   --- End of inner exception stack trace ---
   at System.Net.Http.Http2Connection.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   at System.Net.Http.HttpConnectionPool.SendWithRetryAsync(HttpRequestMessage request, Boolean doRequestAuth, CancellationToken cancellationToken)
   at System.Net.Http.RedirectHandler.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   at Grpc.Net.Client.Internal.GrpcCall`2.RunCall(HttpRequestMessage request, Nullable`1 timeout)")


２）通信がタイム・アウトした場合のエラー
System.SystemException
  HResult=0x80131501
  Message=メッセージの送信xxx、処理時間=10484ms
  Source=GrpcClinetConsoleApp
  スタック トレース:
   at GrpcClinetConsoleApp.Program.<Main>d__0.MoveNext() in D:\dev\sample_gomi\20200819_.NETCore_gRPC\RealTimeBigDataWorkloadProcess\GrpcClinetConsoleApp\Program.cs:line 46
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at GrpcClinetConsoleApp.Program.<Main>(String[] args)

内部例外 1:
RpcException: Status(StatusCode="DeadlineExceeded", Detail="")
Grpc.Core.RpcException: Status(StatusCode="DeadlineExceeded", Detail="")
   at GrpcClinetConsoleApp.Program.Main(String[] args) in D:\dev\sample_gomi\20200819_.NETCore_gRPC\RealTimeBigDataWorkloadProcess\GrpcClinetConsoleApp\Program.cs:line 33

