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
    [DisallowMultipleComponent]
    public sealed class UIStandardEditModeEditPanel: MonoBehaviour
    {
        [SerializeField] private TMP_InputField _id;
        [SerializeField] private TMP_Dropdown _interactionType;
        [SerializeField] private TMP_InputField _platform;
        [SerializeField] private Toggle _grabbable;
        [SerializeField] private Toggle _movecabMovable;
        [SerializeField] private Toggle _movecabGrabbable;
        [SerializeField] private ArcadeContext _arcadeContext;

        private RectTransform _transform;
        private ModelConfigurationComponent _target;

        private void Awake() => _transform = transform as RectTransform;

        private void OnEnable() => Show();

        private void OnDisable() => Hide();

        public void Show() => _transform.DOAnchorPosX(20f, 0.4f);

        public void Hide() => _transform.DOAnchorPosX(-340f, 0.4f);

        public void SetUIData(ModelConfigurationComponentPair modelConfigurationComponentPair)
        {
            if (modelConfigurationComponentPair.Previous == null)
            {
                ResetUIData();
                return;
            }

            _target = modelConfigurationComponentPair.Previous;

            ModelConfiguration modelConfiguration = modelConfigurationComponentPair.Previous.Configuration;

            _id.text               = modelConfiguration.Id;
            _interactionType.value = (int)modelConfiguration.InteractionType;
            _platform.text         = modelConfiguration.PlatformConfiguration?.Id;
            _grabbable.isOn        = modelConfiguration.Grabbable;
            _movecabMovable.isOn   = modelConfiguration.MoveCabMovable;
            _movecabGrabbable.isOn = modelConfiguration.MoveCabGrabbable;
        }

        public void ApplyChanges()
        {
            if (_target == null)
                return;

            ModelConfiguration cfg = _target.Configuration;
            cfg.Id                 = _id.text;
            cfg.InteractionType    = (InteractionType)_interactionType.value;
            cfg.Platform           =_platform.text;
            cfg.Grabbable          =_grabbable.isOn;
            cfg.MoveCabMovable     =_movecabMovable.isOn;
            cfg.MoveCabGrabbable   =_movecabGrabbable.isOn;

            _target.SetModelConfiguration(cfg);
            _ = _arcadeContext.SaveCurrentArcade();
        }

        private void ResetUIData()
        {
            _target                = null;
            _id.text               = null;
            _interactionType.value = 0;
            _platform.text         = null;
            _grabbable.isOn        = false;
            _movecabMovable.isOn   = false;
            _movecabGrabbable.isOn = false;
        }
    }
}
