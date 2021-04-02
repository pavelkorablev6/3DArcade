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

using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using Zenject;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIController : MonoBehaviour, IUIController
    {
        [SerializeField] private GameObject _loadingCanvas;
        [SerializeField] private GameObject _normalCanvas;
        [SerializeField] private GameObject _moveCabCanvas;
        [SerializeField] private GameObject _configurationCanvas;

        [SerializeField] private TMP_Text _progressMessage;
        [SerializeField] private RectTransform _progressTransform;
        [SerializeField] private TMP_Text _progressPercent;

        public UIContext UIContext { get; private set; }

        [Inject, SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "DI")]
        private void Construct(UIContext uiContext) => UIContext = uiContext;

        public void TransitionTo<T>() where T : UIState => UIContext.TransitionTo<T>();

        public void EnableSceneLoadingUI() => _loadingCanvas.SetActive(true);

        public void EnableSceneNormalUI() => _normalCanvas.SetActive(true);

        public void EnableSceneEditModeUI() => _moveCabCanvas.SetActive(true);

        public void EnableConfigurationUI() => _configurationCanvas.SetActive(true);

        public void DisableSceneLoadingUI() => _loadingCanvas.SetActive(false);

        public void DisableSceneNormalUI() => _normalCanvas.SetActive(false);

        public void DisableSceneEditModeUI() => _moveCabCanvas.SetActive(false);

        public void DisableConfigurationUI() => _configurationCanvas.SetActive(false);

        public void DisableAll()
        {
            DisableSceneLoadingUI();
            DisableSceneNormalUI();
            DisableSceneEditModeUI();
            DisableConfigurationUI();
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
            _progressMessage.Clear();
            _progressTransform.localScale = new Vector3(0f, 1f, 1f);
            _progressPercent.Clear();
        }

#if UNITY_EDITOR
        [ContextMenu("Loading State")]
        private void Context_LoadingState()
        {
            DisableSceneNormalUI();
            DisableSceneEditModeUI();
            DisableConfigurationUI();

            EnableSceneLoadingUI();
        }

        [ContextMenu("Normal State")]
        private void Context_NormalState()
        {
            DisableSceneLoadingUI();
            DisableSceneEditModeUI();
            DisableConfigurationUI();

            EnableSceneNormalUI();
        }

        [ContextMenu("MoveCab State")]
        private void Context_MoveCabState()
        {
            DisableSceneLoadingUI();
            DisableSceneNormalUI();
            DisableConfigurationUI();

            EnableSceneEditModeUI();
        }

        [ContextMenu("Configuration State")]
        private void Context_ConfigurationState()
        {
            DisableSceneLoadingUI();
            DisableSceneNormalUI();
            DisableSceneEditModeUI();

            EnableConfigurationUI();
        }

        [ContextMenu("Disabled State")]
        private void Context_DisableAll() => DisableAll();
#endif
    }
}
