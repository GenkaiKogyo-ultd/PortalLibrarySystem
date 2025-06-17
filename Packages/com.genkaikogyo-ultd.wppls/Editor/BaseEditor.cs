using UnityEngine;
using UnityEditor;

namespace Wacky612.PortalLibrarySystem2
{
    public class BaseEditor : Editor
    {
        private protected SerializedProperty FindProperty(string propertyPath)
        {
            return serializedObject.FindProperty(propertyPath);
        }

        static public void DrawSpacer()
        {
            EditorGUILayout.Space();
            GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
        }
    }
}
