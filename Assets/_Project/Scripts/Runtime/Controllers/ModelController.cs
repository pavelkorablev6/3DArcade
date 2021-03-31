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
    public abstract class ModelController
    {
        protected abstract Vector3 ModelPosition { get; }
        protected abstract Quaternion ModelOrientation { get; }

        protected readonly ModelConfiguration _modelConfiguration;
        protected readonly IModelNameProvider _modelNameProvider;

        private readonly Transform _parent;

        protected ModelController(ModelConfiguration modelConfiguration,
                                  Transform parent,
                                  IModelNameProvider modelNameProvider)
        {
            _modelConfiguration = modelConfiguration;
            _parent             = parent;
            _modelNameProvider  = modelNameProvider;
        }

        protected void SpawnModel()
        {
            IEnumerable<string> namesToTry = GetNamesToTry();
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                SetupEntityOutsidePlayMode(namesToTry);
                return;
            }
#endif
            Addressables.LoadResourceLocationsAsync(namesToTry, Addressables.MergeMode.UseFirst, typeof(GameObject)).Completed += ResourceLocationsRetrievedCallback;
        }

        protected abstract IEnumerable<string> GetNamesToTry();

        protected abstract void SetupArtworks();

        private void ResourceLocationsRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        {
            if (aoHandle.Result.Count == 0)
                return;

            Addressables.InstantiateAsync(aoHandle.Result[0], ModelPosition, ModelOrientation, _parent).Completed += InstantiatedCallback;
        }

        private void InstantiatedCallback(AsyncOperationHandle<GameObject> aoHandle)
        {
            if (aoHandle.Status == AsyncOperationStatus.Succeeded)
                SetupModel(aoHandle.Result);
        }

        private void SetupModel(GameObject gameObject)
        {
            gameObject.name                 = _modelConfiguration.Id;
            gameObject.transform.localScale = _modelConfiguration.Scale;
            gameObject.AddComponent<ModelConfigurationComponent>()
                      .SetModelConfiguration(_modelConfiguration);

            // Look for artworks only in play mode / runtime
            if (Application.isPlaying)
                SetupArtworks();

            //PlatformConfiguration platform = null;
            //if (!string.IsNullOrEmpty(modelConfiguration.Platform))
            //    platform = _platformDatabase.Get(modelConfiguration.Platform);

            //GameConfiguration game = null;
            //if (platform != null && platform.MasterList != null)
            //{
            //    game = _gameDatabase.Get(modelConfiguration.platform.MasterList, game.Id);
            //    if (game != null)
            //    {
            //    }
            //}

            //_marqueeNodeController.Setup(this, instantiatedModel, modelConfiguration, renderSettings.MarqueeIntensity);
            //_screenNodeController.Setup(this, instantiatedModel, modelConfiguration, GetScreenIntensity(game, renderSettings));
            //_genericNodeController.Setup(this, instantiatedModel, modelConfiguration, 1f);

            //if (gameModels)
            //{
            //    _allGames.Add(instantiatedModel.transform);
            //    AddModelsToWorldAdditionalLoopStepsForGames(instantiatedModel);
            //}
            //else
            //    AddModelsToWorldAdditionalLoopStepsForProps(instantiatedModel);
        }

#if UNITY_EDITOR
        private void SetupEntityOutsidePlayMode(IEnumerable<string> namesToTry)
        {
            foreach (string nameToTry in namesToTry)
            {
                try
                {
                    GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(nameToTry);
                    if (prefab != null)
                    {
                        GameObject gameObject = Object.Instantiate(prefab, ModelPosition, ModelOrientation, _parent);
                        SetupModel(gameObject);
                        return;
                    }
                }
                catch
                {
                }
            }
        }
#endif
    }
}
