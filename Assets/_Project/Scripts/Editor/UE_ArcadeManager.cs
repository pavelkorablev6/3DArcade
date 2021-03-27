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

using UnityEditor;
using UnityEngine;

namespace Arcade.UnityEditor
{
    internal static class UE_ArcadeManager
    {
        public static bool Ready { get; private set; }

        public static readonly MultiFileDatabase<EmulatorConfiguration> EmulatorDatabase;
        public static readonly MultiFileDatabase<PlatformConfiguration> PlatformDatabase;
        public static readonly MultiFileDatabase<ArcadeConfiguration> ArcadeDatabase;

        private static readonly Player _player;

        private static readonly IVirtualFileSystem _virtualFileSystem;
        private static readonly GeneralConfiguration _generalConfiguration;
        private static readonly ModelMatcher _modelMatcher;
        private static readonly IUIController _uiController;
        private static ArcadeController _arcadeController;

        static UE_ArcadeManager()
        {
            _player = Object.FindObjectOfType<Player>();
            if (_player == null)
                return;

            _player.Construct(new PlayerContext(_player));

            string dataPath = SystemUtils.GetDataPath();
            _virtualFileSystem = new VirtualFileSystem().MountFile("general_cfg", $"{dataPath}/3darcade~/Configuration/GeneralConfiguration.xml")
                                                        .MountDirectory("emulator_cfgs", $"{dataPath}/3darcade~/Configuration/Emulators")
                                                        .MountDirectory("platform_cfgs", $"{dataPath}/3darcade~/Configuration/Platforms")
                                                        .MountDirectory("arcade_cfgs", $"{dataPath}/3darcade~/Configuration/Arcades")
                                                        .MountDirectory("gamelist_cfgs", $"{dataPath}/3darcade~/Configuration/Gamelists")
                                                        .MountDirectory("medias", $"{dataPath}/3darcade~/Media");

            _generalConfiguration = new GeneralConfiguration(_virtualFileSystem);
            EmulatorDatabase      = new EmulatorDatabase(_virtualFileSystem);
            PlatformDatabase      = new PlatformDatabase(_virtualFileSystem);
            ArcadeDatabase        = new ArcadeDatabase(_virtualFileSystem);
            _modelMatcher         = new ModelMatcher();
            _uiController         = Object.FindObjectOfType<UIController>();

            _generalConfiguration.Initialize();
            EmulatorDatabase.Initialize();
            PlatformDatabase.Initialize();
            ArcadeDatabase.Initialize();

            Ready = true;
        }

        public static void RefreshConfigurations()
        {
            _generalConfiguration.Load();
            _ = EmulatorDatabase.LoadAll();
            _ = PlatformDatabase.LoadAll();
            _ = ArcadeDatabase.LoadAll();
        }

        public static void SetCurrentArcade()
        {
            ArcadeConfigurationComponent arcadeConfigurationComponent = Object.FindObjectOfType<ArcadeConfigurationComponent>();
            if (arcadeConfigurationComponent != null)
            {
                EditorPrefs.SetString("ArcadeManagerArcadeConfiguration", arcadeConfigurationComponent.Id);
                EditorPrefs.SetInt("ArcadeManagerArcadeType", (int)ArcadeType.Fps);
                EditorPrefs.SetBool("ArcadeManagerSpawnEntities", true);
            }
        }

        public static void LoadArcade(string name, ArcadeType arcadeType, bool spawnEntitites)
        {
            if (!ArcadeDatabase.Get(name, out ArcadeConfiguration arcadeConfiguration))
                return;

            UE_Utilities.CloseAllScenes();

            _player.TransitionTo<PlayerDisabledState>();
            _arcadeController?.StopArcade();
            _arcadeController = null;

            EditorPrefs.SetString("ArcadeManagerArcadeConfiguration", arcadeConfiguration.Id);
            EditorPrefs.SetInt("ArcadeManagerArcadeType", (int)arcadeType);
            EditorPrefs.SetBool("ArcadeManagerSpawnEntities", spawnEntitites);

            switch (arcadeType)
            {
                case ArcadeType.Fps:
                    _arcadeController = new FpsArcadeController(_player, _generalConfiguration, PlatformDatabase, _modelMatcher, _uiController/*_objectsHierarchy, EmulatorDatabase, PlatformDatabase, _gameObjectCache, _marqueeNodeController, _screenNodeController, _genericNodeController*/);
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
                        CylArcadeWheelVariant.LineCustom     => new CylArcadeControllerLine(_player, _generalConfiguration, PlatformDatabase, _modelMatcher, _uiController),
                        _                                    => new CylArcadeControllerLineHorizontal(_player, _generalConfiguration, PlatformDatabase, _modelMatcher, _uiController)
                    };
                }
                break;
            }

            _arcadeController?.StartArcade(arcadeConfiguration, arcadeType, spawnEntitites);
        }

        public static void ReloadCurrentArcade()
        {
            string arcadeId       = EditorPrefs.GetString("ArcadeManagerArcadeConfiguration", null);
            ArcadeType arcadeType = (ArcadeType)EditorPrefs.GetInt("ArcadeManagerArcadeType", (int)ArcadeType.Fps);
            bool spawnEntitites   = EditorPrefs.GetBool("ArcadeManagerSpawnEntities", true);
            if (!string.IsNullOrEmpty(arcadeId))
                LoadArcade(arcadeId, arcadeType, spawnEntitites);
        }

        public static void SaveArcade(ArcadeConfigurationComponent arcadeConfiguration)
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
