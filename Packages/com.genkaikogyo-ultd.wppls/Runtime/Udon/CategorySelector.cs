using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;

namespace Wacky612.PortalLibrarySystem2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CategorySelector : Selector
    {
        private CategoryButton[] _categoryButtons;
        
        public void GenerateButtons(DataToken data)
        {
            SetLoadingEffectActive(false);
            DestroyButtons();

            _buttons         = new GameObject[data.DataList.Count];
            _categoryButtons = new CategoryButton[data.DataList.Count];

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

            while (true)
            {
                _buttons[_pIndex]         = Instantiate(_buttonTemplate, _parentOfButtons);
                _categoryButtons[_pIndex] = _buttons[_pIndex].GetComponent<CategoryButton>();

                var text = DataUtil.ForceString(_pData.DataList[_pIndex].DataDictionary["Category"]);

                _categoryButtons[_pIndex].Initialize(_pIndex, text);

                if (_pIndex == 0)
                {
                    SetScrollAreaHeight(_categoryButtons[0].Height * _pData.DataList.Count);
                    Select(0);
                }

                if (step == 0) step = (int) Math.Ceiling(GetViewPortHeight() / _categoryButtons[0].Height);

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
            
            foreach (var cb in _categoryButtons)
            {
                if (cb != null) cb.UpdateButtonState(index);
            }
        }
    }
}
