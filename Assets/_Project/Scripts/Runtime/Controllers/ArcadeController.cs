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

namespace Arcade
{
    public abstract class ArcadeController
    {
        public abstract float AudioMinDistance { get; protected set; }
        public abstract float AudioMaxDistance { get; protected set; }
        public abstract AnimationCurve VolumeCurve { get; protected set; }

        public bool ArcadeSceneLoaded { get; private set; }

        protected abstract CameraSettings CameraSettings { get; }

        protected ArcadeConfiguration ArcadeConfiguration { get; private set; }
        protected ArcadeType ArcadeType { get; private set; }

        protected readonly Player _player;
        protected readonly GeneralConfiguration _generalConfiguration;
        protected readonly MultiFileDatabase<PlatformConfiguration> _platformDatabase;
        protected readonly ModelMatcher _modelMatcher;

        protected Transform _gamesNodeTransform;
        protected Transform _propsNodeTransform;

        private readonly IUIController _uiController;
        private readonly List<ModelController> _gameControllers;

        private AsyncOperationHandle<SceneInstance> _loadArcadeSceneHandle;
        private IResourceLocation _arcadeSceneResourceLocation;
        private SceneInstance _arcadeSceneInstance;

        private bool _includeEntities;
        private bool _triggerArcadeSceneReload;

        //public ModelConfigurationComponent CurrentGame { get; protected set; }

        //protected const string GAME_RESOURCES_DIRECTORY              = "Games";
        //protected const string PROP_RESOURCES_DIRECTORY              = "Props";
        //protected const string CYLARCADE_PIVOT_POINT_GAMEOBJECT_NAME = "InternalCylArcadeWheelPivotPoint";

        //protected readonly Main _main;
        //protected readonly ObjectsHierarchy _normalHierarchy;
        //protected bool _animating;

        //private readonly Database<EmulatorConfiguration> _emulatorDatabase;

        //private readonly NodeController<MarqueeNodeTag> _marqueeNodeController;
        //private readonly NodeController<ScreenNodeTag> _screenNodeController;
        //private readonly NodeController<GenericNodeTag> _genericNodeController;

        public ArcadeController(Player player, GeneralConfiguration generalConfiguration, MultiFileDatabase<PlatformConfiguration> platformDatabase, ModelMatcher modelMatcher, IUIController uiController)
        {
            _player               = player;
            _generalConfiguration = generalConfiguration;
            _platformDatabase     = platformDatabase;
            _modelMatcher         = modelMatcher;
            _uiController         = uiController;
            _gameControllers      = new List<ModelController>();
        }

        public void DebugLogProgress()
        {
            if (_loadArcadeSceneHandle.IsValid() && !_loadArcadeSceneHandle.IsDone)
                _uiController.UpdateStatusBar(_loadArcadeSceneHandle.PercentComplete);
        }

        public void StartArcade(ArcadeConfiguration arcadeConfiguration, ArcadeType arcadeType, bool includeEntities = true)
        {
            ArcadeSceneLoaded = false;

            ArcadeConfiguration = arcadeConfiguration;
            ArcadeType          = arcadeType;
            _includeEntities    = includeEntities;

            SetupEntitiesHierarchy();

            List<string> namesToTry = _modelMatcher.GetNamesToTryForArcade(arcadeConfiguration, arcadeType);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                LoadInEditorArcade(namesToTry);
                return;
            }
#endif
            Addressables.LoadResourceLocationsAsync(namesToTry, Addressables.MergeMode.UseFirst, typeof(SceneInstance)).Completed += ArcadeSceneResourceLocationRetrievedCallback;
        }

        public void StopArcade() => UnloadArcadeScene();

        protected abstract void SetupPlayer();

        protected abstract ModelController SetupGame(ModelConfiguration modelConfiguration);

        private void ArcadeSceneResourceLocationRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        {
            if (aoHandle.Result.Count == 0)
                return;

            _arcadeSceneResourceLocation = aoHandle.Result[0];

            _uiController.InitStatusBar($"Loading arcade: {_arcadeSceneResourceLocation}...");

            LoadArcadeScene();
        }

