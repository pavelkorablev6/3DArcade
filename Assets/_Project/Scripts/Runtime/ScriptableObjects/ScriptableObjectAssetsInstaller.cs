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
    [CreateAssetMenu(menuName = "Arcade/ScriptableObjectInstaller", fileName = "ScriptableObjectInstaller")]
    public sealed class ScriptableObjectAssetsInstaller : ScriptableObjectInstaller<ScriptableObjectAssetsInstaller>
    {
        [SerializeField] private VirtualFileSystem _virtualFileSystem;
        [SerializeField] private GeneralConfigurationVariable _generalConfigurationVariable;
        [SerializeField] private Databases _databases;

        public override void InstallBindings()
        {
            _ = Container.Bind<VirtualFileSystem>().FromScriptableObject(_virtualFileSystem).AsSingle();
            _ = Container.Bind<GeneralConfigurationVariable>().FromScriptableObject(_generalConfigurationVariable).AsSingle();

            _ = Container.Bind<MultiFileDatabase<EmulatorConfiguration>>().To<EmulatorDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<MultiFileDatabase<PlatformConfiguration>>().To<PlatformDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<MultiFileDatabase<ArcadeConfiguration>>().To<ArcadeDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<GameDatabase>().AsSingle().NonLazy();

            _ = Container.Bind<Databases>().FromScriptableObject(_databases).AsSingle();
        }
    }
}
