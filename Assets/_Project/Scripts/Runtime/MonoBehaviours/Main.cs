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
using DG.Tweening;
using SK.Utilities.Unity;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class Main : MonoBehaviour
    {
        private VirtualFileSystem _virtualFileSystem;
        private ArcadeContext _arcadeContext;

        [Inject]
        public void Construct(VirtualFileSystem virtualFileSystem, ArcadeContext arcadeContext)
        {
            _virtualFileSystem = virtualFileSystem;
            _arcadeContext     = arcadeContext;
        }

        private void Start()
        {
            QualitySettings.vSyncCount  = 0;
            Application.targetFrameRate = -1;
            Time.timeScale              = 1f;

            ValidateCurrentOS();

            DOTween.SetTweensCapacity(1250, 50);

            string dataPath = SystemUtils.GetDataPath();
            _ = _virtualFileSystem.MountFile("general_cfg", $"{dataPath}/3darcade~/Configuration/GeneralConfiguration.xml")
                                  .MountDirectory("emulator_cfgs", $"{dataPath}/3darcade~/Configuration/Emulators")
                                  .MountDirectory("platform_cfgs", $"{dataPath}/3darcade~/Configuration/Platforms")
                                  .MountDirectory("arcade_cfgs", $"{dataPath}/3darcade~/Configuration/Arcades")
                                  .MountDirectory("gamelist_cfgs", $"{dataPath}/3darcade~/Configuration/Gamelists")
                                  .MountDirectory("medias", $"{dataPath}/3darcade~/Media")
                                  .MountFile("game_database", $"{dataPath}/3darcade~/GameDatabase.db");

            _arcadeContext.Start();

            //string mameSupportDirectory = $"{Application.streamingAssetsPath}/3darcade~/Dats";
            //string listXmlPath          = $"{mameSupportDirectory}/mame2003-plus.xml";
            //string iniGenrePath         = $"{mameSupportDirectory}/genre.ini";
            //string iniMaturePath        = $"{mameSupportDirectory}/mature.ini";

            //try
            //{
            //    ParseMameXMLAndAddToDb("mame2003_plus", listXmlPath, iniGenrePath, iniMaturePath);
            //}
            //catch (System.Exception e)
            //{
            //    Debug.LogException(e);
            //}
        }

#if !UNITY_EDITOR
        private const int SLEEP_TIME = 200;
        private bool _focused = true;
        private void OnApplicationFocus(bool focus) => _focused = focus;
        private void Update()
        {
            if (!_focused)
            {
                System.Threading.Thread.Sleep(SLEEP_TIME);
                return;
            }
            _arcadeContext.OnUpdate(Time.deltaTime);
        }
#else
        private void Update() => _arcadeContext.OnUpdate(Time.deltaTime);
#endif
        private void FixedUpdate() => _arcadeContext.OnFixedUpdate(Time.fixedDeltaTime);

        private void OnDisable() => DOTween.KillAll();

        // TODO: Replace/Remove the public functions...
        public void ToggleInput(bool enable)
        {
            if (enable)
            {
                _arcadeContext.InputActions.FpsArcade.Enable();
                if (Cursor.lockState == CursorLockMode.None)
                    _arcadeContext.InputActions.FpsArcade.Look.Disable();

                _arcadeContext.InputActions.FpsMoveCab.Enable();

                _arcadeContext.InputActions.CylArcade.Enable();
                if (Cursor.lockState == CursorLockMode.None)
                    _arcadeContext.InputActions.CylArcade.Look.Disable();
            }
            else
            {
                _arcadeContext.InputActions.FpsArcade.Disable();
                _arcadeContext.InputActions.FpsMoveCab.Disable();
                _arcadeContext.InputActions.CylArcade.Disable();
            }
        }

        public void ToggleInteractionsInput(bool enable)
        {
            if (enable)
            {
                _arcadeContext.InputActions.FpsArcade.Enable();
                if (Cursor.lockState == CursorLockMode.None)
                    _arcadeContext.InputActions.FpsArcade.Look.Disable();

                _arcadeContext.InputActions.FpsMoveCab.Enable();

                _arcadeContext.InputActions.CylArcade.Enable();
                if (Cursor.lockState == CursorLockMode.None)
                    _arcadeContext.InputActions.CylArcade.Look.Disable();
            }
            else
            {
                _arcadeContext.InputActions.FpsArcade.Interact.Disable();
                _arcadeContext.InputActions.FpsArcade.Look.Disable();

                _arcadeContext.InputActions.FpsMoveCab.Grab.Disable();

                _arcadeContext.InputActions.CylArcade.Interact.Disable();
                _arcadeContext.InputActions.CylArcade.Look.Disable();
            }
        }

        public void TransitionToFpsNormalState()
        {
            if (_arcadeContext.GeneralConfiguration.EnableVR)
                _arcadeContext.TransitionTo<ArcadeVirtualRealityFpsNormalState>();
            else
                _arcadeContext.TransitionTo<ArcadeStandardFpsNormalState>();
        }

        public void TransitionToFpsEditModeState()
        {
            if (_arcadeContext.GeneralConfiguration.EnableVR)
                _arcadeContext.TransitionTo<ArcadeVirtualRealityFpsEditModeState>();
            else
                _arcadeContext.TransitionTo<ArcadeStandardFpsEditModeState>();
        }

        private readonly List<GameObject> _editModeAddedItems = new List<GameObject>();

        public void AddGameModel() => AddGameModelAsync().Forget();

        private async UniTaskVoid AddGameModelAsync()
        {
            if (_arcadeContext.ArcadeController == null)
                return;

            Transform playerTransform = _arcadeContext.Player.ActiveTransform;

            Vector3 playerPosition  = playerTransform.position;
            Vector3 playerDirection = playerTransform.forward;
            float spawnDistance     = 2f;

            ModelConfiguration modelConfiguration = new ModelConfiguration { Id = "id" };

            Vector3 verticalOffset   = Vector3.up * 0.4f;
            Vector3 spawnPosition    = playerPosition + verticalOffset + playerDirection * spawnDistance;
            Quaternion spawnRotation = Quaternion.LookRotation(-playerDirection);

            GameObject spawnedModel = await _arcadeContext.ArcadeController.SpawnGame(modelConfiguration, spawnPosition, spawnRotation);
            _editModeAddedItems.Add(spawnedModel);
        }

        public void RemoveModel()
        {
            ModelConfigurationComponent target = _arcadeContext.InteractionControllers.EditModeController.InteractionData.Current;
            if (target == null)
                return;

            _ = _editModeAddedItems.Remove(target.gameObject);
            ObjectUtils.DestroyObject(target.gameObject);
        }

        public void RestoreCurrentArcadeModels()
        {
            foreach (GameObject item in _editModeAddedItems)
                ObjectUtils.DestroyObject(item);
            _editModeAddedItems.Clear();

            _arcadeContext.ArcadeController?.RestoreModelPositions();
        }

        public void SaveCurrentArcadeModels() => _arcadeContext.SaveCurrentArcade(true);

        private void ValidateCurrentOS()
        {
            try
            {
                OS currentOS = SystemUtils.GetCurrentOS();
                Debug.Log($"Current OS: {currentOS}");
            }
            catch (System.Exception e)
            {
                ApplicationUtils.ExitApp(e.Message);
            }
        }

        //private Dictionary<string, string> _gameGenreDictionary;

        //private void ParseMameXMLAndAddToDb(string gameListName, string listXmlPath, string genreInipath, string matureIniPath)
        //{
        //    ParseAndAddGenres(genreInipath);

        //    IniFile iniMature = new IniFile();
        //    iniMature.Load(matureIniPath);

        //    ParseAndAddGames(gameListName, listXmlPath, iniMature);
        //}

        //private void ParseAndAddGenres(string iniPath)
        //{
        //    IniFile ini = new IniFile();
        //    ini.Load(iniPath);

        //    if (ini.Count < 3)
        //        return;

        //    int iniIndex           = 0;
        //    HashSet<string> genres = new HashSet<string>();
        //    _gameGenreDictionary   = new Dictionary<string, string>();
        //    foreach (KeyValuePair<string, IniSection> iniSection in ini)
        //    {
        //        if (iniIndex < 2)
        //        {
        //            ++iniIndex;
        //            continue;
        //        }

        //        string genre = iniSection.Key;
        //        if (string.IsNullOrEmpty(genre))
        //            continue;

        //        _ = genres.Add(genre);
        //        foreach (KeyValuePair<string, IniValue> iniValue in iniSection.Value)
        //        {
        //            string gameName = iniValue.Key;
        //            if (!string.IsNullOrEmpty(gameName))
        //                _gameGenreDictionary.Add(gameName, genre);
        //        }
        //    }

        //    _sceneContext.Databases.GameDatabase.AddGenres(genres);
        //}

        //private void ParseAndAddGames(string gameListName, string listXmlPath, IniFile iniMature)
        //{
        //    Emulation.Mame.v2003Plus.Data mameData = XMLUtils.Deserialize<Emulation.Mame.v2003Plus.Data>(listXmlPath);
        //    if (mameData == null || mameData.Games == null || mameData.Games.Length == 0)
        //        return;

        //    IniSection matureList = null;
        //    _ = iniMature?.TryGetSection("ROOT_FOLDER", out matureList);

        //    HashSet<string> years         = new HashSet<string>();
        //    HashSet<string> manufacturers = new HashSet<string>();
        //    HashSet<string> screenTypes   = new HashSet<string>();
        //    HashSet<int> screenRotations  = new HashSet<int>();

        //    List<GameConfiguration> games = new List<GameConfiguration>();

        //    foreach (Emulation.Mame.v2003Plus.Game machine in mameData.Games)
        //    {
        //        string year                             = machine.Year;
        //        string manufacturer                     = machine.Manufacturer;
        //        GameScreenType screenType               = GameScreenType.Default;
        //        GameScreenOrientation screenOrientation = GameScreenOrientation.Default;

        //        if (!string.IsNullOrEmpty(year))
        //            _ = years.Add(year);

        //        if (!string.IsNullOrEmpty(manufacturer))
        //            _ = manufacturers.Add(manufacturer);

        //        if (machine.Video != null)
        //        {
        //            screenType = machine.Video.Screen switch
        //            {
        //                "raster" => GameScreenType.Raster,
        //                "vector" => GameScreenType.Vector,
        //                _        => GameScreenType.Default
        //            };

        //            if (screenType == GameScreenType.Raster)
        //                _ = screenTypes.Add("raster");
        //            else if (screenType == GameScreenType.Vector)
        //                _ = screenTypes.Add("vector");

        //            screenOrientation = machine.Video.Orientation switch
        //            {
        //                "horizontal" => GameScreenOrientation.Horizontal,
        //                "vertical"   => GameScreenOrientation.Vertical,
        //                _            => GameScreenOrientation.Default
        //            };

        //            if (screenOrientation == GameScreenOrientation.Horizontal)
        //                _ = screenRotations.Add(0);
        //            else if (screenOrientation == GameScreenOrientation.Vertical)
        //                _ = screenRotations.Add(90);
        //        }

        //        string gameName = machine.Name;
        //        string genre    = _gameGenreDictionary != null && _gameGenreDictionary.TryGetValue(gameName, out string foundGenre) ? foundGenre : null;
        //        bool mature     = matureList != null && matureList.ContainsKey(gameName);
        //        bool playable   = machine.Runnable != "no" && machine.Driver != null && (machine.Driver.Status.Equals("good") || machine.Driver.Status.Equals("imperfect"));

        //        GameConfiguration game = new GameConfiguration
        //        {
        //            Name              = gameName,
        //            Description       = machine.Description,
        //            CloneOf           = machine.CloneOf,
        //            RomOf             = machine.RomOf,
        //            Genre             = genre,
        //            Year              = year,
        //            Manufacturer      = manufacturer,
        //            ScreenType        = screenType,
        //            ScreenOrientation = screenOrientation,
        //            Mature            = mature,
        //            Playable          = playable,
        //            Available         = false
        //        };

        //        games.Add(game);
        //    }

        //    _sceneContext.Databases.GameDatabase.AddYears(years);
        //    _sceneContext.Databases.GameDatabase.AddManufacturers(manufacturers);
        //    _sceneContext.Databases.GameDatabase.AddScreenTypes(screenTypes);
        //    _sceneContext.Databases.GameDatabase.AddScreenRotations(screenRotations);

        //    _sceneContext.Databases.GameDatabase.AddGameList(gameListName);

        //    _sceneContext.Databases.GameDatabase.AddGames(gameListName, games);
        //}
    }
}