        private void LoadArcadeScene()
        {
            if (_arcadeSceneInstance.Scene.isLoaded)
            {
                _triggerArcadeSceneReload = true;
                UnloadArcadeScene();
            }
            else
            {
                _loadArcadeSceneHandle = Addressables.LoadSceneAsync(_arcadeSceneResourceLocation, LoadSceneMode.Additive);
                _loadArcadeSceneHandle.Completed += ArcadeSceneLoadedCallback;
            }
        }

        private void SetupEntitiesHierarchy()
        {
            SetupArcadeConfigurationNode();
            SetupGamesNode();
            SetupPropsNode();
        }

        private void SetupArcadeConfigurationNode()
        {
            ArcadeConfigurationComponent arcadeConfigurationComponent = Object.FindObjectOfType<ArcadeConfigurationComponent>(true);
            if (arcadeConfigurationComponent != null)
            {
                arcadeConfigurationComponent.gameObject.name = "ArcadeConfiguration";
                arcadeConfigurationComponent.FromArcadeConfiguration(ArcadeConfiguration);
            }
            else
            {
                GameObject arcadeNodeGameObject = new GameObject("ArcadeConfiguration");
                arcadeNodeGameObject.AddComponent<ArcadeConfigurationComponent>()
                                    .FromArcadeConfiguration(ArcadeConfiguration);
            }
        }

        private void SetupGamesNode() => _gamesNodeTransform = SetupNode<GamesNodeTag>("Games");

        private void SetupPropsNode() => _propsNodeTransform = SetupNode<PropsNodeTag>("Props");

        private static Transform SetupNode<T>(string name) where T : Component
        {
            GameObjectUtils.DestroyGameObjectThatHasComponent<T>();
            return new GameObject(name, typeof(T)).transform;
        }

        private void UnloadArcadeScene()
        {
            if (_arcadeSceneInstance.Scene.isLoaded)
                Addressables.UnloadSceneAsync(_arcadeSceneInstance).Completed += ArcadeSceneUnloadedCallback;
        }

        private void ArcadeSceneLoadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        {
            if (aoHandle.Status != AsyncOperationStatus.Succeeded)
                return;

            _arcadeSceneInstance = aoHandle.Result;

            ArcadeSceneLoaded = true;
            _uiController.ResetStatusBar();
            SetupPlayer();
            SpawnEntities();

            _ = SceneManager.SetActiveScene(_arcadeSceneInstance.Scene);
        }


        private void SpawnEntities()
        {
            if (!_includeEntities)
                return;

            if (ArcadeConfiguration.Games != null)
                foreach (ModelConfiguration modelConfiguration in ArcadeConfiguration.Games)
                    _gameControllers.Add(SetupGame(modelConfiguration));
        }

        private void ArcadeSceneUnloadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        {
            ArcadeSceneLoaded = false;

            if (_triggerArcadeSceneReload)
                LoadArcadeScene();

            _triggerArcadeSceneReload = false;
        }

#if UNITY_EDITOR
        private void LoadInEditorArcade(List<string> namesToTry)
        {
            foreach (string nameToTry in namesToTry)
            {
                System.Type assetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(nameToTry);
                if (assetType == typeof(UnityEditor.SceneAsset))
                {
                    Scene scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(nameToTry, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                    SetupPlayer();
                    SpawnEntities();
                    _ = SceneManager.SetActiveScene(scene);
                    UnityEditor.SceneVisibilityManager.instance.DisablePicking(scene);
                    break;
                }
            }
        }
#endif
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

        protected static float GetScreenIntensity(GameConfiguration gameConfiguration, RenderSettings renderSettings)
        {
            if (gameConfiguration == null)
                return 1.4f;

            return gameConfiguration.ScreenType switch
            {
                GameScreenType.Raster  => renderSettings.ScreenRasterIntensity,
                GameScreenType.Vector  => renderSettings.ScreenVectorIntenstity,
                GameScreenType.Pinball => renderSettings.ScreenPinballIntensity,
                _                      => 1.4f,
            };
        }
        */
    }
}
