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

namespace Arcade
{
    public sealed class SceneContext : FSM.Context<SceneState>
    {
        //public PlayerControls CurrentPlayerControls;
        //public ModelConfigurationComponent CurrentModelConfiguration;

        public ArcadeController ArcadeController { get; private set; }
        public ArcadeConfiguration CurrentArcadeConfiguration { get; private set; }
        public ArcadeType CurrentArcadeType { get; private set; }
        public ArcadeMode CurrentArcadeMode { get; private set; }

        public readonly SceneContextData Data;

        //public VideoPlayerController VideoPlayerController { get; private set; }
        //public LayerMask RaycastLayers => LayerMask.GetMask("Arcade/ArcadeModels", "Arcade/GameModels", "Arcade/PropModels", "Selection");

        //private readonly ObjectsHierarchy _objectsHierarchy;
        //private readonly NodeController<MarqueeNodeTag> _marqueeNodeController;
        //private readonly NodeController<ScreenNodeTag> _screenNodeController;
        //private readonly NodeController<GenericNodeTag> _genericNodeController;

        public SceneContext(SceneContextData data)
        {
            Data = data;

            //_marqueeNodeController = new MarqueeNodeController(EmulatorDatabase, PlatformDatabase);
            //_screenNodeController  = new ScreenNodeController(EmulatorDatabase, PlatformDatabase);
            //_genericNodeController = new GenericNodeController(EmulatorDatabase, PlatformDatabase);

            _ = SetCurrentArcadeConfiguration(data.GeneralConfiguration.StartingArcade,
                                              data.GeneralConfiguration.StartingArcadeType,
                                              ArcadeMode.Normal);

            //_objectsHierarchy = new NormalHierarchy();
        }

        public bool SetCurrentArcadeConfiguration(string id, ArcadeType type, ArcadeMode mode)
        {
            if (Data.ArcadeDatabase.Get(id, out ArcadeConfiguration configuration))
            {
                CurrentArcadeConfiguration = configuration;
                CurrentArcadeType          = type;
                CurrentArcadeMode          = mode;
                return true;
            }
            return false;
        }

        //public void SetAndStartCurrentArcadeConfiguration(string id, SceneType type)
        //{
        //    if (SetCurrentArcadeConfiguration(id, type))
        //        TransitionTo<SceneLoadState>();
        //}

        public bool StartCurrentArcade()
        {
            if (CurrentArcadeConfiguration == null)
                return false;

            //_objectsHierarchy.RootNode.gameObject.AddComponentIfNotFound<ArcadeConfigurationComponent>()
            //                                     .Restore(CurrentSceneConfiguration);

            //_objectsHierarchy.Reset();

            switch (CurrentArcadeType)
            {
                case ArcadeType.Fps:
                {
                    Data.Player.SetState(Data.GeneralConfiguration.EnableVR ? Player.State.VRFPS : Player.State.NormalFPS);
                    //VideoPlayerController = new VideoPlayerControllerFps(LayerMask.GetMask("Arcade/GameModels", "Arcade/PropModels"));
                    ArcadeController = new FpsArcadeController(/*_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController*/);
                }
                break;
                case ArcadeType.Cyl:
                {
                    //VideoPlayerController = null;

                    Data.Player.SetState(Data.GeneralConfiguration.EnableVR ? Player.State.VRCYL : Player.State.NormalCYL);

                    switch (CurrentArcadeConfiguration.CylArcadeProperties.WheelVariant)
                    {
                    //    case WheelVariant.CameraInsideHorizontal:
                    //        ArcadeController = new CylArcadeControllerWheel3DCameraInsideHorizontal(_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController);
                    //        break;
                    //    case WheelVariant.CameraOutsideHorizontal:
                    //        ArcadeController = new CylArcadeControllerWheel3DCameraOutsideHorizontal(_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController);
                    //        break;
                        case WheelVariant.LineHorizontal:
                            ArcadeController = new CylArcadeControllerLineHorizontal(/*_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController*/);
                            break;
                    //    case WheelVariant.CameraInsideVertical:
                    //        ArcadeController = new CylArcadeControllerWheel3DCameraInsideVertical(_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController);
                    //        break;
                    //    case WheelVariant.CameraOutsideVertical:
                    //        ArcadeController = new CylArcadeControllerWheel3DCameraOutsideVertical(_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController);
                    //        break;
                    //    case WheelVariant.LineVertical:
                    //        ArcadeController = new CylArcadeControllerLineVertical(_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController);
                    //        break;
                    //    case WheelVariant.LineCustom:
                    //        ArcadeController = new CylArcadeControllerLine(_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController);
                    //        break;
                    }
                }
                break;
            }

            ArcadeController.StartArcade(CurrentArcadeConfiguration);
            return true;
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
