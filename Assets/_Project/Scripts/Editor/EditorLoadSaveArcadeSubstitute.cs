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
using UnityEngine;
using UnityEngine.Assertions;

namespace Arcade.UnityEditor
{
    public class EditorLoadSaveArcadeSubstitute
    {
        public readonly ArcadeHierarchy ArcadeHierarchy;
        public readonly ArcadeDatabase ArcadeDatabase;

        private readonly IVirtualFileSystem _virtualFileSystem;
        private readonly AssetCache<GameObject> _gameObjectCache;
        private readonly XMLDatabaseMultiFile<EmulatorConfiguration> _emulatorDatabase;
        private readonly PlatformDatabase _platformDatabase;
        private readonly PlayerFpsControls _playerFpsControls;
        private readonly PlayerCylControls _playerCylControls;
        private ArcadeController _arcadeController;

        public EditorLoadSaveArcadeSubstitute()
        {
            ArcadeHierarchy = new ArcadeHierarchy();

            string dataPath    = SystemUtils.GetDataPath();
            _virtualFileSystem = new VirtualFileSystem()
                .MountDirectory("arcade_cfgs", $"{dataPath}/3darcade~/Configuration/Arcades")
                .MountDirectory("platform_cfgs", $"{dataPath}/3darcade~/Configuration/Platforms")
                .MountDirectory("emulator_cfgs", $"{dataPath}/3darcade~/Configuration/Emulators")
                .MountDirectory("gamelist_cfgs", $"{dataPath}/3darcade~/Configuration/Gamelists")
                .MountDirectory("medias", $"{dataPath}/3darcade~/Media")
                .MountFile("general_cfg", $"{dataPath}/3darcade~/Configuration/GeneralConfiguration.xml");

            _gameObjectCache = new GameObjectCache();

            ArcadeDatabase    = new ArcadeDatabase(_virtualFileSystem);
            _emulatorDatabase = new EmulatorDatabase(_virtualFileSystem);
            _platformDatabase = new PlatformDatabase(_virtualFileSystem);

            GameObject playerControls = GameObject.Find("PlayerControls");
            Assert.IsNotNull(playerControls);
            _playerFpsControls = playerControls.GetComponentInChildren<PlayerFpsControls>(true);
            Assert.IsNotNull(_playerFpsControls);
            _playerCylControls = playerControls.GetComponentInChildren<PlayerCylControls>(true);
            Assert.IsNotNull(_playerCylControls);

            if (_playerFpsControls.gameObject.activeInHierarchy)
            {
                _playerFpsControls.gameObject.SetActive(true);
                _playerCylControls.gameObject.SetActive(false);
            }
            else
            {
                _playerFpsControls.gameObject.SetActive(false);
                _playerCylControls.gameObject.SetActive(true);
            }
        }

