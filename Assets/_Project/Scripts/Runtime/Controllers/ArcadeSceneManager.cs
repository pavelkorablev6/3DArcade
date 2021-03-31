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
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Arcade
{
    public sealed class ArcadeSceneManager
    {
        public System.Action<string> Started;
        public System.Action Completed;

        public bool IsSceneLoading => _sceneHandle.IsValid() && !_sceneHandle.IsDone;
        public float LoadingPercentCompleted => _sceneHandle.PercentComplete;

        private AsyncOperationHandle<SceneInstance> _sceneHandle;
        private IResourceLocation _sceneResourceLocation;
        private SceneInstance _sceneInstance;

        private bool _triggerSceneReload;

        public void LoadScene(IEnumerable<string> namesToTry)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                LoadSceneInEditor(namesToTry);
                return;
            }
#endif
            Addressables.LoadResourceLocationsAsync(namesToTry, Addressables.MergeMode.UseFirst, typeof(SceneInstance)).Completed += SceneResourceLocationRetrievedCallback;
        }

        public void UnloadScene()
        {
            if (_sceneInstance.Scene.isLoaded)
                Addressables.UnloadSceneAsync(_sceneInstance).Completed += SceneUnloadedCallback;
        }

        private void SceneResourceLocationRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        {
            if (aoHandle.Result.Count == 0)
                return;

            _sceneResourceLocation = aoHandle.Result[0];
            LoadScene();
        }

        private void LoadScene()
        {
            Started?.Invoke(_sceneResourceLocation.ToString());

            if (_sceneInstance.Scene.isLoaded)
            {
                _triggerSceneReload = true;
                UnloadScene();
            }
            else
            {
                _sceneHandle = Addressables.LoadSceneAsync(_sceneResourceLocation, LoadSceneMode.Additive);
                _sceneHandle.Completed += SceneLoadedCallback;
            }
        }

        private void SceneLoadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        {
            if (aoHandle.Status != AsyncOperationStatus.Succeeded)
                return;

            _sceneInstance = aoHandle.Result;

            Completed?.Invoke();

            _ = SceneManager.SetActiveScene(_sceneInstance.Scene);
        }

        private void SceneUnloadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        {
            if (_triggerSceneReload)
                LoadScene();

            _triggerSceneReload = false;
        }

#if UNITY_EDITOR
        public void LoadSceneInEditor(IEnumerable<string> namesToTry)
        {
            foreach (string nameToTry in namesToTry)
            {
                System.Type assetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(nameToTry);
                if (assetType == typeof(UnityEditor.SceneAsset))
                {
                    Scene scene = EditorSceneManager.OpenScene(nameToTry, OpenSceneMode.Additive);
                    Completed?.Invoke();
                    _ = SceneManager.SetActiveScene(scene);
                    UnityEditor.SceneVisibilityManager.instance.DisablePicking(scene);
                    return;
                }
            }
        }
#endif
    }
}
