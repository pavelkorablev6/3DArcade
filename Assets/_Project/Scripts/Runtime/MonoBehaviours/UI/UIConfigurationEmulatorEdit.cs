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
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIConfigurationEmulatorEdit : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text TitleText { get; private set; }

        [SerializeField] private EmulatorsDatabase _emulatorsDatabase;
        [SerializeField] private FileExplorer _fileExplorer;

        [SerializeField] private TMP_InputField _descriptionInputField;
        [SerializeField] private TMP_Dropdown _interactionTypeDropdown;
        [SerializeField] private Button _directoryButton;
        [SerializeField] private TMP_InputField _directoryInputField;
        [SerializeField] private Button _workingDirectoryButton;
        [SerializeField] private TMP_InputField _workingDirectoryInputField;
        [SerializeField] private Button _executableButton;
        [SerializeField] private TMP_InputField _executableInputField;
        [SerializeField] private TMP_InputField _argumentsInputField;
        [SerializeField] private TMP_InputField _supportedExtensionsInputField;
        [SerializeField] private Button _gamesDirectoriesAddButton;
        [SerializeField] private UIGamesDirectories _gamesDirectories;

        private RectTransform _transform;
        private EmulatorConfiguration _configuration;

        private void Awake() => _transform = transform as RectTransform;

        public void Show(EmulatorConfiguration configuration)
        {
            _configuration = configuration;

            _descriptionInputField.text      = configuration.Description;
            _interactionTypeDropdown.value   = configuration.InteractionType <= InteractionType.URL ? (int)configuration.InteractionType : 0;
            _directoryInputField.text        = configuration.Directory;
            _workingDirectoryInputField.text = configuration.WorkingDirectory;
            _executableInputField.text       = configuration.Executable;
            _argumentsInputField.text        = configuration.Arguments;

            if (configuration.SupportedExtensions.Length > 0)
                _supportedExtensionsInputField.text = string.Join(";", configuration.SupportedExtensions);

            if (configuration.GamesDirectories.Length > 0)
            {
                for (int i = 0; i < configuration.GamesDirectories.Length; ++i)
                    _gamesDirectories.AddDirectory(_fileExplorer, configuration.GamesDirectories[i]);
                _gamesDirectories.AdjustHeight();
            }

            _directoryButton.onClick.AddListener(()
                => _fileExplorer.OpenSingleDirectoryDialog(paths =>
                {
                    if (paths != null && paths.Length > 0)
                        _directoryInputField.text = paths[0];
                }));

            _workingDirectoryButton.onClick.AddListener(()
                => _fileExplorer.OpenSingleDirectoryDialog(paths =>
                {
                    if (paths != null && paths.Length > 0)
                        _workingDirectoryInputField.text = paths[0];
                }));

            _executableButton.onClick.AddListener(()
                => _fileExplorer.OpenSingleFileDialog(paths =>
                {
                    if (paths == null || paths.Length == 0)
                        return;
                    if (string.IsNullOrEmpty(_directoryInputField.text))
                        _directoryInputField.text = Path.GetDirectoryName(paths[0]);
                    _executableInputField.text = Path.GetFileName(paths[0]);
                }));

            _gamesDirectoriesAddButton.onClick.AddListener(()
                => _fileExplorer.OpenSingleDirectoryDialog(paths =>
                {
                    if (paths == null || paths.Length == 0)
                        return;
                    _gamesDirectories.AddDirectory(_fileExplorer, paths[0]);
                    _gamesDirectories.AdjustHeight();
                }));

            _ = _transform.DOAnchorPosX(0f, 0.3f);
        }

        public void SaveAndHide()
        {
            _configuration.Description         = _descriptionInputField.text;
            _configuration.InteractionType     = (InteractionType)_interactionTypeDropdown.value;
            _configuration.Directory           = _directoryInputField.text;
            _configuration.WorkingDirectory    = _workingDirectoryInputField.text;
            _configuration.Executable          = _executableInputField.text;
            _configuration.Arguments           = _argumentsInputField.text;
            _configuration.SupportedExtensions = _supportedExtensionsInputField.text;
            _configuration.GamesDirectories    = _gamesDirectories.GetValues();

            _ = _emulatorsDatabase.Save(_configuration);

            Hide();
        }

        public void Hide()
        {
            _descriptionInputField.text          = "";
            _interactionTypeDropdown.value       = 0;
            _directoryInputField.text            = "";
            _workingDirectoryInputField.text     = "";
            _executableInputField.text           = "";
            _argumentsInputField.text            = "";
            _supportedExtensionsInputField.text  = "";
            _gamesDirectories.Clear();

            _directoryButton.onClick.RemoveAllListeners();
            _workingDirectoryButton.onClick.RemoveAllListeners();
            _executableButton.onClick.RemoveAllListeners();
            _gamesDirectoriesAddButton.onClick.RemoveAllListeners();

            _configuration = null;

            _ = _transform.DOAnchorPosX(-500f, 0.3f);
        }
    }
}
