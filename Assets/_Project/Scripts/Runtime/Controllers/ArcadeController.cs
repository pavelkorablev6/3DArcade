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
using UnityEngine;

namespace Arcade
{
    public abstract class ArcadeController
    {
        public ModelSpawnerBase ModelSpawner { get; private set; }
        public bool Loaded { get; private set; } = false;

        public abstract float AudioMinDistance { get; protected set; }
        public abstract float AudioMaxDistance { get; protected set; }
        public abstract AnimationCurve VolumeCurve { get; protected set; }
        public abstract CameraSettings CameraSettings { get; }
        public abstract RenderSettings RenderSettings { get; }
        public abstract bool GameModelsSpawnAtPositionWithRotation { get; }

        protected readonly ArcadeContext _arcadeContext;

        private List<ModelConfigurationComponent> _gameModels;
        private List<ModelConfigurationComponent> _propModels;

        public ArcadeController(ArcadeContext arcadeContext) => _arcadeContext = arcadeContext;

        public async UniTask Initialize(ModelSpawnerBase modelSpawner)
        {
            Loaded = false;

            ModelSpawner = modelSpawner;

            SetupPlayer();
            _gameModels = await ModelSpawner.SpawnGamesAsync();
            _propModels = await ModelSpawner.SpawPropsAsync();

            ReflectionProbe[] probes = Object.FindObjectsOfType<ReflectionProbe>();
            foreach (ReflectionProbe probe in probes)
                _ = probe.RenderProbe();

            Loaded = true;
        }

        //public void StoreModelPositions()
        //{
        //    StoreModelPositions(_gameModels);
        //    StoreModelPositions(_propModels);
        //}

        //public void RestoreModelPositions()
        //{
        //    RestoreModelPositions(_gameModels);
        //    RestoreModelPositions(_propModels);
        //}

        public void EnableGlobalInputs() => _arcadeContext.InputActions.GlobalActions.Enable();
        public void DisableGlobalInputs() => _arcadeContext.InputActions.GlobalActions.Disable();

        public abstract void EnableMovementInputs();
        public abstract void DisableMovementInputs();

        public abstract void EnableEditModeInputs();
        public abstract void DisableEditModeInputs();

        protected abstract void SetupPlayer();

        //private static void StoreModelPositions(List<ModelConfigurationComponent> models)
        //{
        //    foreach (ModelConfigurationComponent model in models)
        //    {
        //        ModelConfiguration cfg     = model.Configuration;
        //        cfg.BeforeEditModePosition = cfg.Position;
        //        cfg.BeforeEditModeRotation = cfg.Rotation;
        //        cfg.BeforeEditModeScale    = cfg.Scale;
        //    }
        //}

        //private static void RestoreModelPositions(List<ModelConfigurationComponent> models)
        //{
        //    foreach (ModelConfigurationComponent model in models)
        //    {
        //        if (model == null)
        //            continue;

        //        ModelConfiguration cfg = model.Configuration;
        //        cfg.Position           = cfg.BeforeEditModePosition;
        //        cfg.Rotation           = cfg.BeforeEditModeRotation;
        //        cfg.Scale              = cfg.BeforeEditModeScale;

        //        model.transform.position   = cfg.Position;
        //        model.transform.rotation   = Quaternion.Euler(cfg.Rotation);
        //        model.transform.localScale = cfg.Scale;
        //    }
        //}

        /*
        public void NavigateForward(float dt)
        {
            if (!_animating)
                _ = _main.StartCoroutine(CoNavigateForward(dt));
        }

        public void NavigateBackward(float dt)
        {
            if (!_animating)
                _ = _main.StartCoroutine(CoNavigateBackward(dt));
        }

        protected abstract void PreSetupPlayer();

        protected virtual void AddModelsToWorldAdditionalLoopStepsForGames(GameObject instantiatedModel)
        {
        }

        protected virtual void AddModelsToWorldAdditionalLoopStepsForProps(GameObject instantiatedModel)
        {
        }

        protected virtual void LateSetupWorld()
        {
        }

        protected virtual IEnumerator CoNavigateForward(float dt)
        {
            yield break;
        }

        protected virtual IEnumerator CoNavigateBackward(float dt)
        {
            yield break;
        }

        protected IEnumerator AddModelsToWorld(bool gameModels, ModelConfiguration[] modelConfigurations, Transform parent, RenderSettings renderSettings, string resourceDirectory, ModelMatcher.GetNamesToTryDelegate getNamesToTry)
        {
            if (modelConfigurations == null)
                yield break;

            if (gameModels)
            {
                _gameModelsLoaded = false;
                _allGames.Clear();
            }

            foreach (ModelConfiguration modelConfiguration in modelConfigurations)
            {
                List<string> namesToTry = getNamesToTry(modelConfiguration);

                GameObject prefab = _gameObjectCache.Load(resourceDirectory, namesToTry);
                if (prefab == null)
                    continue;

                GameObject instantiatedModel = InstantiatePrefab(prefab, parent, modelConfiguration, !gameModels || UseModelTransforms);

                // Look for artworks only in play mode / runtime
                if (Application.isPlaying)
                {

                    PlatformConfiguration platform = null;
                    //if (!string.IsNullOrEmpty(modelConfiguration.Platform))
                    //    platform = _platformDatabase.Get(modelConfiguration.Platform);

                    GameConfiguration game = null;
                    if (platform != null && platform.MasterList != null)
                    {
                        //game = _gameDatabase.Get(modelConfiguration.platform.MasterList, game.Id);
                        if (game != null)
                        {
                        }
                    }

                    _marqueeNodeController.Setup(this, instantiatedModel, modelConfiguration, renderSettings.MarqueeIntensity);
                    _screenNodeController.Setup(this, instantiatedModel, modelConfiguration, GetScreenIntensity(game, renderSettings));
                    _genericNodeController.Setup(this, instantiatedModel, modelConfiguration, 1f);
                }

                if (gameModels)
                {
                    _allGames.Add(instantiatedModel.transform);
                    AddModelsToWorldAdditionalLoopStepsForGames(instantiatedModel);
                }
                else
                    AddModelsToWorldAdditionalLoopStepsForProps(instantiatedModel);

                // Instantiate asynchronously only when loaded from the editor menu / auto reload
                if (Application.isPlaying)
                    yield return null;
            }

            if (gameModels)
                _gameModelsLoaded = true;
        }

        */
    }
}
