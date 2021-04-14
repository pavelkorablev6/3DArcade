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
    public sealed class ArtworkFileNamesProvider : IArtworkFileNamesProvider
    {
        private readonly MultiFileDatabase<PlatformConfiguration> _platformDatabase;
        private readonly MultiFileDatabase<EmulatorConfiguration> _emulatorDatabase;
        private readonly GameDatabase _gameDatabase;

        public ArtworkFileNamesProvider(MultiFileDatabase<PlatformConfiguration> platformDatabase, MultiFileDatabase<EmulatorConfiguration> emulatorDatabase, GameDatabase gameDatabase)
        {
            _platformDatabase = platformDatabase;
            _emulatorDatabase = emulatorDatabase;
            _gameDatabase     = gameDatabase;
        }

        public string[] GetNamesToTry(ModelConfiguration cfg)
        {
            if (cfg == null)
                return null;

            _ = _platformDatabase.TryGet(cfg.Platform, out PlatformConfiguration platform);
            _ = _emulatorDatabase.TryGet(cfg.Overrides.Emulator, out EmulatorConfiguration overrideEmulator);
            _ = _emulatorDatabase.TryGet(platform?.Emulator, out EmulatorConfiguration platformEmulator);
            _ = _gameDatabase.TryGet(platform?.MasterList, cfg.Id, new string[] { "CloneOf", "RomOf" }, new string[] { "Name" }, out GameConfiguration game);

            ImageSequence imageSequence = new ImageSequence();

            // TODO: From files overrides

            // TODO: From directories overrides

            imageSequence.Add(cfg.Id);

            imageSequence.Add(cfg.Overrides.Game.CloneOf);
            imageSequence.Add(cfg.Overrides.Game.RomOf);

            imageSequence.Add(game?.CloneOf);
            imageSequence.Add(game?.RomOf);

            imageSequence.Add(overrideEmulator?.Id);

            imageSequence.Add(platformEmulator?.Id);

            imageSequence.Add(platform?.Id);

            return imageSequence.Images;
        }
    }
}
