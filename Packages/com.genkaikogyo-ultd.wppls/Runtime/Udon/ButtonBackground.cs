using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ButtonBackground : UdonSharpBehaviour
    {
        [SerializeField] private Image _img;
        [SerializeField] private Color _unselectedColor = Color.white;
        [SerializeField] private Color _selectedColor   = new Color(0.5f, 1.0f, 1.0f, 1.0f);
        [SerializeField] private Color _invalidColor    = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        public void SetBackground(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Unselected:
                    _img.color = _unselectedColor;
                    break;
                case ButtonState.Selected:
                    _img.color = _selectedColor;
                    break;
                case ButtonState.Invalid:
                    _img.color = _invalidColor;
                    break;
            }
        }
    }
}
