using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Category : UdonSharpBehaviour
    {
        [SerializeField] private string   _categoryName;
        [SerializeField] private string[] _permittedRoles;

        public DataToken GetDataToken()
        {
            DataToken data = new DataToken(new DataDictionary());
            data.DataDictionary["Category"] = _categoryName;
            data.DataDictionary["Worlds"]   = new DataToken(new DataList());

            for (int i = 0; i < this.transform.childCount; i++)
            {
                var obj = this.transform.GetChild(i).gameObject;

                if (obj.activeSelf)
                {
                    var world = obj.GetComponent<World>();
                    if (world != null) data.DataDictionary["Worlds"].DataList.Add(world.GetDataToken());
                }
            }

            if (_permittedRoles.Length > 0)
            {
                data.DataDictionary["PermittedRoles"] = new DataToken(new DataList());

                foreach (var role in _permittedRoles)
                {
                    data.DataDictionary["PermittedRoles"].DataList.Add(role);
                }
            }

            return data;
        }
    }
}
