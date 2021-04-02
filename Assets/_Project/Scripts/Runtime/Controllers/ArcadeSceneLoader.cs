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

using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Arcade
{
    public sealed class ArcadeSceneLoader : IArcadeSceneLoader
    {
        public bool Loaded => _sceneInstance.Scene.IsValid() && _sceneInstance.Scene.isLoaded;
        public bool Loading => _sceneHandle.IsValid() && !_sceneHandle.IsDone;
        public float LoadPercentCompleted => Loading ? _sceneHandle.PercentComplete : 0f;

        private System.Action _onComplete;
        private IResourceLocation _sceneResourceLocation;
        private AsyncOperationHandle<SceneInstance> _sceneHandle;
        private SceneInstance _sceneInstance;
        private bool _triggerSceneReload;

        public void Load(IEnumerable<string> namesToTry, System.Action onComplete)
        {
            _onComplete = onComplete;
            Addressables.LoadResourceLocationsAsync(namesToTry, Addressables.MergeMode.UseFirst, typeof(SceneInstance))
                        .Completed += ResourceLocationRetrievedCallback;
        }

        private void ResourceLocationRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        {
            if (aoHandle.Result.Count == 0)
                return;

            _sceneResourceLocation = aoHandle.Result[0];
            LoadScene();
        }

        private void LoadScene()
        {
            if (_sceneInstance.Scene.isLoaded)
            {
                _triggerSceneReload = true;
                Addressables.UnloadSceneAsync(_sceneInstance).Completed += SceneUnloadedCallback;
                return;
            }

            _sceneHandle = Addressables.LoadSceneAsync(_sceneResourceLocation, LoadSceneMode.Additive);
            _sceneHandle.Completed += SceneLoadedCallback;
        }

        private void SceneLoadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        {
            if (aoHandle.Status != AsyncOperationStatus.Succeeded)
                return;

            _sceneInstance = aoHandle.Result;

            _onComplete?.Invoke();

            _ = SceneManager.SetActiveScene(_sceneInstance.Scene);
        }

        private void SceneUnloadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        {
            if (_triggerSceneReload)
                LoadScene();
            _triggerSceneReload = false;
        }
    }
}
