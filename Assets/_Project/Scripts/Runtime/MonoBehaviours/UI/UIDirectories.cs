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

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIDirectories : MonoBehaviour
    {
        [SerializeField] private Button _addButton;
        [SerializeField] private RectTransform _directoryValues;
        [SerializeField] private UIDirectory _uiDirectoryPrefab;

        private RectTransform _transform;
        private float _initialHeight;
        private int _valuesCount = 0;

        private void Awake()
        {
            _transform     = transform as RectTransform;
            _initialHeight = _transform.rect.height;
        }

        public void Init(FileExplorer fileExplorer, string[] paths)
        {
            _addButton.onClick.RemoveAllListeners();
            _addButton.onClick.AddListener(()
                => fileExplorer.OpenDirectoryDialog(paths =>
                {
                    if (paths is null || paths.Length == 0)
                        return;
                    AddDirectory(fileExplorer, paths[0]);
                    AdjustHeight();
                }));

            if (paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; ++i)
                    AddDirectory(fileExplorer, paths[i]);
                AdjustHeight();
            }
        }

        public void AddDirectory(FileExplorer fileExplorer, string path)
        {
            UIDirectory uiDirectory = Instantiate(_uiDirectoryPrefab, _directoryValues);
            uiDirectory.Initialize(this, fileExplorer, path);
            ++_valuesCount;
        }

        public void MoveDirectoryUp(UIDirectory directory)
        {
            int index = directory.transform.GetSiblingIndex();
            if (index > 0)
                directory.transform.SetSiblingIndex(index - 1);
        }

        public void MoveDirectoryDown(UIDirectory directory)
        {
            int index = directory.transform.GetSiblingIndex();
            if (index < _directoryValues.childCount - 1)
                directory.transform.SetSiblingIndex(index + 1);
        }

        public void RemoveDirectory(UIDirectory directory)
        {
            Destroy(directory.gameObject);
            --_valuesCount;
            AdjustHeight();
        }

        public void Clear()
        {
            _addButton.onClick.RemoveAllListeners();

            for (int i = _directoryValues.childCount - 1; i >= 0; --i)
                Destroy(_directoryValues.GetChild(i).gameObject);
            _valuesCount = 0;

            _transform.sizeDelta = new Vector2(_transform.rect.width, _initialHeight);
            // Force canvas refresh...
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void AdjustHeight()
        {
            float height = _valuesCount > 0 ?_valuesCount * _initialHeight : _initialHeight;

            _transform.sizeDelta = new Vector2(_transform.rect.width, height);
            // Force canvas refresh...
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public string[] GetValues()
        {
            if (_directoryValues.childCount == 0)
                return new string[0];
            return _directoryValues.GetComponentsInChildren<UIDirectory>().Select(x => x.InputFieldText).ToArray();
        }
    }
}
