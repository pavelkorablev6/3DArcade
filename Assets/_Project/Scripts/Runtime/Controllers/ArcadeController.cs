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
    public sealed class ModelController
    {
        private readonly ModelMatcher _modelMatcher;

        private readonly ModelConfiguration _modelConfiguration;
        private readonly Transform _parent;

        public ModelController(ModelMatcher modelMatcher, ModelConfiguration modelConfiguration, Transform parent)
        {
            _modelMatcher       = modelMatcher;
            _modelConfiguration = modelConfiguration;
            _parent             = parent;
        }

        public void Instantiate()
        {
            List<string> namesToTry = _modelMatcher.GetNamesToTryForGame(_modelConfiguration);

            Addressables.LoadResourceLocationsAsync(namesToTry, Addressables.MergeMode.UseFirst, typeof(GameObject)).Completed += GameResourceLocationsRetrievedCallback;

            //GameObject prefab = _gameObjectCache.Load(resourceDirectory, namesToTry);
            //if (prefab == null)
            //    continue;

            //GameObject instantiatedModel = InstantiatePrefab(prefab, parent, modelConfiguration, !gameModels || UseModelTransforms);

            //// Look for artworks only in play mode / runtime
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

        private void GameResourceLocationsRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        {
            if (aoHandle.Result.Count > 0)
                Addressables.InstantiateAsync(aoHandle.Result[0], _modelConfiguration.Position, Quaternion.Euler(_modelConfiguration.Rotation), _parent).Completed += GameInstantiatedCallback;
        }

        private void GameInstantiatedCallback(AsyncOperationHandle<GameObject> aoHandle)
        {
        }
    }

    public abstract class ArcadeController
    {
        public bool ArcadeSceneLoaded { get; private set; }

        public abstract float AudioMinDistance { get; protected set; }
        public abstract float AudioMaxDistance { get; protected set; }
        public abstract AnimationCurve VolumeCurve { get; protected set; }

        protected ArcadeConfiguration ArcadeConfiguration { get; private set; }
        protected ArcadeType ArcadeType { get; private set; }

        protected abstract string ArcadeSceneName { get; }
        protected abstract CameraSettings CameraSettings { get; }

        protected readonly Player _player;
        protected readonly GeneralConfiguration _generalConfiguration;

        private const string ARCADE_ADDRESS_PREFIX            = "Arcades";
        private const string INTERNAL_ARCADE_CYLINDER_ADDRESS = ARCADE_ADDRESS_PREFIX + "/_cylinder";

        private readonly IUIController _uiController;
        private readonly ModelMatcher _modelMatcher;
        private readonly List<ModelController> _gameControllers;

        private AsyncOperationHandle<SceneInstance> _loadArcadeSceneHandle;
        private IResourceLocation _arcadeSceneResourceLocation;
        private SceneInstance _arcadeSceneInstance;
        private bool _triggerArcadeSceneReload;

        private Transform _gamesNode;
        private Transform _propsNode;


        //public ModelConfigurationComponent CurrentGame { get; protected set; }

        //public abstract float AudioMinDistance { get; protected set; }
        //public abstract float AudioMaxDistance { get; protected set; }
        //public abstract AnimationCurve VolumeCurve { get; protected set; }

        //protected abstract bool UseModelTransforms { get; }
        //protected abstract PlayerControls PlayerControls { get; }

        //protected const string GAME_RESOURCES_DIRECTORY              = "Games";
        //protected const string PROP_RESOURCES_DIRECTORY              = "Props";
        //protected const string CYLARCADE_PIVOT_POINT_GAMEOBJECT_NAME = "InternalCylArcadeWheelPivotPoint";

        //protected readonly Main _main;
        //protected readonly ObjectsHierarchy _normalHierarchy;
        //protected bool _animating;

        //private readonly Database<EmulatorConfiguration> _emulatorDatabase;
        //private readonly PlatformDatabase _platformDatabase;
        //private readonly AssetCache<SceneInstance> _sceneCache;
        //private readonly AssetCache<GameObject> _gameObjectCache;

        //private readonly NodeController<MarqueeNodeTag> _marqueeNodeController;
        //private readonly NodeController<ScreenNodeTag> _screenNodeController;
        //private readonly NodeController<GenericNodeTag> _genericNodeController;

        //private bool _gameModelsLoaded;

        public ArcadeController(Player player, GeneralConfiguration generalConfiguration, IUIController uiController, ModelMatcher modelMatcher)
        {
            _player               = player;
            _generalConfiguration = generalConfiguration;
            _uiController         = uiController;
            _modelMatcher         = modelMatcher;
            _gameControllers      = new List<ModelController>();
        }

        /*
         * public ArcadeController(ObjectsHierarchy normalHierarchy,
                                Database<EmulatorConfiguration> emulatorDatabase,
                                PlatformDatabase platformDatabase,
                                AssetCache<GameObject> gameObjectCache,
                                NodeController<MarqueeNodeTag> marqueeNodeController,
                                NodeController<ScreenNodeTag> screenNodeController,
                                NodeController<GenericNodeTag> genericNodeController)
        {

            //_sceneCache      = sceneCache ?? throw new System.ArgumentNullException(nameof(sceneCache));
            //_gameObjectCache = gameObjectCache ?? throw new System.ArgumentNullException(nameof(gameObjectCache));

            //_main = Object.FindObjectOfType<Main>();

//          _normalHierarchy   = normalHierarchy ?? throw new System.ArgumentNullException(nameof(normalHierarchy));
//          _emulatorDatabase  = emulatorDatabase ?? throw new System.ArgumentNullException(nameof(emulatorDatabase));
//          _platformDatabase  = platformDatabase ?? throw new System.ArgumentNullException(nameof(platformDatabase));

//          _marqueeNodeController = marqueeNodeController;
//          _screenNodeController  = screenNodeController;
//          _genericNodeController = genericNodeController;

//          _modelMatcher = new ModelMatcher(_platformDatabase);

//          GameObject foundPivotPoint = GameObject.Find(CYLARCADE_PIVOT_POINT_GAMEOBJECT_NAME);
//          if (foundPivotPoint != null)
//          {
//#if UNITY_EDITOR
//              Object.DestroyImmediate(foundPivotPoint);
//#else
//              Object.Destroy(foundPivotPoint);
//#endif
//          }
        }
        */

        public void DebugLogProgress()
        {
            if (_loadArcadeSceneHandle.IsValid() && !_loadArcadeSceneHandle.IsDone)
                _uiController.UpdateStatusBar(_loadArcadeSceneHandle.PercentComplete);
        }

        public void StartArcade(ArcadeConfiguration arcadeConfiguration, ArcadeType arcadeType)
        {
            ArcadeConfiguration = arcadeConfiguration;
            ArcadeType          = arcadeType;

            ArcadeSceneLoaded = false;
            string arcadeSceneName = ArcadeSceneName ?? arcadeConfiguration.Id;

            _uiController.InitStatusBar($"Loading arcade: {arcadeSceneName}...");

            Addressables.LoadResourceLocationsAsync($"{ARCADE_ADDRESS_PREFIX}/{arcadeSceneName}", typeof(SceneInstance)).Completed += ArcadeSceneResourceLocationRetrievedCallback;
        }

        public void StopArcade() => UnloadArcadeScene();

        public void SpawnGames()
        {
            Transform rootNode = new GameObject("Arcade").transform;
            _gamesNode = new GameObject("Games").transform;
            _gamesNode.SetParent(rootNode);
            _propsNode = new GameObject("Props").transform;
            _propsNode.SetParent(rootNode);

            if (ArcadeConfiguration.Games != null)
            {
                foreach (ModelConfiguration modelConfiguration in ArcadeConfiguration.Games)
                {
                    ModelController modelController = new ModelController(_modelMatcher, modelConfiguration, _gamesNode);
                    modelController.Instantiate();
                    _gameControllers.Add(modelController);
                }
            }
        }

        protected abstract void SetupPlayer();

        private void ArcadeSceneResourceLocationRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        {
            if (aoHandle.Status == AsyncOperationStatus.Succeeded && aoHandle.Result.Count > 0)
            {
                _arcadeSceneResourceLocation = aoHandle.Result[0];
                LoadArcadeScene();
            }
            else
            {
                Debug.LogError("Arcade addressable not found, loading default cylinder arcade");
                Addressables.LoadResourceLocationsAsync(INTERNAL_ARCADE_CYLINDER_ADDRESS, typeof(SceneInstance)).Completed += ArcadeSceneResourceLocationRetrievedCallback;
            }
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

            if (SceneManager.SetActiveScene(_arcadeSceneInstance.Scene))
            {
                SetupPlayer();
                _uiController.ResetStatusBar();
                ArcadeSceneLoaded = true;
            }
        }

        private void ArcadeSceneUnloadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        {
            ArcadeSceneLoaded = false;

            if (aoHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (_triggerArcadeSceneReload)
                    LoadArcadeScene();
            }

            _triggerArcadeSceneReload = false;
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

        protected static GameObject InstantiatePrefab(GameObject prefab, Transform parent, ModelConfiguration modelConfiguration, bool useModelTransforms)
        {
            Vector3 position    = useModelTransforms ? modelConfiguration.Position : DatabaseVector3.zero;
            Quaternion rotation = useModelTransforms ? Quaternion.Euler(modelConfiguration.Rotation) : Quaternion.identity;

            GameObject model           = Object.Instantiate(prefab, position, rotation, parent);
            model.name                 = modelConfiguration.Id;
            model.transform.localScale = modelConfiguration.Scale;
            model.transform.SetLayersRecursively(parent.gameObject.layer);
            model.AddComponent<ModelConfigurationComponent>()
                 .FromModelConfiguration(modelConfiguration);
            return model;
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
