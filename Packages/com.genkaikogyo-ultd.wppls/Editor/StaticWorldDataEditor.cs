using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Data;

namespace Wacky612.PortalLibrarySystem2
{
    [CustomEditor(typeof(StaticWorldData))]
    public class StaticWorldDataEditor : BaseEditor
    {
        private StaticWorldDataEditorContent _staticWorldDataEditorContent;

        public override async void OnInspectorGUI()
        {
            await _staticWorldDataEditorContent.Draw();
        }

        private void OnEnable()
        {
            _staticWorldDataEditorContent = new StaticWorldDataEditorContent((StaticWorldData) target);
        }

        static public void ExportAsJson(StaticWorldData staticWorldData)
        {
            DataToken data = staticWorldData.ConstructWorldData();

            if (VRCJson.TrySerializeToJson(data, JsonExportType.Beautify, out DataToken json))
            {
                var path = EditorUtility.SaveFilePanel("JSONとしてExport", "", "data", "json");

                if (path.Length != 0)
                {
                    File.WriteAllText(path, json.String);
                }            
            } 
        }

        static public void ImportFromJson(StaticWorldData staticWorldData)
        {
            var path = EditorUtility.OpenFilePanel("JSONからImport", "", "json");
            
            if (path.Length != 0 && VRCJson.TryDeserializeFromJson(File.ReadAllText(path), out DataToken d))
            {
                var so = new SerializedObject(staticWorldData);
                so.FindProperty("_reverseCategorys").boolValue = DataUtil.ForceValueBool(d, "ReverseCategorys");
                so.FindProperty("_showPrivateWorld").boolValue = DataUtil.ForceValueBool(d, "ShowPrivateWorld");

                if (DataUtil.ForceValue(d, "Categorys").TokenType == TokenType.DataList)
                {
                    for (int i = 0; i < DataUtil.ForceValue(d, "Categorys").DataList.Count; i++)
                    {
                        CategoryEditor.Create(staticWorldData.GetComponentInChildren<Categorys>(),
                                              DataUtil.ForceValue(d, "Categorys").DataList[i]);
                    }
                }

                if (DataUtil.ForceValue(d, "Roles").TokenType == TokenType.DataList)
                {
                    for (int i = 0; i < DataUtil.ForceValue(d, "Roles").DataList.Count; i++)
                    {
                        RoleEditor.Create(staticWorldData.GetComponentInChildren<Roles>(),
                                          DataUtil.ForceValue(d, "Roles").DataList[i]);
                    }
                }
            }
        }
    }

    public class StaticWorldDataEditorContent
    {
        private StaticWorldData         _staticWorldData;
        private SerializedObject        _serializedObject;
        private CategoryReorderableList _categoryReorderableList;
        private RoleReorderableList     _roleReorderableList;

        public StaticWorldDataEditorContent(StaticWorldData staticWorldData)
        {
            var categorys = staticWorldData.GetComponentInChildren<Categorys>();
            var roles     = staticWorldData.GetComponentInChildren<Roles>();
            
            _staticWorldData         = staticWorldData;
            _serializedObject        = new SerializedObject(staticWorldData);
            _categoryReorderableList = new CategoryReorderableList(categorys);
            _roleReorderableList     = new RoleReorderableList(roles);
        }

        public async Task Draw()
        {
            _serializedObject.Update();
            
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_reverseCategorys"),
                                          new GUIContent("カテゴリーの順を逆順に"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_showPrivateWorld"),
                                          new GUIContent("プライベートワールドを表示"));

            EditorGUILayout.Space();
            
            _categoryReorderableList.DoLayoutList();
            _roleReorderableList.DoLayoutList();

            BaseEditor.DrawSpacer();

            if (GUILayout.Button("ワールドの情報を全て更新"))
            {
                await VRCApiWrapper.UpdateAllWorldInfomation(_staticWorldData, true);
                return;
            }

            if (GUILayout.Button("ワールドの情報を全て更新（Descriptionを除く）"))
            {
                await VRCApiWrapper.UpdateAllWorldInfomation(_staticWorldData, false);
                return;
            }

            if (GUILayout.Button("JSONとしてエクスポート"))
            {
                EditorApplication.delayCall += () => StaticWorldDataEditor.ExportAsJson(_staticWorldData);
            }

            if (GUILayout.Button("JSONからインポート"))
            {
                EditorApplication.delayCall += () => StaticWorldDataEditor.ImportFromJson(_staticWorldData);
            }

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
