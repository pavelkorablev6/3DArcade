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
    public sealed class SceneContextData
    {
        public readonly Player Player;
        public readonly InputActions InputActions;
        public readonly IUIController UIController;
        public readonly GeneralConfiguration GeneralConfiguration;
        public readonly MultiFileDatabase<EmulatorConfiguration> EmulatorDatabase;
        public readonly MultiFileDatabase<PlatformConfiguration> PlatformDatabase;
        public readonly MultiFileDatabase<ArcadeConfiguration> ArcadeDatabase;

        public SceneContextData(Player player,
                                InputActions inputActions,
                                IUIController uIController,
                                GeneralConfiguration generalConfiguration,
                                MultiFileDatabase<EmulatorConfiguration> emulatorDatabase,
                                MultiFileDatabase<PlatformConfiguration> platformDatabase,
                                MultiFileDatabase<ArcadeConfiguration> arcadeDatabase)
        {
            Player               = player;
            InputActions         = inputActions;
            UIController         = uIController;
            GeneralConfiguration = generalConfiguration;
            EmulatorDatabase     = emulatorDatabase;
            PlatformDatabase     = platformDatabase;
            ArcadeDatabase       = arcadeDatabase;
        }
    }
}
