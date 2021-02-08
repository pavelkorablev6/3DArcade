﻿/* MIT License

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

        private readonly EmulatorDatabase _emulatorDatabase;
        private readonly PlatformDatabase _platformDatabase;

        public ArtworkMatcher(EmulatorDatabase emulatorDatabase, PlatformDatabase platformDatabase)
        {
            _emulatorDatabase = emulatorDatabase;
            _platformDatabase = platformDatabase;
        }

        public List<string> GetDirectoriesToTry(string[] gameArtworkDirectories, string[] emulatorArtworkDirectories, string[] platformArtworkDirectories, string[] defaultArtworkDirectories)
        {
            List<string> result = new List<string>();

            if (gameArtworkDirectories != null)
                foreach (string directory in gameArtworkDirectories)
                    result.AddStringIfNotNullOrEmpty(FileSystem.CorrectPath(directory));

            if (emulatorArtworkDirectories != null)
                foreach (string directory in emulatorArtworkDirectories)
                    result.AddStringIfNotNullOrEmpty(FileSystem.CorrectPath(directory));

            if (platformArtworkDirectories != null)
                foreach (string directory in platformArtworkDirectories)
                    result.AddStringIfNotNullOrEmpty(FileSystem.CorrectPath(directory));

            if (defaultArtworkDirectories != null)
                foreach (string directory in defaultArtworkDirectories)
                    result.AddStringIfNotNullOrEmpty(FileSystem.CorrectPath(directory));

            return result;
        }

        public List<string> GetNamesToTry(ModelConfiguration model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
                return null;

            List<string> result = new List<string>();

            // From model's id
            AddPossibleVariants(result, model.Id);

            PlatformConfiguration platform = null;

            if (!string.IsNullOrEmpty(model.Platform))
                platform = _platformDatabase.Get(model.Platform);

            // TODO: From game's CloneOf and RomOf in platform's masterlist
            if (!string.IsNullOrEmpty(model.Platform))
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

            // From model's emulator override
            if (!string.IsNullOrEmpty(model.Emulator))
            {
                EmulatorConfiguration emulator = _emulatorDatabase.Get(model.Emulator);
                AddPossibleVariants(result, emulator?.Id);
            }

            // From platform's emulator
            if (platform != null)
            {
                EmulatorConfiguration emulator = _emulatorDatabase.Get(platform.Emulator);
                AddPossibleVariants(result, emulator?.Id);

                // From platform's id
                AddPossibleVariants(result, platform.Id);
            }

            return result;
        }

        private static void AddPossibleVariants(List<string> list, string name, int numVariants = NUM_MAX_VARIANTS)
        {
            if (list == null || string.IsNullOrEmpty(name))
                return;

            list.Add(name);

            for (int i = 0; i < numVariants; ++i)
                list.Add($"{name}_{i}");
        }
    }
}
