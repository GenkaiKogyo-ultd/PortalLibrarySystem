using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using VRC.SDK3.Video.Components;

namespace Wacky612.PortalLibrarySystem2
{
    public class PortalLibrarySystem2BuildProcessor : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            foreach (var pls in Object.FindObjectsOfType<PortalLibrarySystem2>(true))
            {
                var jsonWorldData = new SerializedObject(pls.GetComponentInChildren<JsonWorldData>());
                var videoPlayer   = new SerializedObject(pls.GetComponentInChildren<VRCUnityVideoPlayer>());

                var oldRenderTexture = jsonWorldData.FindProperty("_renderTexture").objectReferenceValue;
                var newRenderTexture = new RenderTexture((RenderTexture) oldRenderTexture);

                jsonWorldData.FindProperty("_renderTexture").objectReferenceValue = newRenderTexture;
                videoPlayer.FindProperty("targetTexture").objectReferenceValue = newRenderTexture;

                jsonWorldData.ApplyModifiedProperties();
                videoPlayer.ApplyModifiedProperties();
            }
        }
    }
}
