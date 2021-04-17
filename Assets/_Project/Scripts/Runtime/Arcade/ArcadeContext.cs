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

using SK.Utilities.StateMachine;
using System.Collections.Generic;
using UnityEngine;

namespace Arcade
{
    public sealed class ArcadeContext : Context<ArcadeState>
    {
        public readonly InputActions InputActions;
        public readonly Player Player;
        public readonly GeneralConfiguration GeneralConfiguration;
        public readonly Databases Databases;

        public readonly Scenes Scenes;

        public readonly AssetAddressesProviders AssetAddressesProviders;
        public readonly NodeControllers NodeControllers;
        public readonly IUIController UIController;

        public ArcadeConfiguration ArcadeConfiguration { get; private set; }
        public ArcadeType ArcadeType { get; private set; }
        public ArcadeMode ArcadeMode { get; private set; }

        private ArcadeController _arcadeController;

        //public PlayerControls CurrentPlayerControls;
        //public ModelConfigurationComponent CurrentModelConfiguration;
        public VideoPlayerControllerBase VideoPlayerController { get; private set; }
        //public LayerMask RaycastLayers => LayerMask.GetMask("Arcade/ArcadeModels", "Arcade/GameModels", "Arcade/PropModels", "Selection");

        public ArcadeContext(InputActions inputActions,
                             Player player,
                             GeneralConfiguration generalConfiguration,
                             Databases databases,
                             Scenes scenes,
                             AssetAddressesProviders assetAddressesProviders,
                             NodeControllers nodeControllers,
                             IUIController uiController)
        {
            InputActions            = inputActions;
            Player                  = player;
            GeneralConfiguration    = generalConfiguration;
            Databases               = databases;
            Scenes                  = scenes;
            AssetAddressesProviders = assetAddressesProviders;
            NodeControllers         = nodeControllers;
            UIController            = uiController;
        }

        public void StartArcade(string id, ArcadeType arcadeType, ArcadeMode arcadeMode)
        {
            if (string.IsNullOrEmpty(id))
                return;

            Databases.Arcades.Initialize();
            if (!Databases.Arcades.TryGet(id, out ArcadeConfiguration arcadeConfiguration))
                return;

            ArcadeConfiguration = arcadeConfiguration;
            ArcadeType          = arcadeType;
            ArcadeMode          = arcadeMode;

            VideoPlayerController?.StopAllVideos();
            Player.TransitionTo<PlayerDisabledState>();
            _arcadeController = null;

            switch (ArcadeType)
            {
                case ArcadeType.Fps:
                {
                    VideoPlayerController = new FpsArcadeVideoPlayerController(LayerMask.GetMask("Arcade/GameModels", "Arcade/PropModels"));
                    _arcadeController     = new FpsArcadeController(this);
                }
                break;
                case ArcadeType.Cyl:
                {
                    VideoPlayerController = null;
                    _arcadeController     = ArcadeConfiguration.CylArcadeProperties.WheelVariant switch
                    {
                        //WheelVariant.CameraInsideHorizontal  => new CylArcadeControllerWheel3DCameraInsideHorizontal(ArcadeContext),
                        //WheelVariant.CameraOutsideHorizontal => new CylArcadeControllerWheel3DCameraOutsideHorizontal(ArcadeContext),
                        //WheelVariant.CameraInsideVertical    => new CylArcadeControllerWheel3DCameraInsideVertical(ArcadeContext),
                        //WheelVariant.CameraOutsideVertical   => new CylArcadeControllerWheel3DCameraOutsideVertical(ArcadeContext),
                        CylArcadeWheelVariant.LineHorizontal   => new CylArcadeControllerLineHorizontal(this),
                        //WheelVariant.LineVertical            => new CylArcadeControllerLineVertical(ArcadeContext),
                        CylArcadeWheelVariant.LineCustom     => new CylArcadeControllerLine(this),
                        _                                    => new CylArcadeControllerLineHorizontal(this)
                    };
                }
                break;
            }

            if (_arcadeController == null)
                return;

            if (Application.isPlaying)
                TransitionTo<ArcadeLoadState>();

            Scenes.Entities.Initialize(arcadeConfiguration, arcadeType);
            IEnumerable<AssetAddress> addressesToTry = AssetAddressesProviders.Arcade.GetAddressesToTry(arcadeConfiguration, arcadeType);
            Scenes.Arcade.Load(addressesToTry, _arcadeController.ArcadeSceneLoadCompletedCallback);
        }

