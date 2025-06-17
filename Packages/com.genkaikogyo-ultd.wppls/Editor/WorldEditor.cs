using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using VRC.SDK3.Data;

namespace Wacky612.PortalLibrarySystem2
{
    [CustomEditor(typeof(World))]
    public class WorldEditor : BaseEditor
    {
        private ReorderableList _permittedRolesReorderableList;
        
        public override async void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(FindProperty("_id"),                  new GUIContent("ワールドID"));
            EditorGUILayout.PropertyField(FindProperty("_name"),                new GUIContent("ワールド名"));
            EditorGUILayout.PropertyField(FindProperty("_recommendedCapacity"), new GUIContent("推奨人数"));
            EditorGUILayout.PropertyField(FindProperty("_capacity"),            new GUIContent("最大人数"));
            EditorGUILayout.PropertyField(FindProperty("_releaseStatus"),       new GUIContent("公開状態"));
            EditorGUILayout.PropertyField(FindProperty("_pc"),                  new GUIContent("PC"));
            EditorGUILayout.PropertyField(FindProperty("_android"),             new GUIContent("Android"));
            EditorGUILayout.PropertyField(FindProperty("_ios"),                 new GUIContent("iOS"));
            EditorGUILayout.PropertyField(FindProperty("_description"),         new GUIContent("Description"));
            EditorGUILayout.PropertyField(FindProperty("_thumbnail"),           new GUIContent("サムネイル画像"));
            EditorGUILayout.Space();
            _permittedRolesReorderableList.DoLayoutList();

            BaseEditor.DrawSpacer();

            if (GUILayout.Button("ワールドIDからワールドの情報を取得"))
            {
                await VRCApiWrapper.UpdateWorldInformation((World) target, true);
                return;
            }

            if (GUILayout.Button("ワールドIDからワールドの情報を取得（Descriptionを除く）"))
            {
                await VRCApiWrapper.UpdateWorldInformation((World) target, false);
                return;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            GeneratePermittedRolesReorderableList();
        }

        private void GeneratePermittedRolesReorderableList()
        {
            _permittedRolesReorderableList = new ReorderableList(serializedObject,
                                                                 FindProperty("_permittedRoles"),
                                                                 true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "ワールドの表示を許可するロール");
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

        static public World Create(Category parent, DataToken d)
        {
            var world = new GameObject("World", typeof(World)).GetComponent<World>();
            world.transform.SetParent(parent.transform);
            world.transform.localPosition = Vector3.zero;
            world.transform.localRotation = Quaternion.identity;

            var so = new SerializedObject(world);
            so.FindProperty("_id").stringValue               = DataUtil.ForceValueString(d, "ID");
            so.FindProperty("_name").stringValue             = DataUtil.ForceValueString(d, "Name");
            so.FindProperty("_recommendedCapacity").intValue = DataUtil.ForceValueInt(d, "RecommendedCapacity");
            so.FindProperty("_capacity").intValue            = DataUtil.ForceValueInt(d, "Capacity");
            so.FindProperty("_description").stringValue      = DataUtil.ForceValueString(d, "Description");
            
            so.FindProperty("_pc").boolValue      = DataUtil.ForceValueBool(DataUtil.ForceValue(d, "Platform"),
                                                                            "PC");
            so.FindProperty("_android").boolValue = DataUtil.ForceValueBool(DataUtil.ForceValue(d, "Platform"),
                                                                            "Android");
            so.FindProperty("_ios").boolValue     = DataUtil.ForceValueBool(DataUtil.ForceValue(d, "Platform"),
                                                                            "iOS");

            if (DataUtil.ForceValueString(d, "ReleaseStatus") == "private")
            {
                so.FindProperty("_releaseStatus").enumValueIndex = (int) ReleaseStatus.Private;
            }
            else
            {
                so.FindProperty("_releaseStatus").enumValueIndex = (int) ReleaseStatus.Public;
            }

            if (DataUtil.ForceValue(d, "PermittedRoles").TokenType == TokenType.DataList)
            {
                var roles = so.FindProperty("_permittedRoles");
                roles.arraySize = DataUtil.ForceValue(d, "PermittedRoles").DataList.Count;
                so.ApplyModifiedProperties();

                for (int i = 0; i < roles.arraySize; i++)
                {
                    var role = roles.GetArrayElementAtIndex(i);
                    role.stringValue = DataUtil.ForceValue(d, "PermittedRoles").DataList[i].String;
                }
            }

            so.ApplyModifiedProperties();
            so.Update();

            return world;
        }
    }
}
