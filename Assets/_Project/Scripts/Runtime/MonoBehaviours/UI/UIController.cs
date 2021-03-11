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

using TMPro;
using UnityEngine;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIController : MonoBehaviour, IUIController
    {
        [SerializeField] private GameObject _loadingCanvas;
        [SerializeField] private GameObject _normalCanvas;
        [SerializeField] private GameObject _moveCabCanvas;

        [SerializeField] private TMP_Text _progressMessage;
        [SerializeField] private RectTransform _progressTransform;
        [SerializeField] private TMP_Text _progressPercent;

        private void Start() => SetState(UIState.None);

        public void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.SceneLoading:
                    EnableSceneLoadingUI();
                    DisableSceneNormalUI();
                    DisableSceneEditModeUI();
                    break;
                case UIState.SceneNormal:
                    DisableSceneLoadingUI();
                    EnableSceneNormalUI();
                    DisableSceneEditModeUI();
                    break;
                case UIState.SceneEditMode:
                    DisableSceneLoadingUI();
                    DisableSceneNormalUI();
                    EnableSceneEditModeUI();
                    break;
                case UIState.None:
                default:
                    DisableSceneLoadingUI();
                    DisableSceneNormalUI();
                    DisableSceneEditModeUI();
                    break;
            }
        }

        public void InitStatusBar(string message)
        {
            _progressMessage.SetText(message);
            _progressTransform.localScale = new Vector3(0f, 1f, 1f);
            _progressPercent.SetText("0%");
        }

        public void UpdateStatusBar(float percentComplete)
        {
            _progressTransform.localScale = new Vector3(percentComplete, 1f, 1f);
            _progressPercent.SetText($"{percentComplete * 100:0}%");
        }

        public void ResetStatusBar()
        {
            _progressMessage.SetText(string.Empty);
            _progressTransform.localScale = new Vector3(0f, 1f, 1f);
            _progressPercent.SetText(string.Empty);
        }

        private void EnableSceneLoadingUI()
        {
            if (_loadingCanvas != null)
                _loadingCanvas.SetActive(true);
        }

        private void EnableSceneNormalUI()
        {
            if (_normalCanvas != null)
                _normalCanvas.SetActive(true);
        }

        private void EnableSceneEditModeUI()
        {
            if (_moveCabCanvas != null)
                _moveCabCanvas.SetActive(true);
        }

        private void DisableSceneLoadingUI()
        {
            if (_loadingCanvas != null)
                _loadingCanvas.SetActive(false);
        }

        private void DisableSceneNormalUI()
        {
            if (_normalCanvas != null)
                _normalCanvas.SetActive(false);
        }

        private void DisableSceneEditModeUI()
        {
            if (_moveCabCanvas != null)
                _moveCabCanvas.SetActive(false);
        }
    }
}
