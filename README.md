# RealTimeBigDataWorkloadProcess
大量の受信データを効率よく取り込む
- 実際の処理中のPCの利用状況
![2020-09-04_16h03_33](https://user-images.githubusercontent.com/3734512/92209693-67c42880-eec8-11ea-91f5-c7cf0f1d82de.png)

# GrpcGreeter
メッセージの受信サービス（非同期のホステッドサービスで永続化を一括で非同期化）
- 50msで1,000メッセージを処理可能（メッセージサイズ12Kbyte）
![2020-09-04_16h03_50](https://user-images.githubusercontent.com/3734512/92209703-6c88dc80-eec8-11ea-9bdf-39a5e8a84b5b.png)

# GrpcClinetConsoleApp
メッセージ送信のクライアント。一回あたり１００メッセージを送信する。本稼働の実際のメッセージは、１メッセージ2kbとみている。
- クライアントは10プロセスを立ち上げ、ランダム間隔（100ms～5000ms）で１送信当たり100メッセージを送信する。（メッセージサイズ12Kbyte）
![2020-09-04_16h04_09](https://user-images.githubusercontent.com/3734512/92209723-727ebd80-eec8-11ea-929b-a5ab5eeb0cb1.png)

# DequeueClientConsoleApp
受信メッセージの刈り取り処理。複数起動して処理する。
- 1,000メッセージの取り出しを50ms～100msで取り出して処理します。（メッセージサイズ12Kbyte）
![2020-09-04_16h03_57](https://user-images.githubusercontent.com/3734512/92209715-6f83cd00-eec8-11ea-81b3-8a6678d1dd10.png)

# 課題
処理性能では、DISK I/Oがボトルネックとなっていますので、永続キュー（DiskQueue）をディスク分散させて性能を稼ぐ必要があるとみています。
私のPCでは二本目のNVMeが刺さらない、、、10世代のXPS15か、ThinkPadが欲しいところです。
とはいえ、秒間１万メッセージが捌ければ今のテーマのユーザ要件は十分に満たしています。

# ログ
一括Dequeue処理, cnt=6776, 処理時間=566ms（メッセージサイズ2Kbyte）

