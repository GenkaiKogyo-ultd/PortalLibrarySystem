using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class WorldDataProcessor : UdonSharpBehaviour
    {
        [SerializeField] private DataType _dataType;

        private PortalLibrarySystem2 _pls;
        private BaseWorldData        _worldData;

        private DataList _obtainedRoles;
        private bool     _showPrivateWorld;
        private bool     _reverseCategorys;
        private bool     _isLoadingWorldData;
        private bool     _isLoadingThumbnails;

        private DataToken _categorys;        
        public  DataToken Categorys { get { return _categorys; } }

        public DataType DataType                   { get { return _dataType; } }
        public float    WorldDataLoadDelaySeconds  { get { return _worldData.WorldDataLoadDelaySeconds; } }
        public float    ThumbnailsLoadDelaySeconds { get { return _worldData.ThumbnailsLoadDelaySeconds; } }

        public void Initialize()
        {
            _pls       = GetComponentInParent<PortalLibrarySystem2>();
            _isLoadingWorldData  = false;
            _isLoadingThumbnails = false;

            switch (_dataType)
            {
                case DataType.Static:
                    _worldData = (BaseWorldData) GetComponentInChildren<StaticWorldData>();
                    break;
                case DataType.Json:
                    _worldData = (BaseWorldData) GetComponentInChildren<JsonWorldData>();
                    break;
            }

            _worldData.Initialize();
        }
        
        public void LoadWorldData()
        {
            if (!_isLoadingWorldData)
            {
                _isLoadingWorldData = true;
                _worldData.LoadWorldData();
            }
        }

        public void LoadThumbnails()
        {
            if (!_isLoadingThumbnails)
            {
                _isLoadingThumbnails = true;
                _worldData.LoadThumbnails();
            }
        }

        public void OnThumbnailsLoadFinish()
        {
            _isLoadingThumbnails = false;
        }

        public Texture GetThumbnail(int index)
        {
            return _worldData.GetThumbnail(index);
        }
        
        public void OnWorldDataLoadSuccess(DataToken data)
        {
            _categorys        = data.DataDictionary["Categorys"];
            _obtainedRoles    = GetObtainedRoles(data);
            _showPrivateWorld = DataUtil.ForceValueBool(data, "ShowPrivateWorld");
            _reverseCategorys = DataUtil.ForceValueBool(data, "ReverseCategorys");

            SendCustomEventDelayedFrames(nameof(_AppendThumbnailIndex), 1);
            SendCustomEventDelayedFrames(nameof(_ApplyPermission),      2);
            SendCustomEventDelayedFrames(nameof(_HidePrivateWorld),     3);
            SendCustomEventDelayedFrames(nameof(_ReturnToPLS),          4);
        }

        public void OnWorldDataLoadFailed()
        {
            _isLoadingWorldData = false;
        }

        public void _AppendThumbnailIndex()
        {
            int i = 0;
            
            for (int c = 0; c < _categorys.DataList.Count; c++)
            {
                for (int w = 0; w < GetCategory(c).DataDictionary["Worlds"].DataList.Count; w++)
                {
                    GetWorld(c, w).DataDictionary["ThumbnailIndex"] = new DataToken(i);
                    i++;
                }
            }
        }

        public void _ApplyPermission()
        {
            for (int c = 0; c < _categorys.DataList.Count; c++)
            {
                var forbiddenWorldIndexes = GetForbiddenWorldIndexes(c, _obtainedRoles);
                forbiddenWorldIndexes.Reverse();

                for (int i = 0; i < forbiddenWorldIndexes.Count; i++)
                {
                    GetCategory(c).DataDictionary["Worlds"].DataList.RemoveAt(forbiddenWorldIndexes[i].Int);
                }
            }

            var forbiddenCategoryIndexes = GetForbiddenCategoryIndexes(_obtainedRoles);
            forbiddenCategoryIndexes.Reverse();
            
            for (int i = 0; i < forbiddenCategoryIndexes.Count; i++)
            {
                _categorys.DataList.RemoveAt(forbiddenCategoryIndexes[i].Int);
            }
        }
        
        public void _HidePrivateWorld()
        {
            if (!_showPrivateWorld)
            {
                for (int c = 0; c < _categorys.DataList.Count; c++)
                {
                    for (int w = 0; w < GetCategory(c).DataDictionary["Worlds"].DataList.Count; w++)
                    {
                        var releaseStatus = DataUtil.ForceValueString(GetWorld(c, w), "ReleaseStatus");

                        if (releaseStatus == "private") GetWorld(c, w).DataDictionary["ID"] = new DataToken("");
                    }
                }
            }
        }

        public void _ReturnToPLS()
        {
            if (_reverseCategorys) _categorys.DataList.Reverse();
            _isLoadingWorldData = false;
            _pls.OnWorldDataLoaded();
        }

        private DataList GetObtainedRoles(DataToken data)
        {
            var obtainedRoles = new DataList();
            
            if (data.DataDictionary.ContainsKey("Roles"))
            {
                for (int i = 0; i < data.DataDictionary["Roles"].DataList.Count; i++)
                {
                    var role = data.DataDictionary["Roles"].DataList[i].DataDictionary;
                    
                    if (role["DisplayNames"].DataList.Contains(Networking.LocalPlayer.displayName))
                    {
                        obtainedRoles.Add(role["RoleName"]);
                    }
                }
            }

            return obtainedRoles;
        }

        private DataList GetForbiddenCategoryIndexes(DataList obtainedRoles)
        {
            var forbiddenCategoryIndexes = new DataList();
            
            for (int c = 0; c < _categorys.DataList.Count; c++)
            {
                if (GetCategory(c).DataDictionary.ContainsKey("PermittedRoles"))
                {
                    var categoryPermittedRoles = GetCategory(c).DataDictionary["PermittedRoles"].DataList;
                    
                    if (DataUtil.Intersection(obtainedRoles, categoryPermittedRoles).Count == 0)
                    {
                        forbiddenCategoryIndexes.Add(c);
                    }
                }
            }

            return forbiddenCategoryIndexes;
        }
        
        private DataList GetForbiddenWorldIndexes(int c, DataList obtainedRoles)
        {
            var forbiddenWorldIndexes = new DataList();

            for (int w = 0; w < GetCategory(c).DataDictionary["Worlds"].DataList.Count; w++)
            {
                if (GetWorld(c, w).DataDictionary.ContainsKey("PermittedRoles"))
                {
                    var worldPermittedRoles = GetWorld(c, w).DataDictionary["PermittedRoles"].DataList;
                    
                    if (DataUtil.Intersection(obtainedRoles, worldPermittedRoles).Count == 0)
                    {
                        forbiddenWorldIndexes.Add(w);
                    }                        
                }
            }

            return forbiddenWorldIndexes;
        }

        public DataToken GetCategory(int c)
        {
            return _categorys.DataList[c];
        }
        
        public DataToken GetWorld(int c, int w)
        {
            return _categorys.DataList[c].DataDictionary["Worlds"].DataList[w];
        }
    }

    public enum DataType
    {
        Static, Json
    }
}
