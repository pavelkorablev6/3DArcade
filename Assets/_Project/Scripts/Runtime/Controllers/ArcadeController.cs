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
    public abstract class ArcadeController
    {
        public abstract float AudioMinDistance { get; protected set; }
        public abstract float AudioMaxDistance { get; protected set; }
        public abstract AnimationCurve VolumeCurve { get; protected set; }

        protected abstract CameraSettings CameraSettings { get; }
        protected abstract RenderSettings RenderSettings { get; }
        protected abstract bool GameModelsSpawnAtPositionWithRotation { get; }

        protected readonly ArcadeContext _arcadeContext;

        private readonly List<ModelInstance> _gameModels = new List<ModelInstance>();
        private readonly List<ModelInstance> _propModels = new List<ModelInstance>();

        //public ModelConfigurationComponent CurrentGame { get; protected set; }
        //protected const string CYLARCADE_PIVOT_POINT_GAMEOBJECT_NAME = "InternalCylArcadeWheelPivotPoint";
        //protected bool _animating;

        public ArcadeController(ArcadeContext arcadeContext) => _arcadeContext = arcadeContext;

        public void ArcadeSceneLoadCompletedCallback()
        {
            SetupPlayer();
            SpawnGames();
            SpawProps();
        }

        protected abstract void SetupPlayer();

        private void SpawnGames()
        {
            if (_arcadeContext.ArcadeConfiguration.Games == null)
                return;

            foreach (ModelConfiguration modelConfiguration in _arcadeContext.ArcadeConfiguration.Games)
                _gameModels.Add(SpawnGame(modelConfiguration, _arcadeContext.Scenes.Entities.GamesNodeTransform));
        }

        private void SpawProps()
        {
            switch (_arcadeContext.ArcadeType)
            {
                case ArcadeType.Fps:
                    SpawnProps(_arcadeContext.ArcadeConfiguration.FpsArcadeProperties.Props);
                    break;
                case ArcadeType.Cyl:
                    SpawnProps(_arcadeContext.ArcadeConfiguration.CylArcadeProperties.Props);
                    break;
                default:
                    throw new System.NotImplementedException($"Unhandled switch case for ArcadeType: {_arcadeContext.ArcadeType}");
            }
        }

        private void SpawnProps(ModelConfiguration[] modelConfigurations)
        {
            if (modelConfigurations == null)
                return;

            foreach (ModelConfiguration modelConfiguration in modelConfigurations)
                _propModels.Add(SpawnProp(modelConfiguration, _arcadeContext.Scenes.Entities.PropsNodeTransform));
        }

        private ModelInstance SpawnGame(ModelConfiguration modelConfiguration, Transform parent)
        {
            ModelInstance modelInstance = new ModelInstance(modelConfiguration);

            GameConfiguration game = null;
            if (_arcadeContext.Databases.Platforms.TryGet(modelConfiguration.Platform, out PlatformConfiguration platform))
            {
                //_arcadeContext.Databases.Games.TryGet(platform.MasterList, modelConfiguration.Id, out game);
            }

            IEnumerable<string> namesToTry = _arcadeContext.AssetAddressesProviders.Game.GetNamesToTry(modelConfiguration, platform, game);
            modelInstance.SpawnModel(namesToTry, parent, GameModelsSpawnAtPositionWithRotation, ApplyArtworks);

            return modelInstance;
        }

        private ModelInstance SpawnProp(ModelConfiguration modelConfiguration, Transform parent)
        {
            ModelInstance modelInstance = new ModelInstance(modelConfiguration);

            IEnumerable<string> namesToTry = _arcadeContext.AssetAddressesProviders.Prop.GetNamesToTry(modelConfiguration);
            modelInstance.SpawnModel(namesToTry, parent, true, ApplyArtworks);

            return modelInstance;
        }

        private void ApplyArtworks(GameObject gameObject, ModelConfiguration modelConfiguration)
        {
            // Look for artworks only in play mode / runtime
            if (!Application.isPlaying)
                return;

            //PlatformConfiguration platform = null;
            //if (!string.IsNullOrEmpty(modelConfiguration.Platform))
            //    platform = _platformDatabase.Get(modelConfiguration.Platform);

            GameConfiguration game = null;
            //if (platform != null && platform.MasterList != null)
            //{
            //    game = _gameDatabase.Get(modelConfiguration.platform.MasterList, game.Id);
            //    if (game != null)
            //    {
            //    }
            //}

            _arcadeContext.NodeControllers.Marquee.Setup(gameObject, modelConfiguration, RenderSettings.MarqueeIntensity);
            _arcadeContext.NodeControllers.Screen.Setup(gameObject, modelConfiguration, GetScreenIntensity(game));
            _arcadeContext.NodeControllers.Generic.Setup(gameObject, modelConfiguration, 1f);

            //if (gameModels)
            //{
            //    _allGames.Add(instantiatedModel.transform);
            //    AddModelsToWorldAdditionalLoopStepsForGames(instantiatedModel);
            //}
            //else
            //    AddModelsToWorldAdditionalLoopStepsForProps(instantiatedModel);
        }

        private float GetScreenIntensity(GameConfiguration game)
        {
            if (game == null)
                return 1.4f;

            return game.ScreenType switch
            {
                GameScreenType.Raster  => RenderSettings.ScreenRasterIntensity,
                GameScreenType.Vector  => RenderSettings.ScreenVectorIntenstity,
                GameScreenType.Pinball => RenderSettings.ScreenPinballIntensity,
                _ => 1.4f
            };
        }

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
