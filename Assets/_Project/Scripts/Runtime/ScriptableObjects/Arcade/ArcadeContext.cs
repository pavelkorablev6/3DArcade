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
using SK.Utilities.Unity.StateMachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Zenject;

namespace Arcade
{
    [CreateAssetMenu(menuName = "Arcade/StateMachine/ArcadeContext", fileName = "ArcadeContext")]
    public sealed class ArcadeContext : Context<ArcadeState>
    {
        public InputActions InputActions { get; private set; }
        public Player Player { get; private set; }
        public GeneralConfigurationVariable GeneralConfiguration { get; private set; }
        public Databases Databases { get; private set; }
        public Scenes Scenes { get; private set; }
        public AssetAddressesProviders AssetAddressesProviders { get; private set; }
        public NodeControllers NodeControllers { get; private set; }
        public InteractionControllers InteractionControllers { get; private set; }
        public GameControllers GameControllers { get; private set; }

        public TypeEvent UIStateTransitionEvent { get; private set; }
        public StringVariable ArcadeNameVariable { get; private set; }

        public ArcadeConfiguration ArcadeConfiguration { get; private set; }
        public ArcadeController ArcadeController { get; private set; }
        public VideoPlayerController VideoPlayerController { get; private set; }

        [Inject]
        public void Construct(InputActions inputActions,
                              Player player,
                              GeneralConfigurationVariable generalConfiguration,
                              Databases databases,
                              Scenes scenes,
                              AssetAddressesProviders assetAddressesProviders,
                              NodeControllers nodeControllers                                = null,
                              InteractionControllers interactionControllers                  = null,
                              GameControllers gameControllers                                = null,
                              TypeEvent uiStateTransitionEvent                               = null,
                              [Inject(Id = "arcade_name")] StringVariable arcadeNameVariable = null)
        {
            InputActions            = inputActions;
            Player                  = player;
            GeneralConfiguration    = generalConfiguration;
            Databases               = databases;
            Scenes                  = scenes;
            AssetAddressesProviders = assetAddressesProviders;
            NodeControllers         = nodeControllers;
            InteractionControllers  = interactionControllers;
            GameControllers         = gameControllers;
            UIStateTransitionEvent  = uiStateTransitionEvent;
            ArcadeNameVariable      = arcadeNameVariable;
        }

        protected override void OnContextStart()
        {
            InputActions.Enable();
            GeneralConfiguration.Initialize();
            InteractionControllers.NormalModeRaycaster.Initialize(Player.Camera);
            InteractionControllers.EditModeRaycaster.Initialize(Player.Camera);
            InteractionControllers.NormalModeController.Initialize(this);

            StartArcade(GeneralConfiguration.Value.StartingArcade, GeneralConfiguration.Value.StartingArcadeType, ArcadeMode.Normal).Forget();
        }

        protected override void OnContextUpdate(float dt)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                StartArcade(ArcadeConfiguration.Id, ArcadeConfiguration.ArcadeType, ArcadeConfiguration.ArcadeMode).Forget();
        }

        public async UniTaskVoid StartArcade(string id, ArcadeType arcadeType, ArcadeMode arcadeMode)
        {
            if (string.IsNullOrEmpty(id))
                return;

            GeneralConfiguration.Initialize();
            Databases.Initialize();
            if (!Databases.Arcades.TryGet(id, out ArcadeConfiguration arcadeConfiguration))
                return;

            arcadeConfiguration.ArcadeType = arcadeType;
            arcadeConfiguration.ArcadeMode = arcadeMode;

            ArcadeConfiguration = arcadeConfiguration;

            VideoPlayerController?.StopAllVideos();

            VideoPlayerController = null;
            ArcadeController      = null;

            Player.TransitionTo<PlayerDisabledState>();

            switch (arcadeConfiguration.ArcadeType)
            {
                case ArcadeType.Fps:
                {
                    VideoPlayerController = new FpsArcadeVideoPlayerController(Player, LayerMask.GetMask("Arcade/GameModels", "Arcade/PropModels", "Arcade/Selection"));
                    ArcadeController      = new FpsArcadeController(this);
                }
                break;
                case ArcadeType.Cyl:
                {
                    ArcadeController = ArcadeConfiguration.CylArcadeProperties.WheelVariant switch
                    {
                        //WheelVariant.CameraInsideHorizontal  => new CylArcadeControllerWheel3DCameraInsideHorizontal(ArcadeContext),
                        //WheelVariant.CameraOutsideHorizontal => new CylArcadeControllerWheel3DCameraOutsideHorizontal(ArcadeContext),
                        //WheelVariant.CameraInsideVertical    => new CylArcadeControllerWheel3DCameraInsideVertical(ArcadeContext),
                        //WheelVariant.CameraOutsideVertical   => new CylArcadeControllerWheel3DCameraOutsideVertical(ArcadeContext),
                        CylArcadeWheelVariant.LineHorizontal => new CylArcadeControllerLineHorizontal(this),
                        //WheelVariant.LineVertical            => new CylArcadeControllerLineVertical(ArcadeContext),
                        CylArcadeWheelVariant.LineCustom     => new CylArcadeControllerLine(this),
                        _                                    => new CylArcadeControllerLineHorizontal(this)
                    };
                }
                break;
            }

            if (ArcadeController == null)
                return;


            if (Application.isPlaying)
            {
                ArcadeNameVariable.Value = arcadeConfiguration.ToString();

                List<InputDevice> devices = new List<InputDevice>();
                InputDevices.GetDevices(devices);
                if (devices.Count == 0)
                    GeneralConfiguration.Value.EnableVR = false;

                if (GeneralConfiguration.Value.EnableVR)
                    TransitionTo<ArcadeVirtualRealityLoadState>();
                else
                    TransitionTo<ArcadeStandardLoadState>();
            }

            Scenes.Entities.Initialize(arcadeConfiguration);

            AssetAddresses addressesToTry = AssetAddressesProviders.Arcade.GetAddressesToTry(arcadeConfiguration);

            bool success = await Scenes.Arcade.Load(addressesToTry);
            if (!success)
            {
                if (Application.isPlaying)
                    ArcadeNameVariable.Value = string.Empty;
                return;
            }

            await ArcadeController.Initialize();

            if (Application.isPlaying)
                ArcadeNameVariable.Value = string.Empty;
        }

        public bool SaveCurrentArcade(bool modelsOnly = false)
        {
            if (!EntitiesScene.TryGetArcadeConfiguration(out ArcadeConfigurationComponent arcadeConfigurationComponent))
                return false;

            ArcadeConfiguration arcadeConfiguration = arcadeConfigurationComponent.GetArcadeConfigurationWithUpdatedEntries();
            foreach (ModelConfiguration game in arcadeConfiguration.Games)
            {
                game.Position.RoundValues();
                game.Rotation.RoundValues();
                game.Scale.RoundValues();
            }

            if (!Databases.Arcades.Save(arcadeConfiguration))
                return false;

            if (modelsOnly)
                return true;

            return Databases.Arcades.LoadAll();
        }

        /*
        public bool SaveCurrentArcadeConfiguration()
        {
            ArcadeConfigurationComponent cfgComponent = _objectsHierarchy.RootNode.GetComponent<ArcadeConfigurationComponent>();
            if (cfgComponent == null)
                return false;

            Camera fpsCamera                          = Main.PlayerFpsControls.Camera;
            CinemachineNewVirtualCamera fpsVirtualCamera = Main.PlayerFpsControls.VirtualCamera;
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
            CinemachineNewVirtualCamera cylVirtualCamera = Main.PlayerCylControls.VirtualCamera;
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
