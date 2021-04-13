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

namespace Arcade
{
    public sealed class ArtworkFileNamesProvider
    {
        private readonly MultiFileDatabase<EmulatorConfiguration> _emulatorDatabase;
        private readonly MultiFileDatabase<PlatformConfiguration> _platformDatabase;
        private readonly GameDatabase _gameDatabase;

        public ArtworkFileNamesProvider(MultiFileDatabase<EmulatorConfiguration> emulatorDatabase,
                                    MultiFileDatabase<PlatformConfiguration> platformDatabase,
                                    GameDatabase gameDatabase)
        {
            _emulatorDatabase = emulatorDatabase;
            _platformDatabase = platformDatabase;
            _gameDatabase     = gameDatabase;
        }

        public string[] GetNamesToTry(ModelConfiguration cfg)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id))
                return null;

            ImageSequence imageSequence = new ImageSequence();

            // From files overrides


            // From directories overrides


            imageSequence.Add(cfg.Id);

            // From game CloneOf and RomOf in platform's masterlist
            if (_platformDatabase.TryGet(cfg.Platform, out PlatformConfiguration platform))
            {
                if (_gameDatabase.TryGet(platform.MasterList, cfg.Id, new string[] { "CloneOf", "RomOf" }, new string[] { "Name" }, out GameConfiguration game))
                {
                    imageSequence.Add(game.CloneOf);
                    imageSequence.Add(game.RomOf);
                }
            }

            // From emulator override
            if (_emulatorDatabase.TryGet(cfg.Overrides.Emulator, out EmulatorConfiguration emulator))
                imageSequence.Add(emulator.Id);

            // From platform emulator
            if (_emulatorDatabase.TryGet(platform?.Emulator, out emulator))
                imageSequence.Add(emulator.Id);

            // From platform id
            imageSequence.Add(platform?.Id);

            return imageSequence.Images;
        }
    }
}
