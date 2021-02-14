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
    public sealed class ArtworkMatcher
    {
        private const int NUM_MAX_VARIANTS = 20;

        private readonly XMLDatabaseMultiFile<EmulatorConfiguration> _emulatorDatabase;
        private readonly PlatformDatabase _platformDatabase;

        public ArtworkMatcher(XMLDatabaseMultiFile<EmulatorConfiguration> emulatorDatabase, PlatformDatabase platformDatabase)
        {
            _emulatorDatabase = emulatorDatabase;
            _platformDatabase = platformDatabase;
        }

        public List<string> GetDirectoriesToTry(string[] gameArtworkDirectories, string[] platformArtworkDirectories, string[] defaultArtworkDirectories)
        {
            List<string> result = new List<string>();
            AddPossibleDirectories(gameArtworkDirectories);
            AddPossibleDirectories(platformArtworkDirectories);
            AddPossibleDirectories(defaultArtworkDirectories);
            return result;

            void AddPossibleDirectories(string[] directories)
            {
                if (directories != null)
                    foreach (string directory in directories)
                        result.AddStringIfNotNullOrEmpty(FileSystem.CorrectPath(directory));
            }
        }

        public List<string> GetNamesToTry(ModelConfiguration cfg)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id))
                return null;

            List<string> result = new List<string>();

            // From id
            AddPossibleVariants(cfg.Id);

            PlatformConfiguration platform = _platformDatabase.GetPlatformForConfiguration(cfg);

            // TODO: From game CloneOf and RomOf in platform's masterlist
            if (!string.IsNullOrEmpty(cfg.Platform))
            {
                if (platform != null && !string.IsNullOrEmpty(platform.MasterList))
                {
                    //GameConfiguration game = _gameDatabase.Get(platform.MasterList, game.Id);
                    //if (game != null)
                    //{
                    //    AddPossibleVariants(result, game.CloneOf);
                    //    AddPossibleVariants(result, game.RomOf);
                    //}
                }
            }

            // From emulator override
            if (!string.IsNullOrEmpty(cfg.Emulator) && _emulatorDatabase.Get(cfg.Emulator, out EmulatorConfiguration emulator))
                AddPossibleVariants(emulator.Id);

            // From platform emulator
            if (platform != null)
            {
                if (!string.IsNullOrEmpty(platform.Emulator) && _emulatorDatabase.Get(platform.Emulator, out emulator))
                    AddPossibleVariants(emulator.Id);

                // From platform id
                AddPossibleVariants(platform.Id);
            }

            return result;

            void AddPossibleVariants(string name)
            {
                if (string.IsNullOrEmpty(name))
                    return;

                result.Add(name);

                for (int i = 0; i < NUM_MAX_VARIANTS; ++i)
                    result.Add($"{name}_{i}");
            }
        }
    }
}
