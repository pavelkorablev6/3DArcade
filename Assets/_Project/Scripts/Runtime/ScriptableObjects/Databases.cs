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

using UnityEngine;
using Zenject;

namespace Arcade
{
    [CreateAssetMenu(menuName = "Arcade/Databases", fileName = "Databases")]
    public sealed class Databases : ScriptableObject
    {
        public MultiFileDatabase<EmulatorConfiguration> Emulators { get; private set; }
        public MultiFileDatabase<PlatformConfiguration> Platforms { get; private set; }
        public MultiFileDatabase<ArcadeConfiguration> Arcades { get; private set; }
        public GameDatabase Games { get; private set; }

        [Inject]
        public void Construct(MultiFileDatabase<EmulatorConfiguration> emulatorDatabase,
                              MultiFileDatabase<PlatformConfiguration> platformDatabase,
                              MultiFileDatabase<ArcadeConfiguration> arcadeDatabase,
                              GameDatabase gameDatabase)
        {
            Emulators = emulatorDatabase;
            Platforms = platformDatabase;
            Arcades   = arcadeDatabase;
            Games     = gameDatabase;
        }

        public void Initialize()
        {
            Emulators.Initialize();
            Platforms.Initialize();
            Arcades.Initialize();
            Games.Initialize();
        }
    }
}
