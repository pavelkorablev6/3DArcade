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

using System.Collections.Generic;
using UnityEngine;

namespace Arcade
{
    public sealed class ModelMatcher
    {
        private const string ARCADES_ADDRESS_PREFIX    = "Arcades/";
        private const string DEFAULT_ARCADE_SCENE_NAME = "_cylinder";

        private const string GAMES_ADDRESS_PREFIX     = "Games/";
        private const string DEFAULT_GAME_70_HOR_NAME = "_70_horizontal";
        private const string DEFAULT_GAME_80_HOR_NAME = "_80_horizontal";
        private const string DEFAULT_GAME_90_HOR_NAME = "_90_horizontal";
        private const string DEFAULT_GAME_70_VER_NAME = "_70_vertical";
        private const string DEFAULT_GAME_80_VER_NAME = "_80_vertical";
        private const string DEFAULT_GAME_90_VER_NAME = "_90_vertical";
        private const string DEFAULT_GAME_HOR_NAME    = DEFAULT_GAME_80_HOR_NAME;
        private const string DEFAULT_GAME_VER_NAME    = DEFAULT_GAME_80_VER_NAME;

        private const string PROPS_ADDRESS_PREFIX = "Props/";
        private const string DEFAULT_PROP_NAME    = "ElectricScooter";

        private const string EDITOR_ADDRESSABLES_PATH = "Assets/_Project/Addressables/";

        public List<string> GetNamesToTryForArcade(ArcadeConfiguration cfg, ArcadeType arcadeType)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id))
                return null;

            string sceneName = arcadeType == ArcadeType.Fps ? cfg.FpsArcadeProperties.Scene : cfg.CylArcadeProperties.Scene;

            List<string> result = new List<string>();
            AddToList(sceneName);
            AddToList(cfg.Id);
            AddToList(DEFAULT_ARCADE_SCENE_NAME);
            return result;

            void AddToList(string name)
            {
                if (Application.isPlaying)
                    result.AddStringIfNotNullOrEmpty($"{ARCADES_ADDRESS_PREFIX}{name}");
                else
                    result.AddStringIfNotNullOrEmpty($"{EDITOR_ADDRESSABLES_PATH}{ARCADES_ADDRESS_PREFIX}{name}/{name}.unity");
            }
        }

        public List<string> GetNamesToTryForGame(ModelConfiguration cfg, PlatformConfiguration platform, GameConfiguration game)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id))
                return null;

            List<string> result = new List<string>();

            AddToList(cfg.Model);       // From cfg model override
            AddToList(cfg.Id);          // From cfg id
            AddToList(game?.CloneOf);   // From game CloneOf in platform masterlist
            AddToList(game?.RomOf);     // From game RomOf in platform masterlist
            AddToList(platform?.Model); // From platform model

            // Generic model from cfg orientation/year
            if (!string.IsNullOrEmpty(cfg.Year))
            {
                switch (cfg.ScreenOrientation)
                {
                    case GameScreenOrientation.Default:
                        if (!string.IsNullOrEmpty(GetModelNameForGame(game)))
                            return result;
                        break;
                    case GameScreenOrientation.Horizontal:
                        result.AddStringIfNotNullOrEmpty(GetHorizontalModelNameForYear(cfg.Year));
                        return result;
                    case GameScreenOrientation.Vertical:
                        result.AddStringIfNotNullOrEmpty(GetVerticalModelNameForYear(cfg.Year));
                        return result;
                    default:
                        throw new System.Exception($"Unhandled switch case statement: {cfg.ScreenOrientation}");
                }
            }

            AddToList(GetModelNameForGame(game)); // Generic model from game orientation/year
            AddToList(DEFAULT_GAME_HOR_NAME);     // Default horizontal model

            return result;

            void AddToList(string name)
            {
                if (Application.isPlaying)
                    result.AddStringIfNotNullOrEmpty($"{GAMES_ADDRESS_PREFIX}{name}");
                else
                    result.AddStringIfNotNullOrEmpty($"{EDITOR_ADDRESSABLES_PATH}{GAMES_ADDRESS_PREFIX}{name}.prefab");
            }
        }

        public List<string> GetNamesToTryForProp(ModelConfiguration cfg)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id))
                return null;

            List<string> result = new List<string>();
            AddToList(cfg.Model);
            AddToList(cfg.Id);
            AddToList(DEFAULT_PROP_NAME);
            return result;

            void AddToList(string name)
            {
                if (Application.isPlaying)
                    result.AddStringIfNotNullOrEmpty($"{PROPS_ADDRESS_PREFIX}{name}");
                else
                    result.AddStringIfNotNullOrEmpty($"{EDITOR_ADDRESSABLES_PATH}{PROPS_ADDRESS_PREFIX}{name}/{name}.prefab");
            }
        }

        private static string GetModelNameForGame(GameConfiguration game)
        {
            if (game == null)
                return null;
            return game.ScreenOrientation == GameScreenOrientation.Horizontal
                   ? GetHorizontalModelNameForYear(game.Year)
                   : GetVerticalModelNameForYear(game.Year);
        }

        private static string GetHorizontalModelNameForYear(string yearString)
        {
            string name = DEFAULT_GAME_HOR_NAME;
            if (!string.IsNullOrEmpty(yearString) && int.TryParse(yearString, out int year) && year > 0)
            {
                if (year >= 1970 && year < 1980)
                    name = DEFAULT_GAME_70_HOR_NAME;
                else if (year < 1990)
                    name = DEFAULT_GAME_80_HOR_NAME;
                else if (year < 2000)
                    name = DEFAULT_GAME_90_HOR_NAME;
            }
            return Application.isPlaying ? $"{GAMES_ADDRESS_PREFIX}{name}" : $"{EDITOR_ADDRESSABLES_PATH}{GAMES_ADDRESS_PREFIX}{name}.prefab";
        }

        private static string GetVerticalModelNameForYear(string yearString)
        {
            string name = DEFAULT_GAME_VER_NAME;
            if (!string.IsNullOrEmpty(yearString) && int.TryParse(yearString, out int year) && year > 0)
            {
                if (year >= 1970 && year < 1980)
                    name = DEFAULT_GAME_70_VER_NAME;
                else if (year < 1990)
                    name = DEFAULT_GAME_80_VER_NAME;
                else if (year < 2000)
                    name = DEFAULT_GAME_90_VER_NAME;
            }
            return Application.isPlaying ? $"{GAMES_ADDRESS_PREFIX}/{name}" : $"{EDITOR_ADDRESSABLES_PATH}{GAMES_ADDRESS_PREFIX}{name}.prefab";
        }
    }
}
