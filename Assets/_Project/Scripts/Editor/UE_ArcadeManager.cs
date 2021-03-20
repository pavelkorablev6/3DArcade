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

using UnityEngine;

namespace Arcade.UnityEditor
{
    internal sealed class UE_ArcadeManager
    {
        public string[] ArcadeNames { get; private set; }

        private readonly Player _player;
        private readonly PlayerContext _playerContext;
        private readonly IVirtualFileSystem _virtualFileSystem;
        private readonly GeneralConfiguration _generalConfiguration;
        //private readonly MultiFileDatabase<EmulatorConfiguration> _emulatorDatabase;
        private readonly MultiFileDatabase<PlatformConfiguration> _platformDatabase;
        private readonly MultiFileDatabase<ArcadeConfiguration> _arcadeDatabase;
        private readonly ModelMatcher _modelMatcher;
        private readonly IUIController _uiController;
        private ArcadeController _arcadeController;

        public UE_ArcadeManager()
        {
            _player        = Object.FindObjectOfType<Player>();
            _playerContext = new PlayerContext(_player);
            _player.Construct(_playerContext);

            string dataPath    = SystemUtils.GetDataPath();
            _virtualFileSystem = new VirtualFileSystem();
            _ = _virtualFileSystem.MountFile("general_cfg", $"{dataPath}/3darcade~/Configuration/GeneralConfiguration.xml")
                                  .MountDirectory("emulator_cfgs", $"{dataPath}/3darcade~/Configuration/Emulators")
                                  .MountDirectory("platform_cfgs", $"{dataPath}/3darcade~/Configuration/Platforms")
                                  .MountDirectory("arcade_cfgs", $"{dataPath}/3darcade~/Configuration/Arcades")
                                  .MountDirectory("gamelist_cfgs", $"{dataPath}/3darcade~/Configuration/Gamelists")
                                  .MountDirectory("medias", $"{dataPath}/3darcade~/Media");

            _generalConfiguration = new GeneralConfiguration(_virtualFileSystem);
            //_emulatorDatabase     = new EmulatorDatabase(_virtualFileSystem);
            _platformDatabase     = new PlatformDatabase(_virtualFileSystem);
            _arcadeDatabase       = new ArcadeDatabase(_virtualFileSystem);
            _modelMatcher         = new ModelMatcher();
            _uiController         = Object.FindObjectOfType<UIController>();

            _generalConfiguration.Initialize();
            _platformDatabase.Initialize();
            _arcadeDatabase.Initialize();

            ArcadeNames = _arcadeDatabase.GetNames();
        }

        public void LoadArcade(string name, ArcadeType arcadeType, bool spawnEntitites)
        {
            if (!_arcadeDatabase.Get(name, out ArcadeConfiguration arcadeConfiguration))
                return;

            UE_Utilities.CloseAllScenes();

            _player.TransitionTo<PlayerDisabledState>();
            _arcadeController?.StopArcade();
            _arcadeController = null;

            switch (arcadeType)
            {
                case ArcadeType.Fps:
                    _arcadeController = new FpsArcadeController(_player, _generalConfiguration, _platformDatabase, _modelMatcher, _uiController/*_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController*/);
                    break;
                case ArcadeType.Cyl:
                {
                    _arcadeController = arcadeConfiguration.CylArcadeProperties.WheelVariant switch
                    {
                        //WheelVariant.CameraInsideHorizontal  => new CylArcadeControllerWheel3DCameraInsideHorizontal(_player, _generalConfiguration, _platformDatabase, _modelMatcher, _uiController),
                        //WheelVariant.CameraOutsideHorizontal => new CylArcadeControllerWheel3DCameraOutsideHorizontal(_player, _generalConfiguration, _platformDatabase, _modelMatcher, _uiController),
                        //WheelVariant.CameraInsideVertical    => new CylArcadeControllerWheel3DCameraInsideVertical(_player, _generalConfiguration, _platformDatabase, _modelMatcher, _uiController),
                        //WheelVariant.CameraOutsideVertical   => new CylArcadeControllerWheel3DCameraOutsideVertical(_player, _generalConfiguration, _platformDatabase, _modelMatcher, _uiController),
                        //WheelVariant.LineVertical            => new CylArcadeControllerLineVertical(_player, _generalConfiguration, _platformDatabase, _modelMatcher, _uiController),
                        CylArcadeWheelVariant.LineCustom              => new CylArcadeControllerLine(_player, _generalConfiguration, _platformDatabase, _modelMatcher, _uiController),
                        _                                    => new CylArcadeControllerLineHorizontal(_player, _generalConfiguration, _platformDatabase, _modelMatcher, _uiController)
                    };
                }
                break;
            }

            _arcadeController?.StartArcade(arcadeConfiguration, arcadeType, spawnEntitites);
        }

        public void SaveArcade(ArcadeConfigurationComponent arcadeConfiguration)
        {
            //_playerCylControls.gameObject.SetActive(true);
            //_playerCylControls.gameObject.SetActive(false);

            //Camera camera                          = _playerFpsControls.Camera;
            //CinemachineVirtualCamera virtualCamera = _playerFpsControls.VirtualCamera;
            //CinemachineTransposer transposer       = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            //CameraSettings fpsCameraSettings = new CameraSettings
            //{
            //    Position      = _playerFpsControls.transform.position,
            //    Rotation      = MathUtils.CorrectEulerAngles(camera.transform.eulerAngles),
            //    Height        = transposer.m_FollowOffset.y,
            //    Orthographic  = camera.orthographic,
            //    FieldOfView   = virtualCamera.m_Lens.FieldOfView,
            //    AspectRatio   = virtualCamera.m_Lens.OrthographicSize,
            //    NearClipPlane = virtualCamera.m_Lens.NearClipPlane,
            //    FarClipPlane  = virtualCamera.m_Lens.FarClipPlane,
            //    ViewportRect  = camera.rect
            //};

            //camera        = _playerCylControls.Camera;
            //virtualCamera = _playerCylControls.VirtualCamera;
            //transposer    = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            //CameraSettings cylCameraSettings = new CameraSettings
            //{
            //    Position      = _playerCylControls.transform.position,
            //    Rotation      = MathUtils.CorrectEulerAngles(camera.transform.eulerAngles),
            //    Height        = transposer.m_FollowOffset.y,
            //    Orthographic  = camera.orthographic,
            //    FieldOfView   = virtualCamera.m_Lens.FieldOfView,
            //    AspectRatio   = virtualCamera.m_Lens.OrthographicSize,
            //    NearClipPlane = virtualCamera.m_Lens.NearClipPlane,
            //    FarClipPlane  = virtualCamera.m_Lens.FarClipPlane,
            //    ViewportRect  = camera.rect
            //};

            //_ = arcadeConfiguration.Save(_arcadeDatabase, fpsCameraSettings, cylCameraSettings, !_playerCylControls.gameObject.activeInHierarchy);
        }
    }
}
