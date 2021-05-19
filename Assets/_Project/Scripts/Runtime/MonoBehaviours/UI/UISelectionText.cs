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
    public sealed class UISelectionText : MonoBehaviour
    {
        [SerializeField] private float _animationSpeed = 0.6f;

        private TMP_Text _text;
        private Tween _tween;

        private void Awake() => _text = GetComponent<TMP_Text>();

        public void SetValue(ModelConfigurationComponent modelConfigurationComponent)
        {
            string description = modelConfigurationComponent != null ? GetDescription(modelConfigurationComponent) : string.Empty;
            _text.SetText(description);

            _tween?.Kill();
            _tween = _text.DOColor(!string.IsNullOrEmpty(description) ? Color.white : Color.clear, _animationSpeed);
        }

        public void ResetValue()
        {
            _tween?.Kill();
            _tween = _text.DOColor(Color.clear, _animationSpeed)
                          .OnComplete(() => _text.Clear());
        }

        private static string GetDescription(ModelConfigurationComponent modelConfigurationComponent)
        {
            if (modelConfigurationComponent == null)
                return string.Empty;

            ModelConfiguration modelConfiguration = modelConfigurationComponent.Configuration;

            if (!string.IsNullOrEmpty(modelConfiguration.Overrides.Description))
                return modelConfiguration.Overrides.Description;

            if (modelConfiguration.GameConfiguration != null && !string.IsNullOrEmpty(modelConfiguration.GameConfiguration.Description))
                return modelConfiguration.GameConfiguration.Description;

            return modelConfiguration.Id;
        }
    }
}
