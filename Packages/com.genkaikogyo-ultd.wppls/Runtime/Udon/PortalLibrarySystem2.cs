using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PortalLibrarySystem2 : UdonSharpBehaviour
    {
        [SerializeField] private PortalManager _portalManager;

        private WorldDataProcessor _worldDataProcessor;
        private CategorySelector   _categorySelector;
        private WorldSelector      _worldSelector;
        private Information        _information;

        private void Start()
        {
            _worldDataProcessor = GetComponentInChildren<WorldDataProcessor>();
            _categorySelector   = GetComponentInChildren<CategorySelector>();
            _worldSelector      = GetComponentInChildren<WorldSelector>();
            _information        = GetComponentInChildren<Information>();

            _worldDataProcessor.Initialize();
            _categorySelector.Initialize();
            _worldSelector.Initialize();
            _information.Initialize();

            if (_portalManager != null)
            {
                SendCustomEventDelayedSeconds(nameof(_LoadWorldData),
                                              _worldDataProcessor.WorldDataLoadDelaySeconds);
                SendCustomEventDelayedSeconds(nameof(_LoadThumbnails),
                                              _worldDataProcessor.ThumbnailsLoadDelaySeconds);
            }
            else
            {
                string text = "Error: PortalManagerが設定されていません！！";
                _information.UpdateInformation("", text, null);
            }
        }

        public void Reload()
        {
            _LoadWorldData();
            _LoadThumbnails();
        }

        public void _LoadWorldData()
        {
            _worldDataProcessor.LoadWorldData();
        }

        public void _LoadThumbnails()
        {
            _worldDataProcessor.LoadThumbnails();
        }

        public void OnWorldDataLoaded()
        {
            if (_worldDataProcessor.Categorys.DataList.Count > 0)
            {
                _categorySelector.GenerateButtons(_worldDataProcessor.Categorys);
                _worldSelector.GenerateButtons(_worldDataProcessor.GetCategory(0).DataDictionary["Worlds"]);
                _information.UpdateInformation("", "", null);
            }
            else
            {
                string text = "Error: ワールドのデータが1つも登録されていません！";
                _information.UpdateInformation("", text, null);                
            }
        }          

        public void OnCategoryButtonClicked(int index)
        {
            if (index != _categorySelector.Index)
            {
                _categorySelector.Select(index);
                _worldSelector.GenerateButtons(_worldDataProcessor.GetCategory(index).DataDictionary["Worlds"]);
            }
        }

        public void OnWorldButtonClicked(int index)
        {
            if (index != _worldSelector.Index)
            {
                _worldSelector.Select(index);

                var thumbnailIndex = DataUtil.ForceValueInt(GetSelectedWorld(), "ThumbnailIndex");
                
                _information.UpdateInformation(DataUtil.ForceValueString(GetSelectedWorld(), "ID"),
                                               DataUtil.ForceValueString(GetSelectedWorld(), "Description"),
                                               _worldDataProcessor.GetThumbnail(thumbnailIndex));
            }
        }

        public void GeneratePortal()
        {
            if (_information.ID != "") _portalManager.GeneratePortal(_information.ID);
        }

        public void ResetPortal()
        {
            _portalManager.ResetPortal();
        }

        private DataToken GetSelectedWorld()
        {
            int c = _categorySelector.Index;
            int w = _worldSelector.Index;

            return _worldDataProcessor.GetWorld(c, w);
        }
    }
}
