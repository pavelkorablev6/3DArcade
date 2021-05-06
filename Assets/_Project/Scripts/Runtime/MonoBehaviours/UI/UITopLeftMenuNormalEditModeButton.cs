/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UITopLeftMenuNormalEditModeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private UITopLeftMenu _topLeftMenu;
        [SerializeField] private TypeEvent _arcadeStateTransitionEvent;

        public void OnPointerEnter(PointerEventData eventData) => ShowText();

        public void OnPointerExit(PointerEventData eventData) => HideText();

        public void OnPointerClick(PointerEventData eventData)
        {
            HideText();
            _topLeftMenu.Hide();
            _arcadeStateTransitionEvent.Raise(typeof(ArcadeNormalFpsEditModeState));
        }

        public void TransitionTo(System.Type type) => _arcadeStateTransitionEvent.Raise(type);

        private void ShowText() => _text.DOColor(Color.white, 0.3f);

        private void HideText() => _text.DOColor(Color.clear, 0.3f);
    }
}
