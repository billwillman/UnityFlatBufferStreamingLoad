using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Google.FlatBuffers;
using Google.Protobuf;
using System.IO;
using UnityEditor;
using UnityEngine;
using LitJson;

namespace NsConfig
{

    public class IDMap
    {
        public int dataFileOffset;
        public Dictionary<string, int> items;

        public IDMap() {
            items = new Dictionary<string, int>();
        }
    }

    public static class BuildData<T> where T : IFlatbufferObject
    {
        private static ByteBuffer m_DataBuffer;
        static ByteBuffer GetByteBuffer(Stream stream, int offset = 0) {
            // ByteBuffer构造函数的position,就支持把几个大配置内容flatbuffer二进制格式合并成一个大文件
            if (m_DataBuffer == null)
                m_DataBuffer = new ByteBuffer(new StreamReadBuffer(stream), offset);
            else
                m_DataBuffer.ResetReadOnly(stream, offset);
            return m_DataBuffer;
        }

        private static T LoadDataFromFlatBuffer(string flatBufferFileName) {
            var tt = typeof(T);
            string methodName = "GetRootAs" + tt.Name;
            var methodInfo = tt.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            var data = AssetDatabase.LoadAssetAtPath<TextAsset>(flatBufferFileName);
            MemoryStream stream = new MemoryStream(data.bytes);
            try {
                ByteBuffer byteBuffer = GetByteBuffer(stream);
                T ret = (T)methodInfo.Invoke(tt, new object[] { byteBuffer });
                return ret;
            } finally {
                stream.Dispose();
            }
        }

        [MenuItem("Tools/Config/Monster.json")]
        public static void BuildMonsterCfg() {
            Build("Assets/Resources/Monster.json");
        }

        public static void Build(string dataJsonFileName, string keyFieldName = "id") {
            string onlyFileName = Path.GetFileNameWithoutExtension(dataJsonFileName);
            string dir = Path.GetDirectoryName(dataJsonFileName).Replace("\\", "/");
            if (!dir.EndsWith("/"))
                dir += "/";

            // 1.生成 data flatbuffer
            Console.WriteLine("build flatbuffer data");
            string fbsFileName = dir + onlyFileName + ".fbs";
            string cmd = string.Format("flatc -b {0} {1}", Path.GetFullPath(fbsFileName).Replace("\\", "/"), 
                Path.GetFullPath(dataJsonFileName).Replace("\\", "/"));
            Console.WriteLine(cmd);


            // 2.从data flatbuffer 到 index file
            string dataFlatBufferFileName = dir + onlyFileName + "_flatbuffer.bytes";
            T ret = LoadDataFromFlatBuffer(dataFlatBufferFileName);
            var tt = typeof(T);
            var lenProp = tt.GetProperty("ItemsLength");
            var itemFunc = tt.GetMethod("Items", BindingFlags.Public | BindingFlags.Instance);
            int len = (int)lenProp.GetValue(ret);
            object[] pp = new object[1];
            PropertyInfo prop = null;
            IDMap idMap = null;
            for (int i = 0; i < len; ++i) {
                pp[0] = i;
                var m = itemFunc.Invoke(ret, pp);
                if (m != null) {
                    if (prop == null)
                        prop = m.GetType().GetProperty("HasValue");
                    if ((bool)prop.GetValue(m)) {
                        if (idMap == null) {
                            idMap = new IDMap();
                        }
                        idMap.items.TryAdd(m.GetType().GetProperty("keyFieldName").GetValue(m).ToString(), i);
                    }
                }
            }

            string json = string.Empty;
            if (idMap != null && idMap.items.Count > 0) {
                json = JsonMapper.ToJson(idMap);
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
            string idFileName = dir + onlyFileName + "_Id.json";
            FileStream stream = new FileStream(idFileName, FileMode.Create, FileAccess.Write);
            try {
                stream.Write(buffer);
            } finally {
                stream.Dispose();
            }


            IdMap protoMsg = new IdMap();
            var iter = idMap.items.GetEnumerator();
            try {
                while (iter.MoveNext()) {
                    protoMsg.IdToIdxMap.Add(uint.Parse(iter.Current.Key), iter.Current.Value);
                }
            } finally {
                iter.Dispose();
            }
            buffer = protoMsg.ToByteArray();
            string protoIdFileName = dir + onlyFileName + "_Id_proto.bytes";
            stream = new FileStream(protoIdFileName, FileMode.Create, FileAccess.Write);
            stream.Write(buffer);
            stream.Dispose();

            // 3. Index File Json转Proto
            Console.WriteLine("build idMap proto");

        }

        public static void RunCmd(string command) {

            if (string.IsNullOrEmpty(command))
                return;
#if UNITY_EDITOR_WIN
            command = " /c " + command;
            processCommand("cmd.exe", command);
#elif UNITY_EDITOR_OSX
		processCommand(command, string.Empty);
#endif
        }

        private static void processCommand(string command, string argument) {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(command);
            start.Arguments = argument;
            start.CreateNoWindow = false;
            start.ErrorDialog = true;
            start.UseShellExecute = true;
            //	start.UseShellExecute = false;

            if (start.UseShellExecute) {
                start.RedirectStandardOutput = false;
                start.RedirectStandardError = false;
                start.RedirectStandardInput = false;
            } else {
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                //	start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
                //	start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
                start.StandardOutputEncoding = System.Text.Encoding.Default;
                start.StandardErrorEncoding = System.Text.Encoding.Default;
            }

            System.Diagnostics.Process p = System.Diagnostics.Process.Start(start);

            if (!start.UseShellExecute) {
                Exec_Print(p.StandardOutput, false);
                Exec_Print(p.StandardError, true);
            }

            p.WaitForExit();
            p.Close();
        }

        private static void Exec_Print(StreamReader reader, bool isError) {
            if (reader == null)
                return;

            string str = reader.ReadToEnd();

            if (!string.IsNullOrEmpty(str)) {
                if (isError)
                    Debug.LogError(str);
                else
                    Debug.Log(str);
            }

            reader.Close();
        }


    }

}
