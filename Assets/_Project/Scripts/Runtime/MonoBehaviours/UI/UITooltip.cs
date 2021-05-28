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
using UnityEngine.UI;

namespace Arcade
{
    [DisallowMultipleComponent, RequireComponent(typeof(Image))]
    public sealed class UITooltip : MonoBehaviour
    {
        [SerializeField] private FloatVariable _animationDuration;
        [SerializeField] private Color _backgroundColor = new Color32(40, 40, 40, 180);

        private Image _background;
        private TMP_Text _text;

        private void Awake()
        {
            _background = GetComponent<Image>();
            _text       = GetComponentInChildren<TMP_Text>();
        }

        public void Show()
        {
            _ = _background.DOColor(_backgroundColor, _animationDuration.Value);
            _ = _text.DOColor(Color.white, _animationDuration.Value);
        }

        public void Hide()
        {
            _ = _background.DOColor(Color.clear, _animationDuration.Value);
            _ = _text.DOColor(Color.clear, _animationDuration.Value);
        }

#if UNITY_EDITOR
        [ContextMenu("Show")]
        public void Editor_Show()
        {
            GetComponent<Image>().color              = _backgroundColor;
            GetComponentInChildren<TMP_Text>().color = Color.white;
        }

        [ContextMenu("Hide")]
        public void Editor_Hide()
        {
            GetComponent<Image>().color              = Color.clear;
            GetComponentInChildren<TMP_Text>().color = Color.clear;
        }
#endif
    }
}
