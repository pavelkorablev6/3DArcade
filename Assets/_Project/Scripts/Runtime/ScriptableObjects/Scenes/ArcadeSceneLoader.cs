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
using SK.Utilities.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Arcade
{
    [CreateAssetMenu(menuName = "3DArcade/ArcadeSceneLoader", fileName = "ArcadeSceneLoader")]
    public sealed class ArcadeSceneLoader : AddressableSceneLoaderBase
    {
        private static int _arcadeModelsLayer = 0;

        [System.NonSerialized] private AsyncOperationHandle<SceneInstance> _sceneHandle;
        [System.NonSerialized] private SceneInstance _sceneInstance;

        public override async UniTask<bool> Load(AssetAddresses addressesToTry, bool triggerReload)
        {
            if (_arcadeModelsLayer == 0)
                _arcadeModelsLayer = LayerMask.NameToLayer("Arcade/ArcadeModels");

            IList<IResourceLocation> resourceLocations = await Addressables.LoadResourceLocationsAsync(addressesToTry, Addressables.MergeMode.UseFirst, typeof(SceneInstance));
            if (resourceLocations.Count == 0)
                return false;

            if (triggerReload)
            {
                if (_sceneInstance.Scene.IsValid() && _sceneInstance.Scene.isLoaded)
                    _ = await Addressables.UnloadSceneAsync(_sceneInstance);
                return await Load(addressesToTry, false);
            }

            _sceneHandle   = Addressables.LoadSceneAsync(resourceLocations[0], LoadSceneMode.Additive);
            _sceneInstance = await _sceneHandle;

            if (_sceneHandle.Status != AsyncOperationStatus.Succeeded)
                return false;

            GameObject[] rootObjects = _sceneInstance.Scene.GetRootGameObjects();
            foreach (GameObject gameObject in rootObjects)
            {
                if (gameObject.TryGetComponent(out Collider _))
                    gameObject.transform.SetLayerRecursively(_arcadeModelsLayer);

                Collider[] colliders = gameObject.GetComponentsInChildren<Collider>(true);
                foreach (Collider collider in colliders)
                    collider.transform.SetLayerRecursively(_arcadeModelsLayer);
            }

            _ = SceneManager.SetActiveScene(_sceneInstance.Scene);
            return true;
        }
    }
}
