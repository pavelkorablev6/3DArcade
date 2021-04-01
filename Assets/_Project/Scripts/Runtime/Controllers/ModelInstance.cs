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

namespace Arcade
{
    public sealed class ModelInstance
    {
        private readonly ModelConfiguration _modelConfiguration;
        private readonly IModelSpawner _modelSpawner;

        public ModelInstance(ModelConfiguration modelConfiguration)
        {
            _modelConfiguration = modelConfiguration;
#if UNITY_EDITOR
            if (Application.isPlaying)
                _modelSpawner = new ModelSpawner();
            else
                _modelSpawner = new EditorModelSpawner();
#else
            _modelSpawner = new ModelSpawner();
#endif
        }

        public void SpawnModel(IEnumerable<string> namesToTry, Transform parent, bool atPositionWithRotation)
        {
            Vector3 targetPosition;
            Quaternion targetOrientation;

            if (atPositionWithRotation)
            {
                targetPosition    = _modelConfiguration.Position;
                targetOrientation = Quaternion.Euler(_modelConfiguration.Rotation);
            }
            else
            {
                targetPosition    = Vector3.zero;
                targetOrientation = Quaternion.identity;
            }

            _modelSpawner.Spawn(namesToTry, targetPosition, targetOrientation, parent, SetupModel);
        }

        private void SetupModel(GameObject gameObject)
        {
            gameObject.name                 = _modelConfiguration.Id;
            gameObject.transform.localScale = _modelConfiguration.Scale;
            gameObject.AddComponent<ModelConfigurationComponent>()
                      .SetModelConfiguration(_modelConfiguration);

            // Look for artworks only in play mode / runtime
            //if (Application.isPlaying)
            //    SetupArtworks();

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
    }
}
