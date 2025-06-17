using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Description : UdonSharpBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        
        public void UpdateDescription(string description)
        {
            _text.text = description;
        }
    }
}
