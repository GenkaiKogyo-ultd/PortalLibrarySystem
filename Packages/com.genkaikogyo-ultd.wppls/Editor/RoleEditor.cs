using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using VRC.SDK3.Data;

namespace Wacky612.PortalLibrarySystem2
{
    [CustomEditor(typeof(Role))]
    public class RoleEditor : BaseEditor
    {
        private SerializedProperty _roleName;
        private ReorderableList    _displayNamesReorderableList;

        public override async void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(FindProperty("_roleName"), new GUIContent("ロール名"));
            EditorGUILayout.Space();
            _displayNamesReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            GenerateDisplayNamesReorderableList();
        }

        private void GenerateDisplayNamesReorderableList()
        {
            _displayNamesReorderableList = new ReorderableList(serializedObject,
                                                               FindProperty("_displayNames"),
                                                               true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "ユーザー名（DisplayName）一覧");
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var elementProperty = FindProperty("_displayNames").GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, elementProperty, new GUIContent("ユーザー名"));
                }
            };
        }

        static public Role Create(Roles parent, DataToken data)
        {
            var role = new GameObject("Role", typeof(Role)).GetComponent<Role>();
            role.transform.SetParent(parent.transform);
            role.transform.localPosition = Vector3.zero;
            role.transform.localRotation = Quaternion.identity;

            var so = new SerializedObject(role);
            so.FindProperty("_roleName").stringValue = DataUtil.ForceValueString(data, "RoleName");

            if (DataUtil.ForceValue(data, "DisplayNames").TokenType == TokenType.DataList)
            {
                var names = so.FindProperty("_displayNames");
                names.arraySize = DataUtil.ForceValue(data, "DisplayNames").DataList.Count;
                so.ApplyModifiedProperties();

                for (int i = 0; i < names.arraySize; i++)
                {
                    var name = names.GetArrayElementAtIndex(i);
                    name.stringValue = DataUtil.ForceValue(data, "DisplayNames").DataList[i].String;
                }
            }

            so.ApplyModifiedProperties();
            so.Update();

            return role;
        }
    }
}
