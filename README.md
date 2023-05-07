# UnityFlatBufferStreamingLoad

支持使用Stream加载FlatBuffer，从而支持流式加载，只支持读取不支持写入（配置文件正好适用场景），具体看代码：
https://github.com/billwillman/flatbuffers
byteBuffer.cs 中: StreamReadBuffer ByteBuffer

ByteBuffer构造函数的position,就支持把几个大配置内容flatbuffer二进制格式合并成一个大文件。还可以考虑结合weakreference等减少运行时持续内存占用

protobuf的支持：
https://github.com/billwillman/NsTcpClient.git

![image](https://user-images.githubusercontent.com/3533457/236686954-b7e7f8e2-970b-4b61-ab55-9d4ed9e93dd8.png)

