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
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIConfigurationEmulators : MonoBehaviour
    {
        [SerializeField] private EmulatorsDatabase _emulatorsDatabase;
        [SerializeField] private Button _addButton;
        [SerializeField] private TMP_InputField _addInputField;
        [SerializeField] private RectTransform _listContent;
        [SerializeField] private UIEmulatorsListButton _listButtonPrefab;
        [SerializeField] private UIConfigurationEmulatorEdit _emulatorEdit;

        private readonly List<UIEmulatorsListButton> _buttons = new List<UIEmulatorsListButton>();

        private RectTransform _transform;

        private void Awake() => _transform = transform as RectTransform;

        public void Show()
        {
            _addButton.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(_addInputField.text))
                    return;

                string text = _addInputField.text;
                EmulatorConfiguration cfg = new EmulatorConfiguration { Id = text, Description = text };
                if (_emulatorsDatabase.Add(cfg) == null)
                    return;

                _addInputField.text = null;
                InitializeList();
            });

            InitializeList();
            _ = _transform.DOAnchorPosX(0f, 0.3f);
        }

        public void Hide()
        {
            _addButton.onClick.RemoveAllListeners();
            _ = _transform.DOAnchorPosX(-500f, 0.3f);
        }

        private void InitializeList()
        {
            foreach (UIEmulatorsListButton button in _buttons)
                Destroy(button.gameObject);
            _buttons.Clear();

            _emulatorsDatabase.Initialize();

            EmulatorConfiguration[] emulators = _emulatorsDatabase.GetValues();
            foreach (EmulatorConfiguration emulator in emulators)
            {
                UIEmulatorsListButton buttonObject = Instantiate(_listButtonPrefab, _listContent);
                _buttons.Add(buttonObject);

                buttonObject.name = emulator.Id;

                buttonObject.EmulatorButtonText.SetText(emulator.Description);

                buttonObject.EmulatorButton.onClick.AddListener(() =>
                {
                    Hide();
                    _emulatorEdit.TitleText.SetText(emulator.ToString());
                    _emulatorEdit.Show(emulator);
                });

                buttonObject.DeleteButton.onClick.AddListener(() =>
                {
                    _ = _emulatorsDatabase.Delete(emulator.Id);
                    InitializeList();
                });
            }
        }
    }
}
