using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using VRC.SDK3.Data;

namespace Wacky612.PortalLibrarySystem2
{
    [CustomEditor(typeof(Categorys))]
    public class CategorysEditor : BaseEditor
    {
        private CategoryReorderableList _categoryReorderableList;
        
        public override async void OnInspectorGUI()
        {
            serializedObject.Update();

            _categoryReorderableList.DoLayoutList();
 
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
            _categoryReorderableList = new CategoryReorderableList((Categorys) target);
        }
    }

    public class CategoryReorderableList
    {
        private Categorys       _categorys;
        private List<Category>  _categoryList;
        private ReorderableList _categoryReorderableList;

        public CategoryReorderableList(Categorys categorys)
        {
            _categorys = categorys;
            GenerateCategoryList();
            GenerateCategoryReorderableList();
        }

        public void DoLayoutList()
        {
            _categoryReorderableList.DoLayoutList();
        }
        
        private void GenerateCategoryList()
        {
            _categoryList = new List<Category>();
            
            for (int i = 0; i < _categorys.transform.childCount; i++)
            {
                var category = _categorys.transform.GetChild(i).GetComponent<Category>();

                if (category != null) _categoryList.Add(category);
            }            
        }

        private void GenerateCategoryReorderableList()
        {
            _categoryReorderableList = new ReorderableList(_categoryList, typeof(Category),
                                                           true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "カテゴリ一覧");
                },
                onAddCallback = (list) =>
                {
                    _categoryList.Add(CategoryEditor.Create(_categorys, DataUtil.Null()));
                },
                onRemoveCallback = (list) =>
                {
                    var category = _categoryList[list.index];
                    _categoryList.RemoveAt(list.index);
                    GameObject.DestroyImmediate(category.gameObject);
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedObject so = new SerializedObject(_categoryList[index]);
                    SerializedProperty name = so.FindProperty("_categoryName");

                    Rect nameRect = rect;
                    nameRect.height = EditorGUIUtility.singleLineHeight;
                    
                    EditorGUI.PropertyField(nameRect, name, new GUIContent("カテゴリ名"));

                    so.ApplyModifiedProperties();
                    so.Update();
                },
                onReorderCallback = (ReorderableList list) =>
                {
                    for (int i = 0; i < list.list.Count; i++)
                    {
                        ((Category) list.list[i]).transform.SetSiblingIndex(i);
                    }
                }
            };
        }
    }
}
