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

using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Zenject;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIManager : MonoBehaviour
    {
        [SerializeField] private UICanvasController _normalUI;
        [SerializeField] private UICanvasController _virtualRealityUI;

        private UIContext _uiContext;

        [Inject, SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "DI")]
        private void Construct(UIContext uiContext) => _uiContext = uiContext;

        private void Start() => TransitionTo<UIDisabledState>();

        public void TransitionTo<T>() where T : UIState => _uiContext.TransitionTo<T>();

        public void InitStatusBar(string message)
        {
            _normalUI.InitStatusBar(message);
            _virtualRealityUI.InitStatusBar(message);
        }

        public void UpdateStatusBar(float percentComplete)
        {
            _normalUI.UpdateStatusBar(percentComplete);
            _virtualRealityUI.UpdateStatusBar(percentComplete);
        }

        public void EnableNormalSceneLoadingUI() => _normalUI.EnableSceneLoadingUI();
        public void EnableNormalSceneNormalUI() => _normalUI.EnableSceneNormalUI();
        public void EnableNormalSceneEditModeUI() => _normalUI.EnableSceneEditModeUI();
        public void EnableNormalConfigurationUI() => _normalUI.EnableConfigurationUI();
        public void DisableNormalUI() => _normalUI.Disable();

        public void EnableVirtualRealitySceneLoadingUI() => _virtualRealityUI.EnableSceneLoadingUI();
        public void EnableVirtualRealitySceneNormalUI() => _virtualRealityUI.EnableSceneNormalUI();
        public void EnableVirtualRealitySceneEditModeUI() => _virtualRealityUI.EnableSceneEditModeUI();
        public void EnableVirtualRealityConfigurationUI() => _virtualRealityUI.EnableConfigurationUI();
        public void DisableVirtualRealityUI() => _virtualRealityUI.Disable();

        public void Disable()
        {
            DisableNormalUI();
            DisableVirtualRealityUI();
        }
    }
}
