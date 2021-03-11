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
    public sealed class EmulatorDatabase : MultiFileDatabase<EmulatorConfiguration>
    {
        public static readonly EmulatorConfiguration FpsArcadeLauncher = MakeInternalLauncher("_fps_arcade_launcher", "FpsArcadeLauncher");
        public static readonly EmulatorConfiguration CylArcadeLauncher = MakeInternalLauncher("_cyl_arcade_launcher", "CylArcadeLauncher");
        public static readonly EmulatorConfiguration FpsMenuLauncher   = MakeInternalLauncher("_fps_menu_launcher", "FpsMenuLauncher");
        public static readonly EmulatorConfiguration CylMenuLauncher   = MakeInternalLauncher("_cyl_menu_launcher", "CylMenuLauncher");

        public EmulatorDatabase(IVirtualFileSystem virtualFileSystem)
        : base(virtualFileSystem, "emulator_cfgs")
            => _ = LoadAll();

        public EmulatorConfiguration GetEmulatorForModelConfiguration(ModelConfiguration cfg)
            => !string.IsNullOrEmpty(cfg.Emulator) ? Get(cfg.Emulator) : null;

        public EmulatorConfiguration GetEmulatorForPlatformConfiguration(PlatformConfiguration cfg)
            => !string.IsNullOrEmpty(cfg.Emulator) ? Get(cfg.Emulator) : null;

        private static EmulatorConfiguration MakeInternalLauncher(string id, string description) => new EmulatorConfiguration
        {
            Id                  = id,
            Description         = description,
            Directory           = null,
            WorkingDirectory    = null,
            Executable          = null,
            Arguments           = null,
            SupportedExtensions = null,
            GamesDirectories    = null
        };
    }
}
