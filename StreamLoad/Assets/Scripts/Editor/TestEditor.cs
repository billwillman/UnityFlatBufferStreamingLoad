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
    private MonsterCfg m_MonsterCfg;
    private bool m_IsDataLoad = false;
    private IdMap m_IndexFile = null;
    private FileStream m_DataStream = null;
    private Monster m_LastSearchMonster;
    private bool m_IsSearched = false;

    protected bool IsLoaded {
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
        //m_MonsterCfg.__init(0, null);
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

        EditorGUILayout.Space();
       // GUILayout.Window(0, new Rect(0, 0, Screen.width, 2), null, string.Empty);
        EditorGUILayout.Space();

        if (IsLoaded) {
            m_SearchTxt = EditorGUILayout.TextField("搜索ID", m_SearchTxt);
            EditorGUILayout.Space();
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
                EditorGUILayout.LabelField("姓名", m_LastSearchMonster.Name);
                EditorGUILayout.LabelField("对话", m_LastSearchMonster.Npctalk);
                EditorGUILayout.LabelField("地图", string.Format("{0:D}({1:D}:{2:D})", m_LastSearchMonster.Mapid, m_LastSearchMonster.X, m_LastSearchMonster.Y));
                EditorGUILayout.LabelField("模型编号", m_LastSearchMonster.Model.ToString());
                EditorGUILayout.LabelField("图标", m_LastSearchMonster.Icon.ToString());
                EditorGUILayout.LabelField("缩放", m_LastSearchMonster.Scale.ToString());
            }

        } else {
            if (GUILayout.Button("加载索引文件")) {
                LoadIndexFile();
                LoadDataStream();
            }
        }
           
    }
}
