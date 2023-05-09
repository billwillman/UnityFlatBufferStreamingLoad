# UnityFlatBufferStreamingLoad

支持使用Stream加载FlatBuffer，从而支持流式加载，只支持读取不支持写入（配置文件正好适用场景），具体看代码：
https://github.com/billwillman/flatbuffers
byteBuffer.cs 中: StreamReadBuffer ByteBuffer（注意这个flatbuffers库是被我修改过的），这里只改造了C# 3.5的没开启System.Memory的容器（例如：Span<T>等，用这些容器可以使用更少GC，不过需要重新改造了）

ByteBuffer构造函数的position,就支持把几个大配置内容flatbuffer二进制格式合并成一个大文件。还可以考虑结合weakreference等减少运行时持续内存占用

FlatBuffer存储内存结构讲解：
https://gitee.com/ReallyT-bag/study_notes/blob/master/protocol/flatbuffer%E7%BC%96%E7%A0%81%E7%BB%93%E6%9E%84.md

~protobuf的支持：
https://github.com/billwillman/NsTcpClient.git~

## 可以考虑找比较新的版本，使用pb的c# 优化gc版本具体参考(已经改为22.4版本的protobuf，工程里用的是.net stard2.0版本，其他.net framework的版本在根目录protobuffer下, old_proto是老版本3.5不建议使用)：
https://www.cnblogs.com/egmkang/p/14171962.html

## 通过FileStream流式搜索实时读取Demo:

![image](https://user-images.githubusercontent.com/3533457/236686954-b7e7f8e2-970b-4b61-ab55-9d4ed9e93dd8.png)

N个策划大配置可以采用N个index file(protobuf Encode，items里是key和index + 一个 data file的偏移，整体数据就是 itemCount * [key, index] + dataFileOffset，打包全部进ab内)和合并一个data file(flatbuffer Encode，不走AB)

