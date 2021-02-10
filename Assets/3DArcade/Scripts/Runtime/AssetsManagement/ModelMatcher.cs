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

        private const string DEFAULT_ARCADE_MODEL    = "defaultCylinder";
        private const string DEFAULT_GAME_HOR_MODEL  = "magic-deniro-80-hor";
        private const string DEFAULT_GAME_VERT_MODEL = "default80vert";
        private const string DEFAULT_PROP_MODEL      = "penguin";

        private readonly XMLDatabase<EmulatorConfiguration> _emulatorDatabase;
        private readonly XMLDatabase<PlatformConfiguration> _platformDatabase;

        public ModelMatcher(XMLDatabase<EmulatorConfiguration> emulatorDatabase, XMLDatabase<PlatformConfiguration> platformDatabase)
        {
            _emulatorDatabase = emulatorDatabase ?? throw new System.ArgumentNullException(nameof(emulatorDatabase));
            _platformDatabase = platformDatabase ?? throw new System.ArgumentNullException(nameof(platformDatabase));
        }

        public EmulatorConfiguration GetEmulatorForConfiguration(ModelConfiguration modelConfiguration)
            => modelConfiguration.InteractionType switch
            {
                InteractionType.GameInternal           => _emulatorDatabase.Get(modelConfiguration.Emulator),
                InteractionType.GameExternal           => _emulatorDatabase.Get(modelConfiguration.Emulator),
                InteractionType.URL                    => _emulatorDatabase.Get(modelConfiguration.Emulator),
                InteractionType.FpsArcadeConfiguration => EmulatorDatabase.FpsArcadeLauncher,
                InteractionType.CylArcadeConfiguration => EmulatorDatabase.CylArcadeLauncher,
                InteractionType.FpsMenuConfiguration   => EmulatorDatabase.FpsMenuLauncher,
                InteractionType.CylMenuConfiguration   => EmulatorDatabase.CylMenuLauncher,
                _                                      => null,
            };

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

            // From model's model override
            result.AddStringIfNotNullOrEmpty(cfg.Model);

            // From model's id
            result.Add(cfg.Id);

            PlatformConfiguration platform = null;
            GameConfiguration game         = null;

            if (!string.IsNullOrEmpty(cfg.Platform))
                platform = _platformDatabase.Get(cfg.Platform);

            // TODO: From game's CloneOf and RomOf in platform's masterlist
            if (platform != null && !string.IsNullOrEmpty(platform.MasterList))
            {
                //game = _gameDatabase.Get(platform.MasterList, game.Id);
                if (game != null)
                {
                    //result.AddStringIfNotNullOrEmpty(game.CloneOf);
                    //result.AddStringIfNotNullOrEmpty(game.RomOf);
                }
            }

            // From model's emulator's model and id
            if (!string.IsNullOrEmpty(cfg.Emulator) && _emulatorDatabase.Get(cfg.Emulator, out EmulatorConfiguration emulator))
            {
                result.AddStringIfNotNullOrEmpty(emulator.Model);
                result.AddStringIfNotNullOrEmpty(emulator.Id);
            }

            if (platform != null)
            {
                // From platform's model and id
                result.AddStringIfNotNullOrEmpty(platform.Model);
                result.AddStringIfNotNullOrEmpty(platform.Id);

                // From platform's emulator's model and id
                if (!string.IsNullOrEmpty(platform.Emulator) && _emulatorDatabase.Get(platform.Emulator, out emulator))
                {
                    result.AddStringIfNotNullOrEmpty(emulator.Model);
                    result.AddStringIfNotNullOrEmpty(emulator.Id);
                }
            }

            // Generic model from orientation/year
            if (game != null)
            {
                bool isVertical = game.ScreenOrientation == GameScreenOrientation.Vertical;
                string prefabName;
                if (int.TryParse(game.Year, out int year))
                {
                    if (year >= 1970 && year < 1980)
                        prefabName = isVertical ? "default70vert" : "default70hor";
                    else if (year < 1990)
                        prefabName = isVertical ? "default80vert" : "default80hor";
                    else if (year < 2000)
                        prefabName = isVertical ? "default90vert" : "default90hor";
                    else
                        prefabName = isVertical ? DEFAULT_GAME_VERT_MODEL : DEFAULT_GAME_HOR_MODEL;
                }
                else
                    prefabName = isVertical ? DEFAULT_GAME_VERT_MODEL : DEFAULT_GAME_HOR_MODEL;

                result.Add(prefabName);
            }

            // Default horizontal model
            result.Add(DEFAULT_GAME_HOR_MODEL);

            return result;
        }

        public static List<string> GetNamesToTryForProp(ModelConfiguration cfg)
            => new List<string> { cfg.Model, cfg.Id, DEFAULT_PROP_MODEL };
    }
}
