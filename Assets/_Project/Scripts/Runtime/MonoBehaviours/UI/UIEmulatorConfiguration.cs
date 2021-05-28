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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arcade
{
    public sealed class UIEmulatorConfiguration : UIConfiguration<EmulatorsDatabase, EmulatorConfiguration>
    {
        [SerializeField] private TMP_Dropdown _interactionTypeDropdown;
        [SerializeField] private Button _directoryButton;
        [SerializeField] private TMP_InputField _directoryInputField;
        [SerializeField] private Button _workingDirectoryButton;
        [SerializeField] private TMP_InputField _workingDirectoryInputField;
        [SerializeField] private TMP_Text _executableDescriptionText;
        [SerializeField] private Button _executableButton;
        [SerializeField] private TMP_InputField _executableInputField;
        [SerializeField] private TMP_Dropdown _coreDropdown;
        [SerializeField] private TMP_InputField _argumentsInputField;
        [SerializeField] private TMP_InputField _supportedExtensionsInputField;
        [SerializeField] private UIDirectories _gamesDirectories;

        protected override void SetUIValues()
        {
            _interactionTypeDropdown.onValueChanged.AddListener((index) =>
            {
                if ((InteractionType)index == InteractionType.GameInternal)
                {
                    SwitchToLibretroSetup();
                    return;
                }
                SwitchToExternalSetup();
            });

            int interactionType = _configuration.InteractionType <= InteractionType.URL ? (int)_configuration.InteractionType : 0;
            _interactionTypeDropdown.SetValueWithoutNotify(interactionType);
            if (interactionType == (int)InteractionType.GameInternal)
                SwitchToLibretroSetup();
            else
                SwitchToExternalSetup();

            _directoryButton.onClick.AddListener(()
                => _fileExplorer.OpenDirectoryDialog(paths =>
                {
                    if (paths is null || paths.Length == 0)
                        return;
                    _directoryInputField.text          = FileSystemUtils.GetRelativePath(paths[0]);
                    _directoryInputField.caretPosition = 0;
                }));
            _directoryInputField.text = _configuration.Directory;

            _workingDirectoryButton.onClick.AddListener(()
                => _fileExplorer.OpenDirectoryDialog(paths =>
                {
                    if (paths is null || paths.Length == 0)
                        return;
                    _workingDirectoryInputField.text          = FileSystemUtils.GetRelativePath(paths[0]);
                    _workingDirectoryInputField.caretPosition = 0;
                }));
            _workingDirectoryInputField.text = _configuration.WorkingDirectory;

            _executableButton.onClick.AddListener(()
                => _fileExplorer.OpenFileDialog(paths =>
                {
                    if (paths is null || paths.Length == 0)
                        return;
                    if (string.IsNullOrEmpty(_directoryInputField.text))
                    {
                        _directoryInputField.text          = Path.GetDirectoryName(paths[0]);
                        _directoryInputField.caretPosition = 0;
                    }
                    _executableInputField.text          = Path.GetFileName(paths[0]);
                    _executableInputField.caretPosition = 0;

                }));
            _executableInputField.text = _configuration.Executable;

            _argumentsInputField.text = _configuration.Arguments;

            _supportedExtensionsInputField.text = string.Join(";", _configuration.SupportedExtensions);

            _gamesDirectories.Init(_fileExplorer, _configuration.GamesDirectories);
        }

        protected override void GetUIValues()
        {
            _configuration.Description         = _descriptionInputField.text;
            _configuration.InteractionType     = (InteractionType)_interactionTypeDropdown.value;
            _configuration.Directory           = _directoryInputField.text;
            _configuration.WorkingDirectory    = _workingDirectoryInputField.text;
            _configuration.Executable          = _configuration.InteractionType == InteractionType.GameInternal
                                               ? _coreDropdown.options[_coreDropdown.value].text
                                               : _executableInputField.text;
            _configuration.Arguments           = _argumentsInputField.text;
            _configuration.SupportedExtensions = _supportedExtensionsInputField.text;
            _configuration.GamesDirectories    = _gamesDirectories.GetValues();
        }

        protected override void ClearUIValues()
        {
            _interactionTypeDropdown.value = 0;

            _directoryButton.onClick.RemoveAllListeners();
            _directoryInputField.text          = "";
            _directoryInputField.caretPosition = 0;

            _workingDirectoryButton.onClick.RemoveAllListeners();
            _workingDirectoryInputField.text          = "";
            _workingDirectoryInputField.caretPosition = 0;

            _executableButton.onClick.RemoveAllListeners();
            _executableInputField.text          = "";
            _executableInputField.caretPosition = 0;

            _argumentsInputField.text          = "";
            _argumentsInputField.caretPosition = 0;

            _supportedExtensionsInputField.text          = "";
            _supportedExtensionsInputField.caretPosition = 0;

            _gamesDirectories.Clear();
        }

        private void SwitchToLibretroSetup()
        {
            _directoryButton.interactable     = false;
            _directoryInputField.interactable = false;

            _workingDirectoryButton.interactable     = false;
            _workingDirectoryInputField.interactable = false;

            _executableDescriptionText.SetText("Core");
            _executableButton.gameObject.SetActive(false);
            _executableInputField.gameObject.SetActive(false);

            _coreDropdown.ClearOptions();
            _coreDropdown.AddOptions(new List<string> { "" });
            if (FileSystemUtils.TryGetFiles(FileSystemUtils.PathCombine(SystemUtils.GetDataPath(), "libretro~/cores"), "*.dll", false, out string[] fileNames))
                _coreDropdown.AddOptions(fileNames.Select(x => Path.GetFileNameWithoutExtension(x).Replace("_libretro", "")).ToList());
            _coreDropdown.value = _coreDropdown.options.FindIndex(x => x.text == _configuration.Executable);
            _coreDropdown.gameObject.SetActive(true);

            _argumentsInputField.interactable = false;

            _supportedExtensionsInputField.interactable = false;
        }

        private void SwitchToExternalSetup()
        {
            _directoryButton.interactable     = true;
            _directoryInputField.interactable = true;

            _workingDirectoryButton.interactable     = true;
            _workingDirectoryInputField.interactable = true;

            _coreDropdown.gameObject.SetActive(false);

            _executableDescriptionText.SetText("Executable");
            _executableButton.gameObject.SetActive(true);
            _executableInputField.gameObject.SetActive(true);

            _argumentsInputField.interactable = true;

            _supportedExtensionsInputField.interactable = true;
        }
    }
}
