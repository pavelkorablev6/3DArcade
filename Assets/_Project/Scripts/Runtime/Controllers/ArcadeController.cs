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
        public bool ArcadeLoaded { get; protected set; }

        //public ModelConfigurationComponent CurrentGame { get; protected set; }

        //public abstract float AudioMinDistance { get; protected set; }
        //public abstract float AudioMaxDistance { get; protected set; }
        //public abstract AnimationCurve VolumeCurve { get; protected set; }

        protected abstract string ArcadeName { get; }
        //protected abstract bool UseModelTransforms { get; }
        //protected abstract PlayerControls PlayerControls { get; }
        //protected abstract CameraSettings CameraSettings { get; }

        //protected const string GAME_RESOURCES_DIRECTORY              = "Games";
        //protected const string PROP_RESOURCES_DIRECTORY              = "Props";
        //protected const string CYLARCADE_PIVOT_POINT_GAMEOBJECT_NAME = "InternalCylArcadeWheelPivotPoint";

        //protected readonly Main _main;
        //protected readonly ObjectsHierarchy _normalHierarchy;
        //protected readonly List<Transform> _allGames;

        protected ArcadeConfiguration _currentArcadeConfiguration;

        //protected bool _animating;

        //private readonly Database<EmulatorConfiguration> _emulatorDatabase;
        //private readonly PlatformDatabase _platformDatabase;
        //private readonly AssetCache<SceneInstance> _sceneCache;
        //private readonly AssetCache<GameObject> _gameObjectCache;
        //private readonly ModelMatcher _modelMatcher;

        //private readonly NodeController<MarqueeNodeTag> _marqueeNodeController;
        //private readonly NodeController<ScreenNodeTag> _screenNodeController;
        //private readonly NodeController<GenericNodeTag> _genericNodeController;

        //private bool _gameModelsLoaded;

        private AsyncOperationHandle<SceneInstance> _loadSceneHandle;
        private SceneInstance _sceneInstance;
        private string _sceneName;
        private bool _triggerSceneReload;

        private readonly RectTransform _statusBarProgressBarTransform;
        private Vector3 _statusBarProgressBarScale;

        public ArcadeController(/*ObjectsHierarchy normalHierarchy,
                                Database<EmulatorConfiguration> emulatorDatabase,
                                PlatformDatabase platformDatabase,
                                AssetCache<GameObject> gameObjectCache,
                                NodeController<MarqueeNodeTag> marqueeNodeController,
                                NodeController<ScreenNodeTag> screenNodeController,
                                NodeController<GenericNodeTag> genericNodeController*/)
        {
            _statusBarProgressBarTransform = GameObject.Find("Bar").GetComponent<RectTransform>();
            _statusBarProgressBarScale = Vector3.one;
            //_sceneCache      = sceneCache ?? throw new System.ArgumentNullException(nameof(sceneCache));
            //_gameObjectCache = gameObjectCache ?? throw new System.ArgumentNullException(nameof(gameObjectCache));

            //_main = Object.FindObjectOfType<Main>();

//          _normalHierarchy   = normalHierarchy ?? throw new System.ArgumentNullException(nameof(normalHierarchy));
//          _allGames          = new List<Transform>();
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

        public void DebugLogProgress()
        {
            if (_loadSceneHandle.IsValid() && !_loadSceneHandle.IsDone)
            {
                _statusBarProgressBarScale.x = _loadSceneHandle.PercentComplete;
                _statusBarProgressBarTransform.localScale = _statusBarProgressBarScale;
            }
        }

        public void StartArcade(ArcadeConfiguration arcadeConfiguration)
        {
            ArcadeLoaded = false;

            _currentArcadeConfiguration = arcadeConfiguration;
            _sceneName                  = ArcadeName ?? arcadeConfiguration.Id;

            Addressables.LoadResourceLocationsAsync($"Arcades/{_sceneName}", typeof(SceneInstance)).Completed += ResourceLocationsRetrievedCallback;
        }

        public void StopArcade() => Unload();

        private void ResourceLocationsRetrievedCallback(AsyncOperationHandle<IList<IResourceLocation>> aoHandle)
        {
            if (aoHandle.Status != AsyncOperationStatus.Succeeded)
                _sceneName = "internal_empty";

            Addressables.Release(aoHandle);

            Load();
        }

        private void Load()
        {
            if (_sceneInstance.Scene.isLoaded)
            {
                _triggerSceneReload = true;
                Unload();
            }
            else
            {
                _loadSceneHandle = Addressables.LoadSceneAsync($"Arcades/{_sceneName}", LoadSceneMode.Additive);
                _loadSceneHandle.Completed += SceneLoadedCallback;
            }
        }

        private void Unload()
        {
            if (_sceneInstance.Scene.isLoaded)
                Addressables.UnloadSceneAsync(_sceneInstance).Completed += SceneUnloadedCallback;
        }

        private void SceneLoadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        {
            if (aoHandle.Status != AsyncOperationStatus.Succeeded)
                return;

            _sceneInstance = aoHandle.Result;

            _sceneInstance.ActivateAsync().completed += SceneActivatedCallback;
        }

        private void SceneActivatedCallback(AsyncOperation ao)
        {
            //RenderSettings renderSettings = _currentArcadeConfiguration.RenderSettings;

            //_ = _main.StartCoroutine(AddModelsToWorld(false, _arcadeConfiguration.Props, _normalHierarchy.PropsNode, renderSettings, PROP_RESOURCES_DIRECTORY, ModelMatcher.GetNamesToTryForProp));
            //_ = _main.StartCoroutine(AddModelsToWorld(true, _arcadeConfiguration.Games, _normalHierarchy.GamesNode, renderSettings, GAME_RESOURCES_DIRECTORY, _modelMatcher.GetNamesToTryForGame));
            //while (!_gameModelsLoaded)
            //    return null;

            //LateSetupWorld();

            //SetupPlayer();

            ArcadeLoaded = true;

            OnSceneActivated();
        }

        private void SceneUnloadedCallback(AsyncOperationHandle<SceneInstance> aoHandle)
        {
            ArcadeLoaded = false;

            if (aoHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (_triggerSceneReload)
                    Load();
                else
                    _sceneName = null;
            }

            _triggerSceneReload = false;
        }

        protected virtual void OnSceneActivated()
        {
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

        private void SetupPlayer()
        {
            PreSetupPlayer();

            PlayerControls.transform.SetPositionAndRotation(CameraSettings.Position, Quaternion.Euler(0f, CameraSettings.Rotation.y, 0f));

            PlayerControls.Camera.rect = CameraSettings.ViewportRect;

            CinemachineVirtualCamera vCam = PlayerControls.VirtualCamera;
            vCam.transform.eulerAngles    = new Vector3(0f, CameraSettings.Rotation.y, 0f);
            vCam.m_Lens.Orthographic      = CameraSettings.Orthographic;
            vCam.m_Lens.FieldOfView       = CameraSettings.FieldOfView;
            vCam.m_Lens.OrthographicSize  = CameraSettings.AspectRatio;
            vCam.m_Lens.NearClipPlane     = CameraSettings.NearClipPlane;
            vCam.m_Lens.FarClipPlane      = CameraSettings.FarClipPlane;

            CinemachineTransposer transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset.y      = CameraSettings.Height;
        }
        */
    }
}
