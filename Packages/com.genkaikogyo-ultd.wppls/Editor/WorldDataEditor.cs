using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wacky612.PortalLibrarySystem2
{
    [CustomEditor(typeof(WorldDataProcessor))]
    public class WorldDataProcessorEditor : BaseEditor
    {
        private WorldDataProcessorEditorContent _worldDataProcessorEditorContent;
        
        public override async void OnInspectorGUI()
        {
            await _worldDataProcessorEditorContent.Draw();
        }

        private void OnEnable()
        {
            _worldDataProcessorEditorContent = new WorldDataProcessorEditorContent((WorldDataProcessor) target);
        }
    }

    public class WorldDataProcessorEditorContent
    {
        private WorldDataProcessor           _worldDataProcessor;
        private SerializedObject             _serializedObject;
        private StaticWorldDataEditorContent _staticWorldDataEditorContent;

        public WorldDataProcessorEditorContent(WorldDataProcessor worldDataProcessor)
        {
            var staticWorldData = worldDataProcessor.GetComponentInChildren<StaticWorldData>();
            
            _worldDataProcessor           = worldDataProcessor;
            _serializedObject             = new SerializedObject(worldDataProcessor);
            _staticWorldDataEditorContent = new StaticWorldDataEditorContent(staticWorldData);
        }
        
        public async Task Draw()
        {
            _serializedObject.Update();

            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_dataType"),
                                          new GUIContent("動作モード"));
            
            BaseEditor.DrawSpacer();

            switch ((DataType) _serializedObject.FindProperty("_dataType").enumValueIndex)
            {
                case DataType.Static:
                    await _staticWorldDataEditorContent.Draw();
                    break;
                case DataType.Json:
                    DrawJsonPropertyField();
                    break;
            }

            _serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawJsonPropertyField()
        {
            var jsonWorldData = _worldDataProcessor.transform.GetComponentInChildren<JsonWorldData>();
            var so = new SerializedObject(jsonWorldData);

            EditorGUILayout.PropertyField(so.FindProperty("_jsonUrl"),
                                          new GUIContent("JsonデータのURL"));
            EditorGUILayout.PropertyField(so.FindProperty("_thumbnailUrl"),
                                          new GUIContent("サムネイル動画データのURL"));
            EditorGUILayout.PropertyField(so.FindProperty("_worldDataLoadDelaySeconds"),
                                          new GUIContent("Jsonデータのロード遅延"));
            EditorGUILayout.PropertyField(so.FindProperty("_thumbnailsLoadDelaySeconds"),
                                          new GUIContent("サムネイル動画データのロード遅延"));
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
}