        protected override void OnStart()
        {
            GeneralConfiguration.Initialize();
            Databases.Initialize();

            StartArcade(GeneralConfiguration.StartingArcade, GeneralConfiguration.StartingArcadeType, ArcadeMode.Normal);
        }

        protected override void OnUpdate(float dt)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                StartArcade(ArcadeConfiguration.Id, ArcadeType, ArcadeMode);
        }

        public bool SaveCurrentArcade()
        {
            if (!EntitiesScene.TryGetArcadeConfiguration(out ArcadeConfigurationComponent arcadeConfigurationComponent))
                return false;

            ArcadeConfiguration arcadeConfiguration = arcadeConfigurationComponent.ToArcadeConfiguration();
            foreach (ModelConfiguration game in arcadeConfiguration.Games)
            {
                game.Position.RoundValues();
                game.Rotation.RoundValues();
                game.Scale.RoundValues();
            }

            if (!Databases.Arcades.Save(arcadeConfiguration))
                return false;

            return Databases.Arcades.LoadAll();
        }

        /*
        public bool SaveCurrentArcadeConfiguration()
        {
            ArcadeConfigurationComponent cfgComponent = _objectsHierarchy.RootNode.GetComponent<ArcadeConfigurationComponent>();
            if (cfgComponent == null)
                return false;

            Camera fpsCamera                          = Main.PlayerFpsControls.Camera;
            CinemachineVirtualCamera fpsVirtualCamera = Main.PlayerFpsControls.VirtualCamera;
            CameraSettings fpsCameraSettings          = new CameraSettings
            {
                Position      = Main.PlayerFpsControls.transform.position,
                Rotation      = MathUtils.CorrectEulerAngles(fpsCamera.transform.eulerAngles),
                Height        = fpsVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y,
                Orthographic  = fpsCamera.orthographic,
                FieldOfView   = fpsVirtualCamera.m_Lens.FieldOfView,
                AspectRatio   = fpsVirtualCamera.m_Lens.OrthographicSize,
                NearClipPlane = fpsVirtualCamera.m_Lens.NearClipPlane,
                FarClipPlane  = fpsVirtualCamera.m_Lens.FarClipPlane,
                ViewportRect  = fpsCamera.rect
            };

            Camera cylCamera                          = Main.PlayerCylControls.Camera;
            CinemachineVirtualCamera cylVirtualCamera = Main.PlayerCylControls.VirtualCamera;
            CameraSettings cylCameraSettings          = new CameraSettings
            {
                Position      = Main.PlayerCylControls.transform.position,
                Rotation      = MathUtils.CorrectEulerAngles(cylCamera.transform.eulerAngles),
                Height        = cylVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y,
                Orthographic  = cylCamera.orthographic,
                FieldOfView   = cylVirtualCamera.m_Lens.FieldOfView,
                AspectRatio   = cylVirtualCamera.m_Lens.OrthographicSize,
                NearClipPlane = cylVirtualCamera.m_Lens.NearClipPlane,
                FarClipPlane  = cylVirtualCamera.m_Lens.FarClipPlane,
                ViewportRect  = cylCamera.rect
            };

            return cfgComponent.Save(_sceneDatabase, fpsCameraSettings, cylCameraSettings, !cylCamera.gameObject.activeInHierarchy);
        }

        public bool SaveCurrentArcadeConfigurationModels()
        {
            ArcadeConfigurationComponent cfgComponent = _objectsHierarchy.RootNode.GetComponent<ArcadeConfigurationComponent>();
            return cfgComponent != null && cfgComponent.SaveModelsOnly(_sceneDatabase, CurrentSceneConfiguration);
        }

        public void ReloadCurrentArcadeConfigurationModels()
        {
            if (_objectsHierarchy.RootNode.TryGetComponent(out ArcadeConfigurationComponent cfgComponent))
                cfgComponent.SetGamesAndPropsTransforms(CurrentSceneConfiguration);
        }

        public EmulatorConfiguration GetEmulatorForCurrentModelConfiguration()
        {
            if (CurrentModelConfiguration == null)
                return null;

            if (!string.IsNullOrEmpty(CurrentModelConfiguration.Emulator))
                return EmulatorDatabase.Get(CurrentModelConfiguration.Emulator);

            if (!string.IsNullOrEmpty(CurrentModelConfiguration.Platform)
             && PlatformDatabase.Get(CurrentModelConfiguration.Platform, out PlatformConfiguration platformConfiguration))
            {
                if (!string.IsNullOrEmpty(platformConfiguration.Emulator))
                    return EmulatorDatabase.Get(platformConfiguration.Emulator);
            }

            return null;
        }
        */
    }
}
