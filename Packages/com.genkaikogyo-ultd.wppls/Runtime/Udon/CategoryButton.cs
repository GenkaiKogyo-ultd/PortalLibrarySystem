using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CategoryButton : Button
    {
        public void Initialize(int index, string text)
        {
            base._Initialize(index, text);
        }
        
        public void OnClicked()
        {
            _pls.OnCategoryButtonClicked(Index);
        }

        public void UpdateButtonState(int index)
        {
            if (Index == index)
            {
                SetButtonState(ButtonState.Selected);
            }
            else
            {
                SetButtonState(ButtonState.Unselected);
            }
        }
    }
}
