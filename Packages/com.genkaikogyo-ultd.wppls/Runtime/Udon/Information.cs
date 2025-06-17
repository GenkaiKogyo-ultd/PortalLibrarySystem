using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Information : UdonSharpBehaviour
    {
        [SerializeField] private TextMeshProUGUI  _text;
        [SerializeField] private RawImage         _thumbnail;
        [SerializeField] private GameObject       _creditObj;
        [SerializeField] private GameObject       _descriptionObj;
        [SerializeField] private ButtonBackground _goButtonBackground;
        [SerializeField] private ButtonBackground _creditButtonBackground;
        
        private string  _id;
        private string  _description;
        private Texture _texture;
        private Texture _defaultThumbnailTexture;
        private bool    _isDisplayingCredit;
        
        public string ID { get { return _isDisplayingCredit ? "" : _id; } }

        public void Initialize()
        {
            _defaultThumbnailTexture = _thumbnail.texture;
            UpdateInformation("", "", null);
        }
        
        public void UpdateInformation(string id, string description, Texture texture)
        {
            _id                 = id;
            _description        = description;
            _texture            = texture;
            _isDisplayingCredit = false;

            if (_texture != null && _texture.GetType() == typeof(RenderTexture) &&
                _thumbnail.texture != _texture)
            {
                // Jsonモードで、サムネイル画像を他画像から RenderTexture へと差し替える際
                // RenderTexture に更新前の画像が残っていて一瞬チラつくことがある
                // RenderTexture が更新されるまで10フレームぐらい待つことで対処している
                SendCustomEventDelayedFrames(nameof(_UpdateInformation), 10);
            }
            else
            {
                _UpdateInformation();
            }
        }

        public void _UpdateInformation()
        {
            _creditObj.SetActive(_isDisplayingCredit);
            _descriptionObj.SetActive(!_isDisplayingCredit);

            if (_isDisplayingCredit)
            {
                _goButtonBackground.SetBackground(ButtonState.Invalid);
                _creditButtonBackground.SetBackground(ButtonState.Selected);
            }
            else
            {
                _text.text         = _description;
                _thumbnail.texture = (_texture != null) ? _texture : _defaultThumbnailTexture;

                _goButtonBackground.SetBackground(_id != "" ? ButtonState.Unselected : ButtonState.Invalid);
                _creditButtonBackground.SetBackground(ButtonState.Unselected);
            }
        }

        public void ToggleCredit()
        {
            _isDisplayingCredit = !_isDisplayingCredit;
            _UpdateInformation();
        }
    }
}
