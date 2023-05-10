using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.FlatBuffers;
using UnityEditor;
using UnityEngine;
using Config;

public class CfgRender<T, V> where T: IFlatbufferObject where V: IFlatbufferObject
{
    protected bool IsLoaded {
        get {
            return m_IndexFile != null && m_IsDataLoad && m_DataStream != null;
        }
    }

    private string m_SearchTxt;
    private FileStream m_DataStream = null;
    private V m_LastSearchItem;
    private IdMap m_IndexFile = null;
    private T m_Cfg;
    private bool m_IsSearched = false;
    private bool m_IsDataLoad = false;

    public void OnDestroy() {
        if (m_DataStream != null) {
            m_DataStream.Dispose();
            m_DataStream = null;
        }
        m_IsDataLoad = false;
        m_IndexFile = null;
        //m_MonsterCfg.__init(0, null);
        m_IsSearched = false;
    }

    void LoadIndexFile(string cfgName) {
        string fileName = string.Format("{0}_Id_proto", cfgName);
        var text = Resources.Load<TextAsset>(fileName);
        m_IndexFile = IdMap.Parser.ParseFrom(text.bytes);
    }

    void LoadDataStream(string cfgName, Func<ByteBuffer, T> GetRootAsMonsterCfg) {
        if (m_DataStream != null) {
            m_DataStream.Dispose();
        }
        m_DataStream = new FileStream(string.Format("Assets/Resources/{0}_flatbuffer.bytes", cfgName), FileMode.Open, FileAccess.Read);
        var byteBuffer = new ByteBuffer(new StreamReadBuffer(m_DataStream), m_IndexFile.DataFileOffset);
        m_Cfg = GetRootAsMonsterCfg(byteBuffer);
        m_IsDataLoad = true;
    }

    public bool Render(Func<int, T, V> GetItem, Action<V> OnRenderItem, Func<ByteBuffer, T> GetRootAsMonsterCfg, string cfgName) {
        bool ret = false;
        EditorGUILayout.Space();
        // GUILayout.Window(0, new Rect(0, 0, Screen.width, 2), null, string.Empty);
        EditorGUILayout.Space();
        if (IsLoaded) {
            m_SearchTxt = EditorGUILayout.TextField("搜索" + cfgName, m_SearchTxt);
            EditorGUILayout.Space();
            if (GUILayout.Button("流式搜索")) {
                uint id;
                if (uint.TryParse(m_SearchTxt, out id)) {
                    int idx;
                    if (m_IndexFile.IdToIdxMap.TryGetValue(id, out idx)) {
                        var m = GetItem(idx, m_Cfg);
                        if (m != null) {
                            m_LastSearchItem = m;
                            m_IsSearched = true;
                            ret = true;
                        }
                    }
                }
            }

            if (m_IsSearched) {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                OnRenderItem(m_LastSearchItem);
            }

        } else {
            if (GUILayout.Button("加载" + cfgName)) {
                LoadIndexFile(cfgName);
                LoadDataStream(cfgName, GetRootAsMonsterCfg);
            }
        }

        return ret;

    }
}

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    private CfgRender<MonsterCfg, Monster> m_MonsterRender = null;
    private CfgRender<WhiteListCfg, WhiteItem> m_WhiteListRender = null;

    private void Awake() {
        m_MonsterRender = new CfgRender<MonsterCfg, Monster>();
        m_WhiteListRender = new CfgRender<WhiteListCfg, WhiteItem>();
    }

    private void OnDestroy() {
        m_MonsterRender.OnDestroy();
        m_WhiteListRender.OnDestroy();
    }

    void RenderMonsterCfg() {
        m_MonsterRender.Render(
            (int idx, MonsterCfg cfg) =>
            {
                var m = cfg.Items(idx);
                if (m != null && m.HasValue)
                    return m.Value;
                return new Monster();
            },

            (Monster item) =>
            {
                EditorGUILayout.LabelField("id", item.Id.ToString());
                EditorGUILayout.LabelField("姓名", item.Name);
                EditorGUILayout.LabelField("对话", item.Npctalk);
                EditorGUILayout.LabelField("地图", string.Format("{0:D}({1:D}:{2:D})", item.Mapid, item.X, item.Y));
                EditorGUILayout.LabelField("模型编号", item.Model.ToString());
                EditorGUILayout.LabelField("图标", item.Icon.ToString());
                EditorGUILayout.LabelField("缩放", item.Scale.ToString());
            },

            (ByteBuffer buffer) =>
            {
                return MonsterCfg.GetRootAsMonsterCfg(buffer);
            },

            "MonsterCfg"
    );
    }

    void RenderWhiteListCfg() {
        m_WhiteListRender.Render(
            (int idx, WhiteListCfg cfg) =>
            {
                var m = cfg.Items(idx);
                if (m != null && m.HasValue)
                    return m.Value;
                return new WhiteItem();
            },

            (WhiteItem item) =>
            {
                EditorGUILayout.LabelField("索引", item.Index.ToString());
                EditorGUILayout.LabelField("组ID", item.Groupid.ToString());
                EditorGUILayout.LabelField("类型", item.Type.ToString());
                EditorGUILayout.LabelField("物品ID", item.Itemid.ToString());
                EditorGUILayout.LabelField("currencytype", item.Currencytype.ToString());
                EditorGUILayout.LabelField("bottomprice", item.Bottomprice.ToString());
            },

            (ByteBuffer buffer) =>
            {
                return WhiteListCfg.GetRootAsWhiteListCfg(buffer);
            },

            "whitelistcfg"
    );
    }

    override public void OnInspectorGUI() {
        base.DrawDefaultInspector();

        RenderMonsterCfg();
        RenderWhiteListCfg();
    }
}
