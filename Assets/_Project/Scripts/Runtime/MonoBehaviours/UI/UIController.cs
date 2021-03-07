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

using UnityEngine;

namespace Arcade
{
    public sealed class UIController : IUIController
    {
        private readonly GameObject _sceneLoadingUI;
        private readonly GameObject _sceneNormalUI;
        private readonly GameObject _sceneEditModeUI;

        public UIController()
        {
            UISceneLoadingCanvasTag sceneLoadingCanvasTag = Object.FindObjectOfType<UISceneLoadingCanvasTag>(true);
            if (sceneLoadingCanvasTag != null)
                _sceneLoadingUI = sceneLoadingCanvasTag.gameObject;

            UISceneNormalCanvasTag sceneNormalCanvasTag = Object.FindObjectOfType<UISceneNormalCanvasTag>(true);
            if (sceneNormalCanvasTag != null)
                _sceneNormalUI = sceneNormalCanvasTag.gameObject;

            UISceneMoveCabCanvasTag sceneEditModeCanvasTag = Object.FindObjectOfType<UISceneMoveCabCanvasTag>(true);
            if (sceneEditModeCanvasTag != null)
                _sceneEditModeUI = sceneEditModeCanvasTag.gameObject;

            SetState(UIState.None);
        }

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

        private void EnableSceneLoadingUI()
        {
            if (_sceneLoadingUI != null)
                _sceneLoadingUI.SetActive(true);
        }

        private void EnableSceneNormalUI()
        {
            if (_sceneNormalUI != null)
                _sceneNormalUI.SetActive(true);
        }

        private void EnableSceneEditModeUI()
        {
            if (_sceneEditModeUI != null)
                _sceneEditModeUI.SetActive(true);
        }

        private void DisableSceneLoadingUI()
        {
            if (_sceneLoadingUI != null)
                _sceneLoadingUI.SetActive(false);
        }

        private void DisableSceneNormalUI()
        {
            if (_sceneNormalUI != null)
                _sceneNormalUI.SetActive(false);
        }

        private void DisableSceneEditModeUI()
        {
            if (_sceneEditModeUI != null)
                _sceneEditModeUI.SetActive(false);
        }
    }
}
