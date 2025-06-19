using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using VRC.SDKBase.Editor.Api;

namespace Wacky612.PortalLibrarySystem2
{
    static public class VRCApiWrapper
    {
        static private string _thumbnailsFolder = "Assets/PortalLibrarySystem/Thumbnails";
        
        static public async Task UpdateWorldInformation(World world, bool updateDescription)
        {
            SerializedObject so = new SerializedObject(world);
            SerializedProperty id                  = so.FindProperty("_id");
            SerializedProperty name                = so.FindProperty("_name");
            SerializedProperty recommendedCapacity = so.FindProperty("_recommendedCapacity");
            SerializedProperty capacity            = so.FindProperty("_capacity");
            SerializedProperty releaseStatus       = so.FindProperty("_releaseStatus");
            SerializedProperty pc                  = so.FindProperty("_pc");
            SerializedProperty android             = so.FindProperty("_android");
            SerializedProperty ios                 = so.FindProperty("_ios");
            SerializedProperty description         = so.FindProperty("_description");
            SerializedProperty thumbnail           = so.FindProperty("_thumbnail");

            if (String.IsNullOrEmpty(id.stringValue)) return;
            
            try
            {
                var vrcWorld = await VRCApi.GetWorld(id.stringValue);

                name.stringValue             = vrcWorld.Name;
                recommendedCapacity.intValue = vrcWorld.RecommendedCapacity;
                capacity.intValue            = vrcWorld.Capacity;

                pc.boolValue      = false;
                android.boolValue = false;
                ios.boolValue     = false;

                foreach (var package in vrcWorld.UnityPackages)
                {
                    if (package.Platform == "standalonewindows") pc.boolValue      = true;
                    if (package.Platform == "android"          ) android.boolValue = true;
                    if (package.Platform == "ios"              ) ios.boolValue     = true;
                }

                switch (vrcWorld.ReleaseStatus)
                {
                    case "public":
                        releaseStatus.enumValueIndex = (int) ReleaseStatus.Public;
                        break;
                    case "private":
                        releaseStatus.enumValueIndex = (int) ReleaseStatus.Private;
                        break;
                }

                if (updateDescription) description.stringValue = vrcWorld.Description;

                CreateFolder(_thumbnailsFolder);

                var path = $"{_thumbnailsFolder}/{vrcWorld.ID}.png";
                File.WriteAllBytes(path, (await VRCApi.GetImage(vrcWorld.ThumbnailImageUrl)).EncodeToPNG());
                AssetDatabase.ImportAsset(path);

                var importer = (TextureImporter) AssetImporter.GetAtPath(path);
                importer.crunchedCompression = true;
                AssetDatabase.ImportAsset(path);

                thumbnail.objectReferenceValue = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            }
            catch (ApiErrorException e)
            {
                Debug.LogError("Exception when calling VRCApi.GetWorld: " + e.ErrorMessage);
            }

            so.ApplyModifiedProperties();
            so.Update();
        }

        static private void CreateFolder(string path)
        {
            var parent = path.Split('/')[0];
            
            foreach (var folder in path.Split('/')[1..])
            {
                if (!AssetDatabase.IsValidFolder($"{parent}/{folder}"))
                {
                    AssetDatabase.CreateFolder(parent, folder);
                }

                parent = $"{parent}/{folder}";
            }
        }

        static public async Task UpdateAllWorldInfomation(Component comp, bool updateDescription)
        {
            World[] worlds = comp.GetComponentsInChildren<World>();

            for (int i = 0; i < worlds.Length; i++)
            {
                float progress = (i + 1) / (float) worlds.Length;
                
                bool isCanceled = EditorUtility.DisplayCancelableProgressBar(
                    "ワールド情報の更新中",
                    $"{i + 1}／{worlds.Length} ({(int)(progress * 100)}％)",
                    progress
                );

                if (isCanceled) break;
                
                await VRCApiWrapper.UpdateWorldInformation(worlds[i], updateDescription);

                if (i != worlds.Length - 1) await Task.Delay(1000);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}
