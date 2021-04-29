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
using Cinemachine.PostFX;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Arcade
{
    public abstract class ArcadeController
    {
        public bool Loaded { get; private set; } = false;

        public abstract float AudioMinDistance { get; protected set; }
        public abstract float AudioMaxDistance { get; protected set; }
        public abstract AnimationCurve VolumeCurve { get; protected set; }

        protected abstract CameraSettings CameraSettings { get; }
        protected abstract RenderSettings RenderSettings { get; }
        protected abstract bool GameModelsSpawnAtPositionWithRotation { get; }

        protected readonly ArcadeContext _arcadeContext;

        private readonly List<GameObject> _gameModels = new List<GameObject>();
        private readonly List<GameObject> _propModels = new List<GameObject>();

        private IModelSpawner _modelSpawner;

        public ArcadeController(ArcadeContext arcadeContext) => _arcadeContext = arcadeContext;

        public async UniTask Initialize()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                _modelSpawner = new EditorModelSpawner();
            else
                _modelSpawner = new ModelSpawner();
#else
            _modelSpawner = new ModelSpawner();
#endif
            SetupPlayer();
            await SpawnGames();
            await SpawProps();

            Loaded = true;
        }

        protected abstract void SetupPlayer();

        private async UniTask SpawnGames()
        {
            if (_arcadeContext.ArcadeConfiguration.Games == null)
                return;

            foreach (ModelConfiguration modelConfiguration in _arcadeContext.ArcadeConfiguration.Games)
            {
                GameObject go = await SpawnGame(modelConfiguration, _arcadeContext.Scenes.Entities.GamesNodeTransform);
                _gameModels.Add(go);
            }
        }

        private async UniTask SpawProps()
        {
            switch (_arcadeContext.ArcadeConfiguration.ArcadeType)
            {
                case ArcadeType.Fps:
                    await SpawnProps(_arcadeContext.ArcadeConfiguration.FpsArcadeProperties.Props);
                    break;
                case ArcadeType.Cyl:
                    await SpawnProps(_arcadeContext.ArcadeConfiguration.CylArcadeProperties.Props);
                    break;
                default:
                    throw new System.NotImplementedException($"Unhandled switch case for ArcadeType: {_arcadeContext.ArcadeConfiguration.ArcadeType}");
            }
        }

        private async UniTask SpawnProps(ModelConfiguration[] modelConfigurations)
        {
            if (modelConfigurations == null)
                return;

            foreach (ModelConfiguration modelConfiguration in modelConfigurations)
            {
                GameObject go = await SpawnProp(modelConfiguration, _arcadeContext.Scenes.Entities.PropsNodeTransform);
                _propModels.Add(go);
            }
        }

        private async UniTask<GameObject> SpawnGame(ModelConfiguration modelConfiguration, Transform parent)
        {
            GameConfiguration game = null;
            if (_arcadeContext.Databases.Platforms.TryGet(modelConfiguration.Platform, out PlatformConfiguration platform))
            {
                _ = _arcadeContext.Databases.Games.TryGet(platform.MasterList,
                                                          modelConfiguration.Id,
                                                          new string[] { "CloneOf", "RomOf", "Year", "ScreenType", "ScreenRotation" },
                                                          new string[] { "Name" },
                                                          out game);
            }

            modelConfiguration.PlatformConfiguration = platform;
            modelConfiguration.GameConfiguration     = game;

            AssetAddresses addressesToTry = _arcadeContext.AssetAddressesProviders.Game.GetAddressesToTry(modelConfiguration);
            return await SpawnModel(modelConfiguration, parent, EntitiesScene.GamesLayer, GameModelsSpawnAtPositionWithRotation, addressesToTry, true);
        }

        private async UniTask<GameObject> SpawnProp(ModelConfiguration modelConfiguration, Transform parent)
        {
            AssetAddresses addressesToTry = _arcadeContext.AssetAddressesProviders.Prop.GetAddressesToTry(modelConfiguration);
            return await SpawnModel(modelConfiguration, parent, EntitiesScene.PropsLayer, true, addressesToTry, false);
        }

        private async UniTask<GameObject> SpawnModel(ModelConfiguration modelConfiguration, Transform parent, int layer, bool spawnAtPositionWithRotation, AssetAddresses addressesToTry, bool applyArtworks)
        {
            Vector3 position;
            Quaternion orientation;

            if (spawnAtPositionWithRotation)
            {
                position    = modelConfiguration.Position;
                orientation = Quaternion.Euler(modelConfiguration.Rotation);
            }
            else
            {
                position    = Vector3.zero;
                orientation = Quaternion.identity;
            }

            GameObject go = await _modelSpawner.Spawn(addressesToTry, position, orientation, parent);
            if (go == null)
                return null;

            go.name                 = modelConfiguration.Id;
            go.transform.localScale = modelConfiguration.Scale;
            go.layer                = layer;

            ModelConfigurationComponent modelConfigurationComponent = go.AddComponent<ModelConfigurationComponent>();
            modelConfigurationComponent.SetModelConfiguration(modelConfiguration);

            if (_arcadeContext.GeneralConfiguration.EnableVR)
                _ = go.AddComponent<XRSimpleInteractable>();
            else
            {
                CinemachineVirtualCamera vCam = go.GetComponentInChildren<CinemachineVirtualCamera>();
                if (vCam != null)
                {
                    vCam.Priority = 0;

                    CinemachineVolumeSettings currentVolumeSettings = _arcadeContext.Player.Camera.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>();
                    if (currentVolumeSettings != null)
                    {
                        CinemachineVolumeSettings volumeSettings = vCam.gameObject.AddComponent<CinemachineVolumeSettings>();
                        volumeSettings.m_Profile = currentVolumeSettings.m_Profile;
                        vCam.AddExtension(volumeSettings);
                    }
                }
            }

            if (applyArtworks)
                await ApplyArtworks(go, modelConfiguration);

            return go;
        }

        private async UniTask ApplyArtworks(GameObject gameObject, ModelConfiguration modelConfiguration)
        {
            // Look for artworks only in play mode / runtime
            if (!Application.isPlaying)
                return;

            await _arcadeContext.NodeControllers.Marquee.Setup(this, gameObject, modelConfiguration, RenderSettings.MarqueeIntensity);

            float screenIntensity = GetScreenIntensity(modelConfiguration.GameConfiguration);
            await _arcadeContext.NodeControllers.Screen.Setup(this, gameObject, modelConfiguration, screenIntensity);

            await _arcadeContext.NodeControllers.Generic.Setup(this, gameObject, modelConfiguration, 1f);

            //if (gameModels)
            //    AddModelsToWorldAdditionalLoopStepsForGames(instantiatedModel);
            //else
            //    AddModelsToWorldAdditionalLoopStepsForProps(instantiatedModel);
        }

        private float GetScreenIntensity(GameConfiguration game)
        {
            if (game == null)
                return 1f;

            return game.ScreenType switch
            {
                GameScreenType.Default => 1f,
                GameScreenType.Lcd     => RenderSettings.ScreenLcdIntensity,
                GameScreenType.Raster  => RenderSettings.ScreenRasterIntensity,
                GameScreenType.Svg     => RenderSettings.ScreenSvgIntensity,
                GameScreenType.Vector  => RenderSettings.ScreenVectorIntenstity,
                _                      => throw new System.NotImplementedException($"Unhandled switch case for GameScreenType: {game.ScreenType}")
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
