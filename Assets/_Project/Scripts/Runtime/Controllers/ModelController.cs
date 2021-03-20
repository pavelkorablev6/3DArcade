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

using System;
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

        private readonly Transform _parent;

        public ModelController(ModelConfiguration modelConfiguration, Transform parent, MultiFileDatabase<PlatformConfiguration> platformDatabase, ModelMatcher modelMatcher)
        {
            _modelConfiguration = modelConfiguration;
            _parent             = parent;

            PlatformConfiguration platform = !string.IsNullOrEmpty(modelConfiguration.Platform) ? platformDatabase.Get(modelConfiguration.Platform) : null;
            GameConfiguration game         = /*platform != null && !string.IsNullOrEmpty(platform.MasterList) ? gameDatabase.Get(platform.MasterList) : */null;
            List<string> namesToTry        = modelMatcher.GetNamesToTryForGame(_modelConfiguration, platform, game);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                LoadInEditorGame(namesToTry);
                return;
            }
#endif
            Addressables.LoadResourceLocationsAsync(namesToTry, Addressables.MergeMode.UseFirst, typeof(GameObject)).Completed += GameResourceLocationsRetrievedCallback;
        }

        private void GameResourceLocationsRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        {
            if (aoHandle.Result.Count == 0)
                return;

            Addressables.InstantiateAsync(aoHandle.Result[0], ModelPosition, ModelOrientation, _parent).Completed += GameInstantiatedCallback;
        }

        private void GameInstantiatedCallback(AsyncOperationHandle<GameObject> aoHandle)
        {
            if (aoHandle.Status == AsyncOperationStatus.Succeeded)
                SetupModel(aoHandle.Result);
        }

        private void SetupModel(GameObject gameObject)
        {
            gameObject.name                 = _modelConfiguration.Id;
            gameObject.transform.localScale = _modelConfiguration.Scale;
            gameObject.AddComponent<ModelConfigurationComponent>()
                      .FromModelConfiguration(_modelConfiguration);

            // Look for artworks only in play mode / runtime
            //if (Application.isPlaying)
            //{
            //    PlatformConfiguration platform = null;
            //    //if (!string.IsNullOrEmpty(modelConfiguration.Platform))
            //    //    platform = _platformDatabase.Get(modelConfiguration.Platform);

            //    GameConfiguration game = null;
            //    if (platform != null && platform.MasterList != null)
            //    {
            //        //game = _gameDatabase.Get(modelConfiguration.platform.MasterList, game.Id);
            //        if (game != null)
            //        {
            //        }
            //    }

            //    _marqueeNodeController.Setup(this, instantiatedModel, modelConfiguration, renderSettings.MarqueeIntensity);
            //    _screenNodeController.Setup(this, instantiatedModel, modelConfiguration, GetScreenIntensity(game, renderSettings));
            //    _genericNodeController.Setup(this, instantiatedModel, modelConfiguration, 1f);
            //}

            //if (gameModels)
            //{
            //    _allGames.Add(instantiatedModel.transform);
            //    AddModelsToWorldAdditionalLoopStepsForGames(instantiatedModel);
            //}
            //else
            //    AddModelsToWorldAdditionalLoopStepsForProps(instantiatedModel);
        }

#if UNITY_EDITOR
        private void LoadInEditorGame(List<string> namesToTry)
        {
            foreach (string nameToTry in namesToTry)
            {
                try
                {
                    GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(nameToTry);
                    if (prefab != null)
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate(prefab, ModelPosition, ModelOrientation, _parent);
                        SetupModel(gameObject);
                        break;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
#endif
    }
}
