using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Categorys : UdonSharpBehaviour
    {
        public DataToken GetDataToken()
        {
            DataToken data = new DataToken(new DataList());

            for (int i = 0; i < this.transform.childCount; i++)
            {
                var obj = this.transform.GetChild(i).gameObject;

                if (obj.activeSelf)
                {
                    var category = obj.GetComponent<Category>();
                    if (category != null) data.DataList.Add(category.GetDataToken());
                }
            }

            return data;
        }
    }
}
