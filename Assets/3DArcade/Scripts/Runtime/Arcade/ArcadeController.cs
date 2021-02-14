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

using Cinemachine;
using SK.Utilities.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Arcade
{
    public abstract class ArcadeController
    {
        public bool ArcadeLoaded { get; protected set; }
        public ModelConfigurationComponent CurrentGame { get; protected set; }

        public abstract float AudioMinDistance { get; protected set; }
        public abstract float AudioMaxDistance { get; protected set; }
        public abstract AnimationCurve VolumeCurve { get; protected set; }

        protected abstract string SceneName { get; }
        protected abstract bool UseModelTransforms { get; }
        protected abstract PlayerControls PlayerControls { get; }
        protected abstract CameraSettings CameraSettings { get; }

        protected const string GAME_RESOURCES_DIRECTORY              = "Games";
        protected const string PROP_RESOURCES_DIRECTORY              = "Props";
        protected const string CYLARCADE_PIVOT_POINT_GAMEOBJECT_NAME = "InternalCylArcadeWheelPivotPoint";

        protected readonly ArcadeHierarchy _arcadeHierarchy;
        protected readonly PlayerFpsControls _playerFpsControls;
        protected readonly PlayerCylControls _playerCylControls;

        protected readonly List<Transform> _allGames;

        protected ArcadeConfiguration _arcadeConfiguration;

        protected bool _animating;

        private static Scene _loadedScene;
        private static bool _sceneLoaded;

        private readonly XMLDatabase<EmulatorConfiguration> _emulatorDatabase;
        private readonly PlatformDatabase _platformDatabase;
        private readonly AssetCache<GameObject> _gameObjectCache;

        private readonly ModelMatcher _modelMatcher;

        private readonly NodeController<MarqueeNodeTag> _marqueeNodeController;
        private readonly NodeController<ScreenNodeTag> _screenNodeController;
        private readonly NodeController<GenericNodeTag> _genericNodeController;

        private readonly CoroutineHelper _coroutineHelper;

        private bool _gameModelsLoaded;

        public ArcadeController(ArcadeHierarchy arcadeHierarchy,
                                PlayerFpsControls playerFpsControls,
                                PlayerCylControls playerCylControls,
                                XMLDatabase<EmulatorConfiguration> emulatorDatabase,
                                PlatformDatabase platformDatabase,
                                AssetCache<GameObject> gameObjectCache,
                                NodeController<MarqueeNodeTag> marqueeNodeController,
                                NodeController<ScreenNodeTag> screenNodeController,
                                NodeController<GenericNodeTag> genericNodeController)
        {
            _arcadeHierarchy   = arcadeHierarchy ?? throw new System.ArgumentNullException(nameof(arcadeHierarchy));
            _playerFpsControls = playerFpsControls != null ? playerFpsControls : throw new System.ArgumentNullException(nameof(playerFpsControls));
            _playerCylControls = playerCylControls != null ? playerCylControls : throw new System.ArgumentNullException(nameof(playerCylControls));
            _allGames          = new List<Transform>();
            _emulatorDatabase  = emulatorDatabase ?? throw new System.ArgumentNullException(nameof(emulatorDatabase));
            _platformDatabase  = platformDatabase ?? throw new System.ArgumentNullException(nameof(platformDatabase));
            _gameObjectCache   = gameObjectCache ?? throw new System.ArgumentNullException(nameof(gameObjectCache));

            _marqueeNodeController = marqueeNodeController;
            _screenNodeController  = screenNodeController;
            _genericNodeController = genericNodeController;

            _modelMatcher = new ModelMatcher(_platformDatabase);

            _coroutineHelper = Object.FindObjectOfType<CoroutineHelper>();
            Assert.IsNotNull(_coroutineHelper);

            GameObject foundPivotPoint = GameObject.Find(CYLARCADE_PIVOT_POINT_GAMEOBJECT_NAME);
            if (foundPivotPoint != null)
            {
#if UNITY_EDITOR
                Object.DestroyImmediate(foundPivotPoint);
#else
                Object.Destroy(foundPivotPoint);
#endif
            }
        }

        public void StartArcade(ArcadeConfiguration arcadeConfiguration)
        {
            Assert.IsNotNull(arcadeConfiguration);

            _arcadeConfiguration = arcadeConfiguration;
            ArcadeLoaded         = false;

            if (_sceneLoaded)
                _ = _coroutineHelper.StartCoroutine(CoUnloadArcadeScene());
            else
                _ = _coroutineHelper.StartCoroutine(CoSetupWorld());
        }

        public void NavigateForward(float dt)
        {
            if (!_animating)
                _ = _playerCylControls.StartCoroutine(CoNavigateForward(dt));
        }

        public void NavigateBackward(float dt)
        {
            if (!_animating)
                _ = _playerCylControls.StartCoroutine(CoNavigateBackward(dt));
        }

        protected abstract void PreSetupPlayer();

        protected virtual void PostLoadScene()
        {
        }

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
            Vector3 position    = useModelTransforms ? modelConfiguration.Position : XMLVector3.zero;
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

        private IEnumerator CoUnloadArcadeScene()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(_loadedScene, true);
            else
            {
                AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(_loadedScene);
                while (!asyncOperation.isDone)
                    yield return null;
            }
#else
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(_loadedScene);
            while (!asyncOperation.isDone)
                yield return null;
#endif
           _ = _coroutineHelper.StartCoroutine(CoSetupWorld());
        }

        private IEnumerator CoSetupWorld()
        {
            _sceneLoaded = false;

            string sceneName = SceneName ?? _arcadeConfiguration.Id;

            List<string> scenes = new List<string>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
                scenes.Add(Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));

            if (!scenes.Contains(sceneName))
                sceneName = "empty";

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                try
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/3DArcade/Scenes/{sceneName}/{sceneName}.unity", UnityEditor.SceneManagement.OpenSceneMode.Additive);
                }
                catch (System.Exception)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/3DArcade/Scenes/empty/empty.unity", UnityEditor.SceneManagement.OpenSceneMode.Additive);
                }
            }
            else
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!asyncOperation.isDone)
                    yield return null;
            }
#else
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncOperation.isDone)
                yield return null;
#endif
            _loadedScene = SceneManager.GetSceneByName(sceneName);

            PostLoadScene();

            RenderSettings renderSettings = _arcadeConfiguration.RenderSettings;

            _ = _coroutineHelper.StartCoroutine(AddModelsToWorld(false, _arcadeConfiguration.Props, _arcadeHierarchy.PropsNode, renderSettings, PROP_RESOURCES_DIRECTORY, ModelMatcher.GetNamesToTryForProp));
            _ = _coroutineHelper.StartCoroutine(AddModelsToWorld(true, _arcadeConfiguration.Games, _arcadeHierarchy.GamesNode, renderSettings, GAME_RESOURCES_DIRECTORY, _modelMatcher.GetNamesToTryForGame));
            while (!_gameModelsLoaded)
                yield return null;

            LateSetupWorld();

            SetupPlayer();

            _sceneLoaded = true;
            ArcadeLoaded = true;
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
    }
}
