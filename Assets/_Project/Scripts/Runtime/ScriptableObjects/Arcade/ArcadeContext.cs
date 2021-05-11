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
using UnityEngine.InputSystem;
using Zenject;

namespace Arcade
{
    [CreateAssetMenu(menuName = "Arcade/StateMachine/ArcadeContext", fileName = "ArcadeContext")]
    public sealed class ArcadeContext : Context<ArcadeState>
    {
        [SerializeField] private Databases _databases;
        [SerializeField] private Scenes _scenes;
        [SerializeField] private InteractionControllers _interactionControllers;
        [SerializeField] private BoolVariable _mouseOverUIVariable;
        [SerializeField] private GeneralConfigurationVariable _generalConfigurationVariable;
        [SerializeField] private ArcadeConfigurationVariable _arcadeConfigurationVariable;
        [SerializeField] private ArcadeControllerVariable _arcadeControllerVariable;
        [SerializeField] private VideoPlayerControllerVariable _videoPlayerControllerVariable;
        [SerializeField] private TypeEvent _uiStateTransitionEvent;

        public InputActions InputActions { get; private set; }
        public Player Player { get; private set; }
        public GameControllers GameControllers { get; private set; }

        public Databases Databases => _databases;
        public Scenes Scenes => _scenes;
        public InteractionControllers InteractionControllers => _interactionControllers;
        public bool MouseOverUI => _mouseOverUIVariable.Value;
        public GeneralConfiguration GeneralConfiguration => _generalConfigurationVariable.Value;
        public ArcadeConfiguration ArcadeConfiguration => _arcadeConfigurationVariable.Value;
        public ArcadeController ArcadeController => _arcadeControllerVariable.Value;
        public VideoPlayerController VideoPlayerController => _videoPlayerControllerVariable.Value;
        public TypeEvent UIStateTransitionEvent => _uiStateTransitionEvent;

        public AssetAddressesProviders AssetAddressesProviders { get; private set; }
        public IModelSpawner ModelSpawner { get; private set; }
        public NodeControllers NodeControllers { get; private set; }

        [Inject]
        public void Construct(InputActions inputActions, Player player, AssetAddressesProviders assetAddressesProviders, IModelSpawner modelSpawner, NodeControllers nodeControllers = null, GameControllers gameControllers = null)
        {
            InputActions            = inputActions;
            Player                  = player;
            AssetAddressesProviders = assetAddressesProviders;
            ModelSpawner            = modelSpawner;
            NodeControllers         = nodeControllers;
            GameControllers         = gameControllers;
        }

        protected override void OnContextStart() => Restart();

        protected override void OnContextUpdate(float dt)
        {
            if (_mouseOverUIVariable.Value || Keyboard.current is null)
                return;

            if (Keyboard.current.insertKey.wasPressedThisFrame)
                StartArcade(_arcadeConfigurationVariable.Value.Id, _arcadeConfigurationVariable.Value.ArcadeType, _arcadeConfigurationVariable.Value.ArcadeMode).Forget();

            if (Keyboard.current.homeKey.wasPressedThisFrame)
                Restart();
        }

        public void Restart()
        {
            _generalConfigurationVariable.Initialize();
            StartArcade(_generalConfigurationVariable.Value.StartingArcade, _generalConfigurationVariable.Value.StartingArcadeType, ArcadeMode.Normal).Forget();
        }

        public async UniTaskVoid StartArcade(string id, ArcadeType arcadeType, ArcadeMode arcadeMode)
        {
            if (string.IsNullOrEmpty(id))
                return;

            _databases.Initialize();
            if (!_databases.Arcades.TryGet(id, out ArcadeConfiguration arcadeConfiguration))
                return;

            InputActions?.Enable();

            _generalConfigurationVariable.Initialize();

            _interactionControllers.NormalModeRaycaster.Initialize(Player.Camera);
            _interactionControllers.EditModeRaycaster.Initialize(Player.Camera);

            arcadeConfiguration.ArcadeType = arcadeType;
            arcadeConfiguration.ArcadeMode = arcadeMode;

            _arcadeConfigurationVariable.Value = arcadeConfiguration;

            _arcadeControllerVariable.Value = null;

            _videoPlayerControllerVariable.Value?.StopAllVideos();
            _videoPlayerControllerVariable.Value = null;

            Player.TransitionTo<PlayerDisabledState>();

            switch (arcadeConfiguration.ArcadeType)
            {
                case ArcadeType.Fps:
                {
                    _videoPlayerControllerVariable.Value = new FpsArcadeVideoPlayerController(Player, LayerMask.GetMask("Arcade/GameModels", "Arcade/PropModels", "Arcade/Selection"));
                    _arcadeControllerVariable.Value      = new FpsArcadeController(this);
                }
                break;
                case ArcadeType.Cyl:
                {
                    _arcadeControllerVariable.Value = arcadeConfiguration.CylArcadeProperties.WheelVariant switch
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

            if (_arcadeControllerVariable.Value == null)
                return;

            if (Application.isPlaying)
            {
                List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
                UnityEngine.XR.InputDevices.GetDevices(devices);
                if (devices.Count == 0)
                    _generalConfigurationVariable.SetEnableVR(false);

                if (_generalConfigurationVariable.Value.EnableVR)
                    TransitionTo<ArcadeVirtualRealityLoadState>();
                else
                    TransitionTo<ArcadeStandardLoadState>();
            }

            _scenes.Entities.Initialize(arcadeConfiguration);

            AssetAddresses addressesToTry = AssetAddressesProviders.Arcade.GetAddressesToTry(arcadeConfiguration);

            bool success = await _scenes.Arcade.Load(addressesToTry);
            if (!success)
                return;

            await _arcadeControllerVariable.Value.Initialize(ModelSpawner);
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

            if (!_databases.Arcades.Save(arcadeConfiguration))
                return false;

            if (modelsOnly)
                return true;

            return _databases.Arcades.LoadAll();
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
        */
    }
}
