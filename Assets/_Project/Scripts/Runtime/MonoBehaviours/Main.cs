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

using System;
using UnityEngine;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class Main : MonoBehaviour
    {
        public InputActions InputActions { get; private set; }

        [SerializeField] private Player _player;

        private IVirtualFileSystem _virtualFileSystem;
        private GeneralConfiguration _generalConfiguration;

        private MultiFileDatabase<EmulatorConfiguration> _emulatorDatabase;
        private MultiFileDatabase<PlatformConfiguration> _platformDatabase;
        private MultiFileDatabase<ArcadeConfiguration> _arcadeDatabase;

        private IUIController _uiController;

        private SceneContext _sceneContext;

        private void Awake()
        {
            SetGlobalApplicationSettings();

            CheckCurrentOS();

            InitVFS();

            InitInputActions();

            LoadGeneralConfiguration();

            LoadDatabases();

            InitUIController();
        }

        private void Start()
        {
            SystemUtils.HideMouseCursor();

            InitSceneContext();

            _sceneContext.TransitionTo<SceneLoadState>();
        }

        private void OnEnable() => InputActions.Global.Enable();

        private void OnDisable() => InputActions.Global.Disable();

#if !UNITY_EDITOR
        private const bool SLEEP_TIME = 200;
        private bool _focused = true;
        private void OnApplicationFocus(bool focus) => _focused = focus;
        private void Update()
        {
            if (!_focused)
            {
                System.Threading.Thread.Sleep(SLEEP_TIME);
                return;
            }
            _sceneContext.Update(Time.deltaTime);
        }
#else
        private void Update() => _sceneContext.Update(Time.deltaTime);
#endif
        private void FixedUpdate() => _sceneContext.FixedUpdate(Time.fixedDeltaTime);

        private static void SetGlobalApplicationSettings()
        {
            QualitySettings.vSyncCount  = 0;
            Application.targetFrameRate = -1;
            Time.timeScale              = 1f;
        }

        private void CheckCurrentOS()
        {
            try
            {
                OS currentOS = SystemUtils.GetCurrentOS();
                Debug.Log($"Current OS: {currentOS}");
            }
            catch (System.Exception e)
            {
                SystemUtils.ExitApp(e.Message);
            }
        }

        private void InitVFS()
        {
            string dataPath = SystemUtils.GetDataPath();
            Debug.Log($"Data path: {dataPath}");

            _virtualFileSystem = new VirtualFileSystem()
                .MountFile("general_cfg", $"{dataPath}/3darcade~/Configuration/GeneralConfiguration.xml")
                .MountDirectory("emulator_cfgs", $"{dataPath}/3darcade~/Configuration/Emulators")
                .MountDirectory("platform_cfgs", $"{dataPath}/3darcade~/Configuration/Platforms")
                .MountDirectory("arcade_cfgs", $"{dataPath}/3darcade~/Configuration/Arcades")
                .MountDirectory("gamelist_cfgs", $"{dataPath}/3darcade~/Configuration/Gamelists")
                .MountDirectory("medias", $"{dataPath}/3darcade~/Media");
        }

        private void InitInputActions() => InputActions = new InputActions();

        private void LoadGeneralConfiguration()
        {
            if (_virtualFileSystem == null)
            {
                SystemUtils.ExitApp("Virtual File System not initialized");
                return;
            }

            _generalConfiguration = new GeneralConfiguration(_virtualFileSystem);
            if (!_generalConfiguration.Load())
                SystemUtils.ExitApp("Failed to load general configuration");
        }

        private void LoadDatabases()
        {
            if (_virtualFileSystem == null)
            {
                SystemUtils.ExitApp("Virtual File System not initialized");
                return;
            }

            _emulatorDatabase = new EmulatorDatabase(_virtualFileSystem);
            _platformDatabase = new PlatformDatabase(_virtualFileSystem);
            _arcadeDatabase   = new ArcadeDatabase(_virtualFileSystem);
        }

        private void InitUIController() => _uiController = new UIController();

        private void InitSceneContext()
        {
            SceneContextData sceneContextData = new SceneContextData(_player,
                                                                     InputActions,
                                                                     _uiController,
                                                                     _generalConfiguration,
                                                                     _emulatorDatabase,
                                                                     _platformDatabase,
                                                                     _arcadeDatabase);
            _sceneContext = new SceneContext(sceneContextData);
        }
    }
}
