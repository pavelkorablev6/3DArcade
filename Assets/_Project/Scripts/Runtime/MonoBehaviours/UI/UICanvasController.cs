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
    [DisallowMultipleComponent]
    public sealed class UICanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingUI;
        [SerializeField] private GameObject _normalUI;
        [SerializeField] private GameObject _moveCabUI;
        [SerializeField] private GameObject _configurationUI;

        public void EnableSceneLoadingUI() => _loadingUI.SetActive(true);

        public void EnableSceneNormalUI() => _normalUI.SetActive(true);

        public void EnableSceneEditModeUI() => _moveCabUI.SetActive(true);

        public void EnableConfigurationUI() => _configurationUI.SetActive(true);

        public void DisableSceneLoadingUI() => _loadingUI.SetActive(false);

        public void DisableSceneNormalUI() => _normalUI.SetActive(false);

        public void DisableSceneEditModeUI() => _moveCabUI.SetActive(false);

        public void DisableConfigurationUI() => _configurationUI.SetActive(false);

        public void Disable()
        {
            DisableSceneLoadingUI();
            DisableSceneNormalUI();
            DisableSceneEditModeUI();
            DisableConfigurationUI();
        }
    }
}
