using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using Google.FlatBuffers;
using Google.Protobuf;
using Config;

/*
 * Json to flatbuffer
 * https://blog.csdn.net/qq_32482645/article/details/126285684
 */

/*
 * ProtoBuf Map类型：
 * https://blog.csdn.net/qq23001186/article/details/125748720
 */

using IDMap = System.Collections.Generic.Dictionary<string, int>;

public class Test : MonoBehaviour
{
    private IDMap m_CfgKeyToIndexMap = null;
    private ByteBuffer m_DataBuffer = null;

    void LoadJsonIdMap() {
        if (m_CfgKeyToIndexMap != null)
            return;
        var textids = Resources.Load<TextAsset>("MonsterCfg_Id");
        m_CfgKeyToIndexMap = JsonMapper.ToObject<IDMap>(textids.text);
    }

    /// <summary>
    /// 可以复用ByteBuffer, 支持多个Stream只用一个ByteBuffer，具体去看flatbuffer的C#源代码
    /// </summary>
    /// <param name="stream">外部传Stream</param>
    /// <returns></returns>
    ByteBuffer GetByteBuffer(Stream stream) {
        // ByteBuffer构造函数的position,就支持把几个大配置内容flatbuffer二进制格式合并成一个大文件
        if (m_DataBuffer == null)
            m_DataBuffer = new ByteBuffer(new StreamReadBuffer(stream));
        else
            m_DataBuffer.ResetReadOnly(stream);
        return m_DataBuffer;
    }

#if UNITY_EDITOR

    void BuildIndexMapJson() {
        var cfg = LoadAllMonsterCfg();
        IDMap idMap = null;
        for (int i = 0; i < cfg.ItemsLength; ++i) {
            var monster = cfg.Items(i);
            if (monster != null && monster.HasValue) {
                if (idMap == null)
                    idMap = new IDMap();

                idMap.TryAdd(monster.Value.Id.ToString(), i);
            }
        }

        string json = string.Empty;
        if (idMap != null && idMap.Count > 0) {
            json = JsonMapper.ToJson(idMap);
        }

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
        FileStream stream = new FileStream("Assets/Resources/MonsterCfg_Id.json", FileMode.Create, FileAccess.Write);
        try {
            stream.Write(buffer);
        } finally {
            stream.Dispose();
        }
    }

    void BuildConfigIdMapFile() {
        BuildIndexMapJson();
        m_CfgKeyToIndexMap = null;
        LoadJsonIdMap();
        IdMap protoMsg = new IdMap();
        var iter = m_CfgKeyToIndexMap.GetEnumerator();
        try {
            while (iter.MoveNext()) {
                protoMsg.IdToIdxMap.Add(uint.Parse(iter.Current.Key), iter.Current.Value);
            }
        } finally {
            iter.Dispose();
        }
        var buffer = protoMsg.ToByteArray();
        FileStream stream = new FileStream("Assets/Resources/MonsterCfg_Id_proto.bytes", FileMode.Create, FileAccess.Write);
        stream.Write(buffer);
        stream.Dispose();
    }
#endif

    IdMap LoadIndexMap() {
        var text = Resources.Load<TextAsset>("MonsterCfg_Id_proto");
        var idMap = IdMap.Parser.ParseFrom(text.bytes);
        return idMap;
    }

    void TestConfigIdMap() {
        var idMap = LoadIndexMap();
        if (idMap.IdToIdxMap.Count > 0) {
            var iter = idMap.IdToIdxMap.GetEnumerator();
            try {
                while (iter.MoveNext()) {
                    Debug.LogFormat("id: {0:D} dataIndex: {1:D}", iter.Current.Key, iter.Current.Value);
                }
            } finally {
                iter.Dispose();
            }
        }
    }

#if UNITY_EDITOR
    void TestIndexFileData() {
        var idMap = LoadIndexMap();
        FileStream stream = new FileStream("Assets/Resources/MonsterCfg_flatbuffer.bytes", FileMode.Open, FileAccess.Read);
        try {
            var byteBuffer = GetByteBuffer(stream);
            MonsterCfg cfg = MonsterCfg.GetRootAsMonsterCfg(byteBuffer);
            var iter = idMap.IdToIdxMap.GetEnumerator();
            try {
                while (iter.MoveNext()) {
                    var id = iter.Current.Key;
                    var index = iter.Current.Value;
                    var data = cfg.Items(index);
                    if (data != null && data.HasValue) {
                        Debug.LogFormat("[Monster] id: {0:D} name: {1:D}", data.Value.Id, data.Value.Name);
                    }
                }
            } finally {
                iter.Dispose();
            }
        } finally {
            stream.Dispose();
        }
    }
#endif

    MonsterCfg LoadAllMonsterCfg() {
        var data = Resources.Load<TextAsset>("MonsterCfg_flatbuffer");
        MemoryStream stream = new MemoryStream(data.bytes);
        var byteBuffer = GetByteBuffer(stream);
        MonsterCfg cfg = MonsterCfg.GetRootAsMonsterCfg(byteBuffer);
        return cfg;
    }

    private void OnGUI() {
        if (GUI.Button(new Rect(100, 100, 200, 100), "Streaming加载")) {
            LoadJsonIdMap();
            var cfg = LoadAllMonsterCfg();
            int length = cfg.ItemsLength;
            for (int i = 0; i < length; ++i) {
                var monster = cfg.Items(i);
                if (monster != null && monster.HasValue) {
                    Debug.LogFormat("[{0:D}] id: {1:D} name: {2:D}", i, monster.Value.Id, monster.Value.Name);
                }
            }
          //  Debug.Log(cfg.ToString());
        }

#if UNITY_EDITOR
        var oldColor = GUI.color;
        GUI.color = Color.red;
        try {
            if (GUI.Button(new Rect(100, 100 + 100 + 10, 200, 100), "生成proto idmap")) {
                BuildConfigIdMapFile();
            }
        } finally {
            GUI.color = oldColor;
        }
#endif
        if (GUI.Button(new Rect(100 + 200 + 10, 100 + 100 + 10, 200, 100), "读取proto idmap")) {
            TestConfigIdMap();
        }
#if UNITY_EDITOR
        oldColor = GUI.color;
        try {
            GUI.color = Color.green;
            if (GUI.Button(new Rect(100, 200 + 100 + 20, 200, 100), "索引文件流读取数据")) {
                TestIndexFileData();
            }
        } finally {
            GUI.color = oldColor;
        }
#endif
    }
}
