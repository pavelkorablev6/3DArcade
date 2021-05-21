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

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIGamesDirectories : MonoBehaviour
    {
        [SerializeField] private RectTransform _gamesDirectoriesValues;
        [SerializeField] private UIGamesDirectory _uiGamesDirectoryPrefab;

        private RectTransform _transform;
        private float _initialHeight;
        private int _valuesCount = 0;

        private void Awake()
        {
            _transform     = transform as RectTransform;
            _initialHeight = _transform.rect.height;
        }

        public void AddDirectory(FileExplorer fileExplorer, string path)
        {
            UIGamesDirectory uiGamesDirectory = Instantiate(_uiGamesDirectoryPrefab, _gamesDirectoriesValues);
            uiGamesDirectory.Initialize(this, fileExplorer, path);
            ++_valuesCount;
        }

        public void RemoveDirectory(UIGamesDirectory directory)
        {
            Destroy(directory.gameObject);
            --_valuesCount;
            AdjustHeight();
        }

        public void Clear()
        {
            for (int i = _gamesDirectoriesValues.childCount - 1; i >= 0; --i)
                Destroy(_gamesDirectoriesValues.GetChild(i).gameObject);

            _transform.sizeDelta = new Vector2(_transform.rect.width, _initialHeight);
            gameObject.SetActive(false);
            gameObject.SetActive(true);

            _valuesCount = 0;
        }

        public void AdjustHeight()
        {
            float height = _valuesCount * _initialHeight;

            _transform.sizeDelta = new Vector2(_transform.rect.width, height);

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public string[] GetValues()
        {
            if (_gamesDirectoriesValues.childCount == 0)
                return new string[0];
            return _gamesDirectoriesValues.GetComponentsInChildren<UIGamesDirectory>().Select(x => x.InputFieldText).ToArray();
        }
    }
}
