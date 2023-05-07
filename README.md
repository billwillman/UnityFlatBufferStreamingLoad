# UnityFlatBufferStreamingLoad

支持使用Stream加载FlatBuffer，从而支持流式加载，只支持读取不支持写入（配置文件正好适用场景），具体看代码：
https://github.com/billwillman/flatbuffers
byteBuffer.cs 中: StreamReadBuffer ByteBuffer（注意之恶个flatbuffers库是被我修改过的）

ByteBuffer构造函数的position,就支持把几个大配置内容flatbuffer二进制格式合并成一个大文件。还可以考虑结合weakreference等减少运行时持续内存占用

protobuf的支持：
https://github.com/billwillman/NsTcpClient.git

通过FileStream流式搜索实时读取Demo:

![image](https://user-images.githubusercontent.com/3533457/236686954-b7e7f8e2-970b-4b61-ab55-9d4ed9e93dd8.png)

N个策划大配置可以采用N个index file(protobuf Encode，items里是key和index + 一个 data file的偏移，整体数据就是 itemCount * [key, index] + dataFileOffset，打包全部进ab内)和合并一个data file(flatbuffer Encode，不走AB)

