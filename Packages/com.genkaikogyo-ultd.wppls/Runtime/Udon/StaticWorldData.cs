using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class StaticWorldData : BaseWorldData
    {
        [SerializeField] private bool _reverseCategorys = false;
        [SerializeField] private bool _showPrivateWorld = false;

        private Texture2D[] _thumbnails;

        public override float WorldDataLoadDelaySeconds  { get { return 0; } }
        public override float ThumbnailsLoadDelaySeconds { get { return 0; } }
        
        public override void LoadWorldData()
        {
            _worldDataProcessor.OnWorldDataLoadSuccess(ConstructWorldData());
        }

        public override void LoadThumbnails()
        {
            ConstructThumbnails();
            _worldDataProcessor.OnThumbnailsLoadFinish();
        }

        public DataToken ConstructWorldData()
        {
            DataToken data = new DataToken(new DataDictionary());          
            data.DataDictionary["ReverseCategorys"] = _reverseCategorys;
            data.DataDictionary["ShowPrivateWorld"] = _showPrivateWorld;
            data.DataDictionary["Categorys"]        = GetComponentInChildren<Categorys>().GetDataToken();
            data.DataDictionary["Roles"]            = GetComponentInChildren<Roles>().GetDataToken();

            return data;
        }

        private void ConstructThumbnails()
        {
            var worlds  = GetComponentsInChildren<World>();
            _thumbnails = new Texture2D[worlds.Length];

            for (int i = 0; i < worlds.Length; i++)
            {
                _thumbnails[i] = worlds[i].Thumbnail;
            }
        }

        public override Texture GetThumbnail(int index)
        {
            return (Texture) _thumbnails[index];
        }
    }
}
