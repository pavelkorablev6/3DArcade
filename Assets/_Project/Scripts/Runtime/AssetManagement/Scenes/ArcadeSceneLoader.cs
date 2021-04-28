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

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Arcade
{
    public sealed class ArcadeSceneLoader : ArcadeSceneLoaderBase
    {
        public override float LoadPercentCompleted => _sceneHandle.IsValid() && !_sceneHandle.IsDone ? _sceneHandle.PercentComplete : 0f;

        private AsyncOperationHandle<SceneInstance> _sceneHandle;
        private SceneInstance _sceneInstance;

        public override async UniTask<bool> Load(AssetAddresses addressesToTry, bool triggerReload)
        {
            IList<IResourceLocation> resourceLocations = await Addressables.LoadResourceLocationsAsync(addressesToTry, Addressables.MergeMode.UseFirst, typeof(SceneInstance));
            if (resourceLocations.Count == 0)
                return false;

            if (_sceneInstance.Scene.IsValid() && _sceneInstance.Scene.isLoaded)
            {
                _ = await Addressables.UnloadSceneAsync(_sceneInstance);
                if (triggerReload)
                    return await Load(addressesToTry, false);
            }

            _sceneHandle   = Addressables.LoadSceneAsync(resourceLocations[0], LoadSceneMode.Additive);
            _sceneInstance = await _sceneHandle;

            if (_sceneHandle.Status != AsyncOperationStatus.Succeeded)
                return false;

            await _sceneInstance.ActivateAsync();
            return true;
        }

        //public override void Load(AssetAddresses addressesToTry, System.Action onComplete)
        //{
        //    Loaded = false;

        //    if (_sceneHandle.IsValid())
        //        Addressables.Release(_sceneHandle);

        //    _onComplete = onComplete;

        //    Addressables.LoadResourceLocationsAsync(addressesToTry, Addressables.MergeMode.UseFirst, typeof(SceneInstance))
        //                .Completed += ResourceLocationRetrievedCallback;
        //}

        //private void ResourceLocationRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        //{
        //    if (aoHandle.Result.Count == 0)
        //        return;

        //    _sceneResourceLocation = aoHandle.Result[0];
        //    LoadScene();
        //}

        //private void LoadScene()
        //{
        //    if (_sceneInstance.Scene.isLoaded)
        //    {
        //        _triggerSceneReload = true;
        //        UnloadScene();
        //        return;
        //    }

        //    _sceneHandle = Addressables.LoadSceneAsync(_sceneResourceLocation, LoadSceneMode.Additive);
        //    _sceneHandle.Completed += SceneLoadedCallback;
        //}

        //private void UnloadScene() => Addressables.UnloadSceneAsync(_sceneInstance).Completed += SceneUnloadedCallback;

        //private void SceneLoadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        //{
        //    if (aoHandle.Status != AsyncOperationStatus.Succeeded)
        //        return;

        //    _sceneInstance = aoHandle.Result;

        //    _onComplete?.Invoke();

        //    _ = SceneManager.SetActiveScene(_sceneInstance.Scene);

        //    Loaded = true;
        //}

        //private void SceneUnloadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        //{
        //    if (!_triggerSceneReload)
        //        return;

        //    LoadScene();
        //    _triggerSceneReload = false;
        //}
    }
}
