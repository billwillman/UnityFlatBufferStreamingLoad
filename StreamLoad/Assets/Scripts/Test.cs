using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using Google.FlatBuffers;
using Config;

/*
 * Json to flatbuffer
 * https://blog.csdn.net/qq_32482645/article/details/126285684
 */

using IDMap = System.Collections.Generic.Dictionary<string, int>;

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
        var textids = Resources.Load<TextAsset>("MonsterCfg_Id");
        m_CfgKeyToIndexMap = JsonMapper.ToObject<IDMap>(textids.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ByteBuffer GetByteBuffer(Stream stream) {
        if (m_DataBuffer == null)
            m_DataBuffer = new ByteBuffer(new StreamReadBuffer(stream));
        else
            m_DataBuffer.ResetReadOnly(stream);
        return m_DataBuffer;
    }

    private void OnGUI() {
        if (GUI.Button(new Rect(100, 100, 200, 100), "Streamingº”‘ÿ")) {
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
    }
}
