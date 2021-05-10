﻿/* MIT License

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
    [DisallowMultipleComponent]
    public sealed class UISelectionText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetText(ModelConfigurationComponentPair modelConfigurationComponentPair)
        {
            string description = string.Empty;

            if (modelConfigurationComponentPair.Current != null)
            {
                ModelConfiguration modelConfiguration = modelConfigurationComponentPair.Current.Configuration;

                if (!string.IsNullOrEmpty(modelConfiguration.Overrides.Description))
                    description = modelConfiguration.Overrides.Description;
                else if (modelConfiguration.GameConfiguration != null && !string.IsNullOrEmpty(modelConfiguration.GameConfiguration.Description))
                    description = modelConfiguration.GameConfiguration.Description;
                else
                    description = modelConfiguration.Id;
            }

            _text.SetText(description);
            _ = _text.DOColor(!string.IsNullOrEmpty(description) ? Color.white : Color.clear, 0.8f);
        }
    }
}
