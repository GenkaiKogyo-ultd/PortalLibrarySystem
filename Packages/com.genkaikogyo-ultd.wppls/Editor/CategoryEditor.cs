using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using VRC.SDK3.Data;

namespace Wacky612.PortalLibrarySystem2
{
    [CustomEditor(typeof(Category))]
    public class CategoryEditor : BaseEditor
    {
        private SerializedProperty _categoryName;
        private ReorderableList    _permittedRolesReorderableList;
        private ReorderableList    _worldReorderableList;
        private List<World>        _worldList;

        public override async void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(FindProperty("_categoryName"), new GUIContent("カテゴリー名"));
            EditorGUILayout.Space();
            _worldReorderableList.DoLayoutList();
            EditorGUILayout.Space();
            _permittedRolesReorderableList.DoLayoutList();
            
            BaseEditor.DrawSpacer();

            if (GUILayout.Button("ワールドの情報を全て更新"))
            {
                await VRCApiWrapper.UpdateAllWorldInfomation((Component) target, true);
                return;
            }

            if (GUILayout.Button("ワールドの情報を全て更新（Descriptionを除く）"))
            {
                await VRCApiWrapper.UpdateAllWorldInfomation((Component) target, false);
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            GenerateWorldList();
            GenerateWorldReorderableList();
            GeneratePermittedRolesReorderableList();
        }

        private void GenerateWorldList()
        {
            _worldList = new List<World>();
            
            var category = ((Component) target).transform;

            for (int i = 0; i < category.childCount; i++)
            {
                var world = category.GetChild(i).GetComponent<World>();

                if (world != null) _worldList.Add(world);
            }            
        }

        private void GenerateWorldReorderableList()
        {
            _worldReorderableList = new ReorderableList(_worldList, typeof(World),
                                                        true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "ワールド一覧");
                },
                onAddCallback = (list) =>
                {
                    _worldList.Add(WorldEditor.Create(((Category) target), DataUtil.Null()));
                },
                onRemoveCallback = (list) =>
                {
                    var world = _worldList[list.index];
                    _worldList.RemoveAt(list.index);
                    GameObject.DestroyImmediate(world.gameObject);
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedObject so = new SerializedObject(_worldList[index]);
                    SerializedProperty id   = so.FindProperty("_id");
                    SerializedProperty name = so.FindProperty("_name");

                    Rect idRect   = rect;
                    Rect nameRect = rect;
                    
                    idRect.height   = EditorGUIUtility.singleLineHeight;
                    nameRect.height = EditorGUIUtility.singleLineHeight;
                    nameRect.y     += (EditorGUIUtility.singleLineHeight +
                                       EditorGUIUtility.standardVerticalSpacing);
                    
                    EditorGUI.PropertyField(idRect,   id,   new GUIContent("ワールドID"));
                    EditorGUI.PropertyField(nameRect, name, new GUIContent("ワールド名"));

                    so.ApplyModifiedProperties();
                    so.Update();
                },
                onReorderCallback = (ReorderableList list) =>
                {
                    for (int i = 0; i < list.list.Count; i++)
                    {
                        ((World) list.list[i]).transform.SetSiblingIndex(i);
                    }
                },
                elementHeight = (EditorGUIUtility.singleLineHeight +
                                 EditorGUIUtility.standardVerticalSpacing) * 2
            };
        }

        private void GeneratePermittedRolesReorderableList()
        {
            _permittedRolesReorderableList = new ReorderableList(serializedObject,
                                                                 FindProperty("_permittedRoles"),
                                                                 true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "カテゴリの表示を許可するロール");
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var elementProperty = FindProperty("_permittedRoles").GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, elementProperty, new GUIContent("ロール名"));
                },
                drawNoneElementCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "全員に表示を許可");
                }
            };
        }

        static public Category Create(Categorys parent, DataToken data)
        {
            var category = new GameObject("Category", typeof(Category)).GetComponent<Category>();
            category.transform.SetParent(parent.transform);
            category.transform.localPosition = Vector3.zero;
            category.transform.localRotation = Quaternion.identity;

            var so = new SerializedObject(category);
            so.FindProperty("_categoryName").stringValue = DataUtil.ForceValueString(data, "Category");

            if (DataUtil.ForceValue(data, "Worlds").TokenType == TokenType.DataList)
            {
                for (int i = 0; i < DataUtil.ForceValue(data, "Worlds").DataList.Count; i++)
                {
                    WorldEditor.Create(category, DataUtil.ForceValue(data, "Worlds").DataList[i]);
                }
            }

            if (DataUtil.ForceValue(data, "PermittedRoles").TokenType == TokenType.DataList)
            {
                var roles = so.FindProperty("_permittedRoles");
                roles.arraySize = DataUtil.ForceValue(data, "PermittedRoles").DataList.Count;
                so.ApplyModifiedProperties();

                for (int i = 0; i < roles.arraySize; i++)
                {
                    var role = roles.GetArrayElementAtIndex(i);
                    role.stringValue = DataUtil.ForceValue(data, "PermittedRoles").DataList[i].String;
                }
            }

            so.ApplyModifiedProperties();
            so.Update();

            return category;

        }
    }
}
