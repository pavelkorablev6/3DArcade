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
    public sealed class UITopLeftMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private NormalModeInteractionController _normalModeInteractionController;
        [SerializeField] private EditModeInteractionController _editModeInteractionController;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _normalModeInteractionController.Enabled = false;
            _editModeInteractionController.Enabled   = false;
            Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Hide();
            _normalModeInteractionController.Enabled = true;
            _editModeInteractionController.Enabled   = true;
        }

        public void Show() => _panel.DOAnchorPosX(50f, 0.4f);

        public void Hide() => _panel.DOAnchorPosX(0f, 0.4f);
    }
}
