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
    public sealed class GamePrefabAddressesProvider : IGamePrefabAddressesProvider
    {
        private const string FILE_EXTENSION             = "prefab";
        private const string ADDRESSABLES_PREFIX        = "Games/";
        private const string DEFAULT_70_HOR_PREFAB_NAME = "_70_horizontal";
        private const string DEFAULT_80_HOR_PREFAB_NAME = "_80_horizontal";
        private const string DEFAULT_90_HOR_PREFAB_NAME = "_90_horizontal";
        private const string DEFAULT_70_VER_PREFAB_NAME = "_70_vertical";
        private const string DEFAULT_80_VER_PREFAB_NAME = "_80_vertical";
        private const string DEFAULT_90_VER_PREFAB_NAME = "_90_vertical";
        private const string DEFAULT_HOR_PREFAB_NAME    = DEFAULT_80_HOR_PREFAB_NAME;
        private const string DEFAULT_VER_PREFAB_NAME    = DEFAULT_80_VER_PREFAB_NAME;

        public IEnumerable<AssetAddress> GetAddressesToTry(ModelConfiguration cfg, PlatformConfiguration platform, GameConfiguration game)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id))
                return null;

            AssetAddresses addresses = new AssetAddresses(FILE_EXTENSION, ADDRESSABLES_PREFIX);

            addresses.Add(cfg.Overrides.Model);
            addresses.Add(cfg.Id);
            addresses.Add(cfg.Overrides.Game?.CloneOf);
            addresses.Add(cfg.Overrides.Game?.RomOf);

            addresses.Add(game?.CloneOf);
            addresses.Add(game?.RomOf);

            addresses.Add(platform?.Model);

            // Generic model from orientation/year
            if (cfg.Overrides.Game != null && !string.IsNullOrEmpty(cfg.Overrides.Game.Year))
            {
                string year = cfg.Overrides.Game.Year;
                switch (cfg.Overrides.Game.ScreenOrientation)
                {
                    case GameScreenOrientation.Default:
                        if (!string.IsNullOrEmpty(GetName(game)))
                            return addresses.Addresses;
                        break;
                    case GameScreenOrientation.Horizontal:
                    case GameScreenOrientation.FlippedHorizontal:
                        addresses.Add(GetHorizontalNameForYear(year));
                        return addresses.Addresses;
                    case GameScreenOrientation.Vertical:
                    case GameScreenOrientation.FlippedVertical:
                        addresses.Add(GetVerticalNameForYear(year));
                        return addresses.Addresses;
                    default:
                        throw new System.Exception($"Unhandled switch case for GameScreenOrientation: {cfg.Overrides.Game.ScreenOrientation}");
                }
            }

            addresses.Add(GetName(game));

            addresses.Add(DEFAULT_HOR_PREFAB_NAME);

            return addresses.Addresses;
        }

        private static string GetName(GameConfiguration game)
        {
            if (game == null)
                return null;

            switch (game.ScreenOrientation)
            {
                case GameScreenOrientation.Default:
                case GameScreenOrientation.Horizontal:
                case GameScreenOrientation.FlippedHorizontal:
                    return GetHorizontalNameForYear(game.Year);
                case GameScreenOrientation.Vertical:
                case GameScreenOrientation.FlippedVertical:
                    return GetVerticalNameForYear(game.Year);
                default:
                    throw new System.NotImplementedException($"Unhandled switch case for GameScreenOrientation: {game.ScreenOrientation}");
            }
        }

        private static string GetHorizontalNameForYear(string yearString) => GetNameForYear(yearString,
                                                                                            DEFAULT_70_HOR_PREFAB_NAME,
                                                                                            DEFAULT_80_HOR_PREFAB_NAME,
                                                                                            DEFAULT_90_HOR_PREFAB_NAME,
                                                                                            DEFAULT_HOR_PREFAB_NAME);

        private static string GetVerticalNameForYear(string yearString) => GetNameForYear(yearString,
                                                                                          DEFAULT_70_VER_PREFAB_NAME,
                                                                                          DEFAULT_80_VER_PREFAB_NAME,
                                                                                          DEFAULT_90_VER_PREFAB_NAME,
                                                                                          DEFAULT_VER_PREFAB_NAME);

        private static string GetNameForYear(string yearString, string model70, string model80, string model90, string modelDefault)
        {
            if (string.IsNullOrEmpty(yearString) && int.TryParse(yearString, out int year) && year > 0)
            {
                if (year >= 1970 && year < 1980)
                    return GetAssetPath(model70);

                if (year < 1990)
                    return GetAssetPath(model80);

                if (year < 2000)
                    return GetAssetPath(model90);
            }

            return GetAssetPath(modelDefault);
        }

        private static string GetAssetPath(string name)
        {
            if (!Application.isPlaying)
                return $"{AssetAddresses.EDITOR_ADDRESSABLES_PATH}{ADDRESSABLES_PREFIX}{name}.{FILE_EXTENSION}";

            return $"{ADDRESSABLES_PREFIX}/{name}";
        }
    }
}
