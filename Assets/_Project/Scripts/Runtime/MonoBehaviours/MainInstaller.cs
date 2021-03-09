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

using Zenject;

namespace Arcade
{
    public sealed class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            _ = Container.Bind<InputActions>().FromNew().AsSingle().NonLazy();

            _ = Container.Bind<Player>().FromInstance(FindObjectOfType<Player>()).AsSingle();

            _ = Container.Bind<IUIController>().FromInstance(new UIController()).AsSingle().NonLazy();

            string dataPath = SystemUtils.GetDataPath();
            IVirtualFileSystem virtualFileSystem = new VirtualFileSystem()
                .MountFile("general_cfg", $"{dataPath}/3darcade~/Configuration/GeneralConfiguration.xml")
                .MountDirectory("emulator_cfgs", $"{dataPath}/3darcade~/Configuration/Emulators")
                .MountDirectory("platform_cfgs", $"{dataPath}/3darcade~/Configuration/Platforms")
                .MountDirectory("arcade_cfgs", $"{dataPath}/3darcade~/Configuration/Arcades")
                .MountDirectory("gamelist_cfgs", $"{dataPath}/3darcade~/Configuration/Gamelists")
                .MountDirectory("medias", $"{dataPath}/3darcade~/Media");
            _ = Container.Bind<IVirtualFileSystem>().FromInstance(virtualFileSystem).AsSingle().NonLazy();

            _ = Container.Bind<GeneralConfiguration>().FromInstance(new GeneralConfiguration(virtualFileSystem)).AsSingle().NonLazy();

            _ = Container.Bind<MultiFileDatabase<EmulatorConfiguration>>().To<EmulatorDatabase>().FromNew().AsSingle().NonLazy();
            _ = Container.Bind<MultiFileDatabase<PlatformConfiguration>>().To<PlatformDatabase>().FromNew().AsSingle().NonLazy();
            _ = Container.Bind<MultiFileDatabase<ArcadeConfiguration>>().To<ArcadeDatabase>().FromNew().AsSingle().NonLazy();

            _ = Container.Bind<SceneContext>().FromNew().AsSingle().NonLazy();
        }
    }
}
