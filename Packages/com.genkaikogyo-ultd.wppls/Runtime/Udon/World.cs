using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class World : UdonSharpBehaviour
    {
        [SerializeField] private string        _id;
        [SerializeField] private string        _name;
        [SerializeField] private int           _recommendedCapacity = 0;
        [SerializeField] private int           _capacity            = 0;
        [SerializeField] private ReleaseStatus _releaseStatus       = ReleaseStatus.Public;
        [SerializeField] private bool          _pc                  = false;
        [SerializeField] private bool          _android             = false;
        [SerializeField] private bool          _ios                 = false;
        [TextArea(3,12)]
        [SerializeField] private string        _description;
        [SerializeField] private Texture2D     _thumbnail;
        [SerializeField] private string[]      _permittedRoles;

        public Texture2D Thumbnail { get { return _thumbnail; } }
        
        public DataToken GetDataToken()
        {
            DataToken data = new DataToken(new DataDictionary());
            
            data.DataDictionary["ID"]                  = _id;
            data.DataDictionary["Name"]                = _name;
            data.DataDictionary["RecommendedCapacity"] = _recommendedCapacity;
            data.DataDictionary["Capacity"]            = _capacity;
            data.DataDictionary["Description"]         = _description;

            if (_pc || _android || _ios)
            {
                data.DataDictionary["Platform"] = new DataToken(new DataDictionary());
                data.DataDictionary["Platform"].DataDictionary["PC"]      = _pc;
                data.DataDictionary["Platform"].DataDictionary["Android"] = _android;
                data.DataDictionary["Platform"].DataDictionary["iOS"]     = _ios;
            }

            switch (_releaseStatus)
            {
                case ReleaseStatus.Public:
                    data.DataDictionary["ReleaseStatus"] = "public";
                    break;
                case ReleaseStatus.Private:
                    data.DataDictionary["ReleaseStatus"] = "private";
                    break;
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

    public enum ReleaseStatus
    {
        Public, Private
    }
}
