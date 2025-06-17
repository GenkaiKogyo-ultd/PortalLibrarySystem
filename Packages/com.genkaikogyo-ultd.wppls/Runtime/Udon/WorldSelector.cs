using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class WorldSelector : Selector
    {
        private WorldButton[] _worldButtons;

        public void GenerateButtons(DataToken data)
        {
            SetLoadingEffectActive(false);
            DestroyButtons();

            _buttons      = new GameObject[data.DataList.Count];
            _worldButtons = new WorldButton[data.DataList.Count];

            if (_pIndex == -1)
            {
                // _pIndex が -1 でない時は、現在走っている _GenerateButtonsLoop を利用する
                SendCustomEventDelayedFrames(nameof(_GenerateButtonsLoop), 1);
            }

            _pIndex = 0;
            _pData  = data;
        }

        // Button の生成には1つあたり2ms程度の時間がかかるため、
        // 1フレームで全ての Button を生成すると処理落ちを起こしてしまう。
        // スクロールしない状態で生成されていないといけない Button の数を求めて、
        // 1フレームにつき、その数の Button を生成するような処理を書いている。
        public void _GenerateButtonsLoop()
        {
            int step = 0;
            
            if (_pData.DataList.Count == 0)
            {
                _pIndex = -1;
                return;
            }

            while (true)
            {
                _buttons[_pIndex]      = Instantiate(_buttonTemplate, _parentOfButtons);
                _worldButtons[_pIndex] = _buttons[_pIndex].GetComponent<WorldButton>();

                var text                = DataUtil.ForceString(_pData.DataList[_pIndex].DataDictionary["Name"]);
                var id                  = DataUtil.ForceString(_pData.DataList[_pIndex].DataDictionary["ID"]);
                var capacity            = DataUtil.ForceValueInt(_pData.DataList[_pIndex], "Capacity");
                var recommendedCapacity = DataUtil.ForceValueInt(_pData.DataList[_pIndex], "RecommendedCapacity");
                var pc                  = IsSupported(_pData.DataList[_pIndex], "PC");
                var android             = IsSupported(_pData.DataList[_pIndex], "Android");
                var ios                 = IsSupported(_pData.DataList[_pIndex], "iOS");

                _worldButtons[_pIndex].Initialize(_pIndex, text, id, capacity, recommendedCapacity,
                                                  pc, android, ios);

                if (_pIndex == 0)
                {
                    SetScrollAreaHeight(_worldButtons[0].Height * _pData.DataList.Count);
                    Select(-1);
                }

                if (step == 0) step = (int) Math.Ceiling(GetViewPortHeight() / _worldButtons[0].Height);

                _pIndex += 1;

                if (_pIndex == _pData.DataList.Count)
                {
                    _pIndex = -1;
                    break;
                }
                else if (_pIndex % step == 0)
                {
                    SendCustomEventDelayedFrames(nameof(_GenerateButtonsLoop), 1);
                    break;
                }
            }
        }

        public override void Select(int index)
        {
            base.Select(index);
            
            foreach (var wb in _worldButtons)
            {
                if (wb != null) wb.UpdateButtonState(index);
            }
        }

        private bool IsSupported(DataToken data, string key)
        {
            return DataUtil.ForceValueBool(DataUtil.ForceValue(data, "Platform"), key);
        }
    }
}
