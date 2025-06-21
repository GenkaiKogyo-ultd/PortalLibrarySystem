using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class WorldButton : Button
    {
        [SerializeField] private RectTransform _capacitys;
        [SerializeField] private RectTransform _capacity;
        [SerializeField] private RectTransform _slash;
        [SerializeField] private RectTransform _recommendedCapacity;

        [SerializeField] private RectTransform _platform;
        [SerializeField] private GameObject    _pc;
        [SerializeField] private GameObject    _android;
        [SerializeField] private GameObject    _ios;

        private string   _id;

        public void Initialize(int index, string text, bool isRolesDefined,
                               string id, int capacity, int recommendedCapacity,
                               bool pc, bool android, bool ios)
        {
            base._Initialize(index, text, isRolesDefined);

            _capacity.GetComponent<TextMeshProUGUI>().text            = capacity.ToString();
            _recommendedCapacity.GetComponent<TextMeshProUGUI>().text = recommendedCapacity.ToString();

            var capacitysWidth = SetCapacitysWidth(0, capacity, recommendedCapacity);
            var platformWidth  = SetPlatformWidth(capacitysWidth, pc, android, ios);

            _text.offsetMax = new Vector2(-(capacitysWidth + platformWidth), 0);

            _id = id;

            UpdateButtonState(-1);
        }

        private float SetCapacitysWidth(float offset, int capacity, int recommendedCapacity)
        {
            SetRectActive(_capacity           , capacity > 0);
            SetRectActive(_slash              , capacity > 0 && recommendedCapacity > 0);
            SetRectActive(_recommendedCapacity, capacity > 0 && recommendedCapacity > 0);

            float capacitysWidth = (GetRectWidth(_capacity) + GetRectWidth(_slash) +
                                    GetRectWidth(_recommendedCapacity));

            _capacitys.offsetMax = new Vector2(-(offset),                  0);
            _capacitys.offsetMin = new Vector2(-(offset + capacitysWidth), 0);

            return capacitysWidth;
        }

        private float SetPlatformWidth(float offset, bool pc, bool android, bool ios)
        {
            _pc.SetActive(pc);
            _android.SetActive(android);
            _ios.SetActive(ios);

            float platformWidth = pc || android || ios ? GetRectWidth(_platform) : 0;

            _platform.offsetMax = new Vector2(-(offset),                 0);
            _platform.offsetMin = new Vector2(-(offset + platformWidth), 0);

            return platformWidth;
        }
        
        public void OnClicked()
        {
            if (!string.IsNullOrEmpty(_id))
            {
                _pls.OnWorldButtonClicked(Index);
            }
        }

        public void UpdateButtonState(int index)
        {
            if (string.IsNullOrEmpty(_id))
            {
                SetButtonState(ButtonState.Invalid);
            }
            else if (Index == index)
            {
                SetButtonState(ButtonState.Selected);
            }
            else
            {
                SetButtonState(ButtonState.Unselected);
            }
        }

        private void SetRectActive(RectTransform rect, bool b)
        {
            rect.gameObject.SetActive(b);
        }

        private float GetRectWidth(RectTransform rect)
        {
            return rect.gameObject.activeSelf ? rect.rect.width : 0;
        }
    }
}
