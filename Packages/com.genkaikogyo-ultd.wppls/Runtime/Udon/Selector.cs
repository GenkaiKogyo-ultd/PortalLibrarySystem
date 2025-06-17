using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    public abstract class Selector : UdonSharpBehaviour
    {
        [SerializeField] private protected GameObject _buttonTemplate;
        [SerializeField] private protected Transform  _parentOfButtons;
        [SerializeField] private           GameObject _parentOfTemplate;
        [SerializeField] private           GameObject _loadingEffect;

        private protected int          _pIndex;
        private protected DataToken    _pData;
        private protected GameObject[] _buttons;
        private           ScrollRect   _scrollRect;

        private int _index;
        public  int Index { get { return _index; }}

        public void Initialize()
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();
            _parentOfTemplate.SetActive(false);
            SetScrollAreaHeight(0);            
            SetLoadingEffectActive(true);
            _pIndex = -1;
        }

        public virtual void Select(int index)
        {
            _index = index;
        }

        private protected void DestroyButtons()
        {
            if (_buttons != null)
            {
                foreach (var obj in _buttons)
                {
                    if (Utilities.IsValid(obj)) GameObject.Destroy(obj);
                }
            }
        }

        private protected void SetScrollAreaHeight(float height)
        {
            _scrollRect.verticalNormalizedPosition = 1.0f;
            _scrollRect.content.offsetMin          = new Vector2(0, -height);
        }

        private protected float GetViewPortHeight()
        {
            return _scrollRect.viewport.rect.height;
        }

        private protected void SetLoadingEffectActive(bool b)
        {
            _loadingEffect.SetActive(b);
        }
    }
}
