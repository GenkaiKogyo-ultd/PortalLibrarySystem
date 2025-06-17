using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Wacky612.PortalLibrarySystem2
{
    [CustomEditor(typeof(Roles))]
    public class RolesEditor : BaseEditor
    {
        private RoleReorderableList _roleReorderableList;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _roleReorderableList.DoLayoutList();
 
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            _roleReorderableList = new RoleReorderableList((Roles) target);
        }
    }

    public class RoleReorderableList
    {
        private Roles           _roles;
        private List<Role>      _roleList;
        private ReorderableList _roleReorderableList;

        public RoleReorderableList(Roles roles)
        {
            _roles = roles;
            GenerateRoleList();
            GenerateRoleReorderableList();
        }

        public void DoLayoutList()
        {
            _roleReorderableList.DoLayoutList();
        }

        private void GenerateRoleList()
        {
            _roleList = new List<Role>();
            
            for (int i = 0; i < _roles.transform.childCount; i++)
            {
                var role = _roles.transform.GetChild(i).GetComponent<Role>();

                if (role != null) _roleList.Add(role);
            }            
        }

        private void GenerateRoleReorderableList()
        {
            _roleReorderableList = new ReorderableList(_roleList, typeof(Role),
                                                       true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "ロール一覧");
                },
                onAddCallback = (list) =>
                {
                    _roleList.Add(RoleEditor.Create(_roles, DataUtil.Null()));
                },
                onRemoveCallback = (list) =>
                {
                    var role = _roleList[list.index];
                    _roleList.RemoveAt(list.index);
                    GameObject.DestroyImmediate(role.gameObject);
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedObject so = new SerializedObject(_roleList[index]);
                    SerializedProperty name = so.FindProperty("_roleName");

                    Rect nameRect = rect;
                    nameRect.height = EditorGUIUtility.singleLineHeight;
                    
                    EditorGUI.PropertyField(nameRect, name, new GUIContent("ロール名"));

                    so.ApplyModifiedProperties();
                    so.Update();
                },
                onReorderCallback = (ReorderableList list) =>
                {
                    for (int i = 0; i < list.list.Count; i++)
                    {
                        ((Role) list.list[i]).transform.SetSiblingIndex(i);
                    }
                }
            };
        }
    }
}
