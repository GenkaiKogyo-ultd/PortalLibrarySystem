using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wacky612.PortalLibrarySystem2
{
    [CustomEditor(typeof(PortalLibrarySystem2))]
    public class PortalLibrarySystem2Editor : BaseEditor
    {
        private WorldDataProcessorEditorContent _worldDataProcessorEditorContent;
        
        public override async void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(FindProperty("_portalManager"), new GUIContent("PortalManager"));
            
            BaseEditor.DrawSpacer();
            
            await _worldDataProcessorEditorContent.Draw();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            var worldDataProcessor = ((Component) target).GetComponentInChildren<WorldDataProcessor>();
            _worldDataProcessorEditorContent = new WorldDataProcessorEditorContent(worldDataProcessor);
        }
    }
}