        public void LoadAndStartArcade(string name)
        {
            if (!ArcadeDatabase.Get(name, out ArcadeConfiguration arcadeConfiguration))
                return;

            ArcadeHierarchy.RootNode.gameObject.AddComponentIfNotFound<ArcadeConfigurationComponent>()
                                               .Restore(arcadeConfiguration);

            ArcadeHierarchy.Reset();
            if (_playerFpsControls.gameObject.activeInHierarchy)
                _arcadeController = new FpsArcadeController(ArcadeHierarchy, _playerFpsControls, _playerCylControls, _emulatorDatabase, _platformDatabase, _gameObjectCache, null, null, null);
            else
            {
                switch (arcadeConfiguration.CylArcadeProperties.WheelVariant)
                {
                    case WheelVariant.CameraInsideHorizontal:
                        _arcadeController = new CylArcadeControllerWheel3DCameraInsideHorizontal(ArcadeHierarchy, _playerFpsControls, _playerCylControls, _emulatorDatabase, _platformDatabase, _gameObjectCache, null, null, null);
                        break;
                    case WheelVariant.CameraOutsideHorizontal:
                        _arcadeController = new CylArcadeControllerWheel3DCameraOutsideHorizontal(ArcadeHierarchy, _playerFpsControls, _playerCylControls, _emulatorDatabase, _platformDatabase, _gameObjectCache, null, null, null);
                        break;
                    case WheelVariant.LineHorizontal:
                        _arcadeController = new CylArcadeControllerLineHorizontal(ArcadeHierarchy, _playerFpsControls, _playerCylControls, _emulatorDatabase, _platformDatabase, _gameObjectCache, null, null, null);
                        break;
                    case WheelVariant.CameraInsideVertical:
                        _arcadeController = new CylArcadeControllerWheel3DCameraInsideVertical(ArcadeHierarchy, _playerFpsControls, _playerCylControls, _emulatorDatabase, _platformDatabase, _gameObjectCache, null, null, null);
                        break;
                    case WheelVariant.CameraOutsideVertical:
                        _arcadeController = new CylArcadeControllerWheel3DCameraOutsideVertical(ArcadeHierarchy, _playerFpsControls, _playerCylControls, _emulatorDatabase, _platformDatabase, _gameObjectCache, null, null, null);
                        break;
                    case WheelVariant.LineVertical:
                        _arcadeController = new CylArcadeControllerLineVertical(ArcadeHierarchy, _playerFpsControls, _playerCylControls, _emulatorDatabase, _platformDatabase, _gameObjectCache, null, null, null);
                        break;
                    case WheelVariant.LineCustom:
                        _arcadeController = new CylArcadeControllerLine(ArcadeHierarchy, _playerFpsControls, _playerCylControls, _emulatorDatabase, _platformDatabase, _gameObjectCache, null, null, null);
                        break;
                }
            }

            _arcadeController?.StartArcade(arcadeConfiguration);
        }

        public void SaveArcade(ArcadeConfigurationComponent arcadeConfiguration)
        {
            _playerCylControls.gameObject.SetActive(true);
            _playerCylControls.gameObject.SetActive(false);

            Camera camera                          = _playerFpsControls.Camera;
            CinemachineVirtualCamera virtualCamera = _playerFpsControls.VirtualCamera;
            CinemachineTransposer transposer       = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            CameraSettings fpsCameraSettings = new CameraSettings
            {
                Position      = _playerFpsControls.transform.position,
                Rotation      = MathUtils.CorrectEulerAngles(camera.transform.eulerAngles),
                Height        = transposer.m_FollowOffset.y,
                Orthographic  = camera.orthographic,
                FieldOfView   = virtualCamera.m_Lens.FieldOfView,
                AspectRatio   = virtualCamera.m_Lens.OrthographicSize,
                NearClipPlane = virtualCamera.m_Lens.NearClipPlane,
                FarClipPlane  = virtualCamera.m_Lens.FarClipPlane,
                ViewportRect  = camera.rect
            };

            camera        = _playerCylControls.Camera;
            virtualCamera = _playerCylControls.VirtualCamera;
            transposer    = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            CameraSettings cylCameraSettings = new CameraSettings
            {
                Position      = _playerCylControls.transform.position,
                Rotation      = MathUtils.CorrectEulerAngles(camera.transform.eulerAngles),
                Height        = transposer.m_FollowOffset.y,
                Orthographic  = camera.orthographic,
                FieldOfView   = virtualCamera.m_Lens.FieldOfView,
                AspectRatio   = virtualCamera.m_Lens.OrthographicSize,
                NearClipPlane = virtualCamera.m_Lens.NearClipPlane,
                FarClipPlane  = virtualCamera.m_Lens.FarClipPlane,
                ViewportRect  = camera.rect
            };

            _ = arcadeConfiguration.Save(ArcadeDatabase, fpsCameraSettings, cylCameraSettings, !_playerCylControls.gameObject.activeInHierarchy);
        }
    }
}
