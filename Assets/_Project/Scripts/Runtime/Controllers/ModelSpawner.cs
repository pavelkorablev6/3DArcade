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

namespace Arcade
{
    public sealed class ModelSpawner : IModelSpawner
    {
        private Vector3 _position;
        private Quaternion _orientation;
        private Transform _parent;
        private System.Action<GameObject> _onComplete;

        void IModelSpawner.Spawn(IEnumerable<string> namesToTry, Vector3 position, Quaternion orientation, Transform parent, System.Action<GameObject> onComplete)
        {
            _position    = position;
            _orientation = orientation;
            _parent      = parent;
            _onComplete  = onComplete;

            Addressables.LoadResourceLocationsAsync(namesToTry, Addressables.MergeMode.UseFirst, typeof(GameObject)).Completed += ResourceLocationsRetrievedCallback;
        }

        private void ResourceLocationsRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        {
            if (aoHandle.Result.Count == 0)
                return;

            Addressables.InstantiateAsync(aoHandle.Result[0], _position, _orientation, _parent).Completed += InstantiatedCallback;
        }

        private void InstantiatedCallback(AsyncOperationHandle<GameObject> aoHandle)
        {
            if (aoHandle.Status == AsyncOperationStatus.Succeeded)
                _onComplete?.Invoke(aoHandle.Result);
        }
    }
}
