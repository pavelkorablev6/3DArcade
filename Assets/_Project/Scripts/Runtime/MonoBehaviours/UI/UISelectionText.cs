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

namespace Arcade
{
    [DisallowMultipleComponent, RequireComponent(typeof(TMP_Text))]
    public sealed class UISelectionText : MonoBehaviour
    {
        [SerializeField] private FloatVariable _animationDuration;

        private TMP_Text _text;
        private Tween _tween;

        private void Awake() => _text = GetComponent<TMP_Text>();

        public void SetVisibility(bool visible)
        {
            if (visible)
                SetText(null);
            else
                ResetText();
        }

        public void SetText(ModelConfigurationComponent modelConfigurationComponent)
        {
            if (modelConfigurationComponent == null)
            {
                ResetText();
                return;
            }

            string description = modelConfigurationComponent.Configuration.GetDescription();
            _tween?.Kill();
            _text.SetText(description);
            _tween = _text.DOColor(Color.white, _animationDuration.Value);
        }

        public void ResetText()
        {
            _tween?.Kill();
            _tween = _text.DOColor(Color.clear, _animationDuration.Value)
                          .OnComplete(() => _text.Clear());
        }
    }
}
