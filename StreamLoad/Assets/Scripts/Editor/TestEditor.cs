using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.FlatBuffers;
using UnityEditor;
using UnityEngine;
using Config;

[CustomEditor(typeof(Test))]
public class TestEditor: Editor
{
    private string m_SearchTxt;
    private static MonsterCfg m_MonsterCfg;
    private static bool m_IsDataLoad = false;
    private static IdMap m_IndexFile = null;
    private static FileStream m_DataStream = null;
    private Monster m_LastSearchMonster;
    private bool m_IsSearched = false;

    protected static bool IsLoaded {
        get {
            return m_IndexFile != null && m_IsDataLoad && m_DataStream != null;
        }
    }

    void LoadIndexFile() {
        var text = Resources.Load<TextAsset>("MonsterCfg_Id_proto");
        m_IndexFile = IdMap.Parser.ParseFrom(text.bytes);
    }

    private void OnDestroy() {
        if (m_DataStream != null) {
            m_DataStream.Dispose();
            m_DataStream = null;
        }
        m_IsDataLoad = false;
        m_IndexFile = null;
       // m_MonsterCfg.__init(0, null);
        m_IsSearched = false;
    }

    void LoadDataStream() {
        if (m_DataStream != null) {
            m_DataStream.Dispose();
        }
        m_DataStream = new FileStream("Assets/Resources/MonsterCfg_flatbuffer.bytes", FileMode.Open, FileAccess.Read);
        var byteBuffer = new ByteBuffer(new StreamReadBuffer(m_DataStream));
        m_MonsterCfg = MonsterCfg.GetRootAsMonsterCfg(byteBuffer);
        m_IsDataLoad = true;
    }

    override public void OnInspectorGUI() {
        base.DrawDefaultInspector();

        if (IsLoaded) {
            m_SearchTxt = EditorGUILayout.TextField("搜索ID", m_SearchTxt);
            if (GUILayout.Button("搜索")) {
                uint id;
                if (uint.TryParse(m_SearchTxt, out id)) {
                    int idx;
                    if (m_IndexFile.IdToIdxMap.TryGetValue(id, out idx)) {
                        var m = m_MonsterCfg.Items(idx);
                        if (m != null && m.HasValue) {
                            m_LastSearchMonster = m.Value;
                            m_IsSearched = true;
                            Repaint();
                        }
                    }
                }
            }

            if (m_IsSearched) {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("id", m_LastSearchMonster.Id.ToString());
                EditorGUILayout.LabelField("name", m_LastSearchMonster.Name);
            }

        } else {
            if (GUILayout.Button("加载索引文件")) {
                LoadIndexFile();
                LoadDataStream();
            }
        }
           
    }
}
