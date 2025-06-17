using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    public abstract class BaseWorldData : UdonSharpBehaviour
    {
        private protected WorldDataProcessor _worldDataProcessor;
    
        public abstract float WorldDataLoadDelaySeconds  { get; }
        public abstract float ThumbnailsLoadDelaySeconds { get; }

        public virtual void Initialize()
        {
            _worldDataProcessor = GetComponentInParent<WorldDataProcessor>();
        }

        public abstract void LoadWorldData();
        public abstract void LoadThumbnails();
        public abstract Texture GetThumbnail(int index);
    }
}
