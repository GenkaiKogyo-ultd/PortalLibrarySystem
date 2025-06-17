using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.SDK3.Components;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PortalManager : UdonSharpBehaviour
    {
        [SerializeField] private bool _isGlobal = true;
        
        private VRCPortalMarker[] _portals;
        private int               _index;

        [UdonSynced]
        private string _json;

        private void Start()
        {
            _portals = GetComponentsInChildren<VRCPortalMarker>();

            if (_isGlobal)
            {
                if (Networking.IsOwner(this.gameObject)) ResetPortal();
            }
            else
            {
                ResetPortal();
            }
        }

        public void GeneratePortal(string id)
        {
            if (_portals != null)
            {
                _index = (_index + 1) % _portals.Length;

                SetPortalID(_portals[_index], id);
                _RequestSerialization();
            }
        }

        public void ResetPortal()
        {
            if (_portals != null)
            {
                foreach (var portal in _portals) SetPortalID(portal, "");

                _index = -1;
                _RequestSerialization();
            }
        }

        private void SetPortalID(VRCPortalMarker portal, string id)
        {
            portal.roomId = id;
            portal.gameObject.SetActive(id != "");
        }

        private void _RequestSerialization()
        {
            if (_isGlobal)
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
                RequestSerialization();
            }
        }

        public override void OnPreSerialization()
        {
            var data = new DataDictionary();
            data["Index"]   = _index;
            data["RoomIDs"] = new DataList();

            foreach (var portal in _portals) data["RoomIDs"].DataList.Add(portal.roomId);

            if (VRCJson.TrySerializeToJson(data, JsonExportType.Minify, out DataToken result))
            {
                _json = result.String;
            }
        }

        public override void OnDeserialization()
        {
            if (_isGlobal) _TryToDecodeJson();
        }

        public void _TryToDecodeJson()
        {
            if (_portals != null)
            {
                DecodeJson();
            }
            else
            {
                SendCustomEventDelayedFrames(nameof(_TryToDecodeJson), 1);
            }
        }

        private void DecodeJson()
        {
            if (VRCJson.TryDeserializeFromJson(_json, out DataToken result))
            {
                var data = result.DataDictionary;
                
                _index = (int) data["Index"].Double;

                for (int i = 0; i < _portals.Length; i++)
                {
                    SetPortalID(_portals[i], data["RoomIDs"].DataList[i].String);
                }
            }
        }
    }
}
