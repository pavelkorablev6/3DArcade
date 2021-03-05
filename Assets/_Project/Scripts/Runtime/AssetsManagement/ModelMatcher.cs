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

namespace Arcade
{
    public sealed class ModelMatcher
    {
        public delegate List<string> GetNamesToTryDelegate(ModelConfiguration modelConfiguration);

        private const string DEFAULT_ARCADE_MODEL      = "internal_cylinder";
        private const string DEFAULT_GAME_70_HOR_MODEL = "internal_70_horizontal";
        private const string DEFAULT_GAME_80_HOR_MODEL = "internal_80_horizontal";
        private const string DEFAULT_GAME_90_HOR_MODEL = "internal_90_horizontal";
        private const string DEFAULT_GAME_70_VER_MODEL = "internal_70_vertical";
        private const string DEFAULT_GAME_80_VER_MODEL = "internal_80_vertical";
        private const string DEFAULT_GAME_90_VER_MODEL = "internal_90_vertical";
        private const string DEFAULT_GAME_HOR_MODEL    = DEFAULT_GAME_80_HOR_MODEL;
        private const string DEFAULT_GAME_VER_MODEL    = DEFAULT_GAME_80_VER_MODEL;
        private const string DEFAULT_PROP_MODEL        = "internal_penguin";

        private readonly PlatformDatabase _platformDatabase;

        public ModelMatcher(PlatformDatabase platformDatabase)
            => _platformDatabase = platformDatabase ?? throw new System.ArgumentNullException(nameof(platformDatabase));

        public static List<string> GetNamesToTryForArcade(ModelConfiguration cfg)
            => new List<string>
            {
                cfg.Model,
                cfg.Id,
                DEFAULT_ARCADE_MODEL
            };

        public List<string> GetNamesToTryForGame(ModelConfiguration cfg)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id))
                return null;

            List<string> result = new List<string>();

            // From cfg model override
            result.AddStringIfNotNullOrEmpty(cfg.Model);

            // From cfg id
            result.Add(cfg.Id);

            PlatformConfiguration platform = _platformDatabase.GetPlatformForConfiguration(cfg);

            // TODO: From game CloneOf and RomOf in platform's masterlist
            GameConfiguration game = null;
            if (platform != null && !string.IsNullOrEmpty(platform.MasterList))
            {
                //game = _gameDatabase.Get(platform.MasterList, game.Id);
                //if (game != null)
                //{
                //    result.AddStringIfNotNullOrEmpty(game.CloneOf);
                //    result.AddStringIfNotNullOrEmpty(game.RomOf);
                //}
            }

            // From platform model
            result.AddStringIfNotNullOrEmpty(platform?.Model);

            // Generic model from cfg orientation/year
            if (!string.IsNullOrEmpty(cfg.Year))
            {
                switch (cfg.ScreenOrientation)
                {
                    case GameScreenOrientation.Undefined:
                    case GameScreenOrientation.Inherited:
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
                        throw new System.Exception($"Unhandled switch case for GameScreenOrientation: {cfg.ScreenOrientation}");
                }
            }

            // Generic model from game orientation/year
            string modelName = GetModelNameForGame(game);
            if (!string.IsNullOrEmpty(modelName))
            {
                result.Add(modelName);
                return result;
            }

            // Default horizontal model
            result.Add(DEFAULT_GAME_HOR_MODEL);
            return result;
        }

        public static List<string> GetNamesToTryForProp(ModelConfiguration cfg)
            => new List<string> { cfg.Model, cfg.Id, DEFAULT_PROP_MODEL };

        private static string GetModelNameForGame(GameConfiguration game)
        {
            if (game == null)
                return null;
            return game.ScreenOrientation == GameScreenOrientation.Vertical
                   ? GetVerticalModelNameForYear(game.Year)
                   : GetHorizontalModelNameForYear(game.Year);
        }

        private static string GetHorizontalModelNameForYear(string yearString)
        {
            if (!string.IsNullOrEmpty(yearString) && int.TryParse(yearString, out int year) && year > 0)
            {
                if (year >= 1970 && year < 1980)
                    return DEFAULT_GAME_70_HOR_MODEL;
                else if (year < 1990)
                    return DEFAULT_GAME_80_HOR_MODEL;
                else if (year < 2000)
                    return DEFAULT_GAME_90_HOR_MODEL;
            }
            return DEFAULT_GAME_HOR_MODEL;
        }

        private static string GetVerticalModelNameForYear(string yearString)
        {
            if (!string.IsNullOrEmpty(yearString) && int.TryParse(yearString, out int year) && year > 0)
            {
                if (year >= 1970 && year < 1980)
                    return DEFAULT_GAME_70_VER_MODEL;
                else if (year < 1990)
                    return DEFAULT_GAME_80_VER_MODEL;
                else if (year < 2000)
                    return DEFAULT_GAME_90_VER_MODEL;
            }
            return DEFAULT_GAME_VER_MODEL;
        }
    }
}
