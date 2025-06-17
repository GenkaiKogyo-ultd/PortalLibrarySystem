using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Role : UdonSharpBehaviour
    {
        [SerializeField] private string   _roleName;
        [SerializeField] private string[] _displayNames;

        public DataToken GetDataToken()
        {
            DataToken data = new DataToken(new DataDictionary());
            data.DataDictionary["RoleName"]     = _roleName;
            data.DataDictionary["DisplayNames"] = new DataToken(new DataList());

            foreach (var displayName in _displayNames)
            {
                data.DataDictionary["DisplayNames"].DataList.Add(displayName);
            }

            return data;
        }
    }
}
