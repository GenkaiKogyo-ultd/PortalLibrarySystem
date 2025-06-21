using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace Wacky612.PortalLibrarySystem2
{
    public abstract class Button : UdonSharpBehaviour
    {
        [SerializeField] private protected RectTransform _text;

        [SerializeField] private Image _background;
        [SerializeField] private Color _unselectedColor   = Color.white;
        [SerializeField] private Color _selectedColor     = new Color(0.5f, 1.0f, 1.0f, 1.0f);
        [SerializeField] private Color _invalidColor      = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        [SerializeField] private Color _rolesDefinedColor = new Color(0.8f, 0.6f, 1.0f, 1.0f);

        private protected PortalLibrarySystem2 _pls;

        private int           _index;
        private RectTransform _rect;
        private float         _height;
        private bool          _isRolesDefined; 

        public int   Index  { get { return _index;  } }
        public float Height { get { return _height; } }
        
        private protected void _Initialize(int index, string text, bool isRolesDefined)
        {
            _index          = index;
            _rect           = GetComponent<RectTransform>();
            _height         = _rect.offsetMax.y - _rect.offsetMin.y;
            _pls            = GetComponentInParent<PortalLibrarySystem2>();
            _isRolesDefined = isRolesDefined;
            
            _rect.offsetMax = new Vector2(0, -( _index      * _height));
            _rect.offsetMin = new Vector2(0, -((_index + 1) * _height));
            
            _text.GetComponent<TextMeshProUGUI>().text = text;

            SetButtonState(ButtonState.Unselected);
        }
        
        private protected void SetButtonState(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Unselected:
                    _background.color = _isRolesDefined ? _rolesDefinedColor : _unselectedColor;
                    break;
                case ButtonState.Selected:
                    _background.color = _selectedColor;
                    break;
                case ButtonState.Invalid:
                    _background.color = _invalidColor;
                    break;
            }
        }
    }

    public enum ButtonState
    {
        Unselected, Selected, Invalid
    }
}
