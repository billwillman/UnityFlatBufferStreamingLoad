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
 * ProtoBuf Map���ͣ�
 * https://blog.csdn.net/qq23001186/article/details/125748720
 */

using IDMap = System.Collections.Generic.Dictionary<string, uint>;

public class Test : MonoBehaviour
{
    private IDMap m_CfgKeyToIndexMap = null;
    private ByteBuffer m_DataBuffer = null;
    // Start is called before the first frame update
    void Start()
    {
        LoadJsonIdMap();
    }

    void LoadJsonIdMap() {
        if (m_CfgKeyToIndexMap != null)
            return;
        var textids = Resources.Load<TextAsset>("MonsterCfg_Id");
        m_CfgKeyToIndexMap = JsonMapper.ToObject<IDMap>(textids.text);
    }

    /// <summary>
    /// ���Ը���ByteBuffer, ֧�ֶ��Streamֻ��һ��ByteBuffer������ȥ��flatbuffer��C#Դ����
    /// </summary>
    /// <param name="stream">�ⲿ��Stream</param>
    /// <returns></returns>
    ByteBuffer GetByteBuffer(Stream stream) {
        // ByteBuffer���캯����position,��֧�ְѼ�������������flatbuffer�����Ƹ�ʽ�ϲ���һ�����ļ�
        if (m_DataBuffer == null)
            m_DataBuffer = new ByteBuffer(new StreamReadBuffer(stream));
        else
            m_DataBuffer.ResetReadOnly(stream);
        return m_DataBuffer;
    }

    void LoadConfigIdMap() {
       //IdMap.Parser.l
    }

#if UNITY_EDITOR
    void BuildConfigIdMapFile() {
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
        FileStream stream = new FileStream("Assets/Resources/MonsterCfg_Id.bytes", FileMode.Create, FileAccess.Write);
        stream.Write(buffer);
        stream.Dispose();
    }
#endif


    private void OnGUI() {
        if (GUI.Button(new Rect(100, 100, 200, 100), "Streaming����")) {
            var data = Resources.Load<TextAsset>("MonsterCfg");
            MemoryStream stream = new MemoryStream(data.bytes);
            var byteBuffer = GetByteBuffer(stream);
            MonsterCfg cfg = MonsterCfg.GetRootAsMonsterCfg(byteBuffer);
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
        if (GUI.Button(new Rect(100, 100 + 100 + 10, 200, 100), "����proto idmap")) {
            BuildConfigIdMapFile();
        }
#endif

    }
}
