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
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIStandardEditModeEditPanel: MonoBehaviour
    {
        [SerializeField] private TMP_InputField _idInputField;
        [SerializeField] private TMP_Dropdown _interactionTypeDropdown;
        [SerializeField] private TMP_Dropdown _platformDropdown;
        [SerializeField] private Toggle _grabbableToggle;
        [SerializeField] private Toggle _movecabMovableToggle;
        [SerializeField] private Toggle _movecabGrabbableToggle;

        private RectTransform _transform;

        private void Awake() => _transform = transform as RectTransform;

        private void Start()
        {
            _interactionTypeDropdown.ClearOptions();
            _interactionTypeDropdown.AddOptions(System.Enum.GetNames(typeof(InteractionType)).ToList());
        }

        public void Show(PlatformsDatabase platformsDatabase)
        {
            _platformDropdown.ClearOptions();
            _platformDropdown.AddOptions(new List<string> { "" }.Concat(platformsDatabase.GetNames()).ToList());

            _ = _transform.DOAnchorPosX(20f, 0.4f);
        }

        public void Hide() => _transform.DOAnchorPosX(-340f, 0.4f);

        public void SetUIData(ModelConfiguration modelConfiguration)
        {
            _idInputField.text             = modelConfiguration.Id;
            _interactionTypeDropdown.value = (int)modelConfiguration.InteractionType;
            _platformDropdown.value        = modelConfiguration.PlatformConfiguration != null
                                           ? _platformDropdown.options.FindIndex(x => x.text == modelConfiguration.PlatformConfiguration.Id)
                                           : 0;
            _grabbableToggle.isOn        = modelConfiguration.Grabbable;
            _movecabMovableToggle.isOn   = modelConfiguration.MoveCabMovable;
            _movecabGrabbableToggle.isOn = modelConfiguration.MoveCabGrabbable;
        }

        public void ResetUIData()
        {
            _idInputField.text             = null;
            _interactionTypeDropdown.value = 0;
            _platformDropdown.value        = 0;
            _grabbableToggle.isOn          = false;
            _movecabMovableToggle.isOn     = false;
            _movecabGrabbableToggle.isOn   = false;
        }

        public ModelConfiguration UpdatedModelConfigurationValues(ModelConfiguration modelConfiguration)
        {
            modelConfiguration.Id               = _idInputField.text;
            modelConfiguration.InteractionType  = (InteractionType)_interactionTypeDropdown.value;
            modelConfiguration.Platform         = _platformDropdown.options[_platformDropdown.value].text;
            modelConfiguration.Grabbable        = _grabbableToggle.isOn;
            modelConfiguration.MoveCabMovable   = _movecabMovableToggle.isOn;
            modelConfiguration.MoveCabGrabbable = _movecabGrabbableToggle.isOn;

            return modelConfiguration;
        }
    }
}
