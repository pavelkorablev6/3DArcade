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
    public sealed class MainInstaller : MonoInstaller<MainInstaller>
    {
        [SerializeField] private Player _player;
        [SerializeField] private NormalModeInteractionRaycaster _normalModeInteractionRaycaster;
        [SerializeField] private NormalModeInteractionController _normalModeInteractionController;
        [SerializeField] private EditModeInteractionRaycaster _editModeInteractionRaycaster;
        [SerializeField] private EditModeInteractionController _editModeInteractionController;
        [SerializeField] private UIManager _uiManager;

        public override void InstallBindings()
        {
            _ = Container.Bind<InputActions>().AsSingle().NonLazy();
            _ = Container.Bind<Player>().FromInstance(_player).AsSingle();

            _ = Container.Bind<IVirtualFileSystem>().To<VirtualFileSystem>().AsSingle().NonLazy();
            _ = Container.Bind<GeneralConfiguration>().AsSingle().NonLazy();
            _ = Container.Bind<MultiFileDatabase<EmulatorConfiguration>>().To<EmulatorDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<MultiFileDatabase<PlatformConfiguration>>().To<PlatformDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<MultiFileDatabase<ArcadeConfiguration>>().To<ArcadeDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<GameDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<Databases>().AsSingle().NonLazy();

            _ = Container.Bind<IEntititesSceneCreator>().To<EntitiesSceneCreator>().AsSingle().NonLazy();
            _ = Container.Bind<ArcadeSceneLoaderBase>().To<ArcadeSceneLoader>().AsSingle().NonLazy();
            _ = Container.Bind<EntitiesScene>().AsSingle().NonLazy();
            _ = Container.Bind<ArcadeScene>().AsSingle().NonLazy();
            _ = Container.Bind<Scenes>().AsSingle().NonLazy();

            _ = Container.Bind<IAssetAddressesProvider<ArcadeConfiguration>>().To<ArcadeSceneAddressesProvider>().AsSingle().NonLazy();
            _ = Container.Bind<IAssetAddressesProvider<ModelConfiguration>>().WithId("game").To<GamePrefabAddressesProvider>().AsSingle().NonLazy();
            _ = Container.Bind<IAssetAddressesProvider<ModelConfiguration>>().WithId("prop").To<PropPrefabAddressesProvider>().AsSingle().NonLazy();
            _ = Container.Bind<AssetAddressesProviders>().AsSingle().NonLazy();

            _ = Container.Bind<AssetCache<Texture>>().To<TextureCache>().AsSingle().NonLazy();
            _ = Container.Bind<ArtworkController>().AsSingle().NonLazy();

            _ = Container.Bind<IArtworkFileNamesProvider>().To<ArtworkFileNamesProvider>().AsSingle().NonLazy();
            _ = Container.Bind<IArtworkDirectoriesProvider>().To<MarqueeArtworkDirectoriesProvider>().AsSingle().WhenInjectedInto<NodeController<MarqueeNodeTag>>().NonLazy();
            _ = Container.Bind<IArtworkDirectoriesProvider>().To<ScreenArtworkDirectoriesProvider>().AsSingle().WhenInjectedInto<NodeController<ScreenNodeTag>>().NonLazy();
            _ = Container.Bind<IArtworkDirectoriesProvider>().To<GenericArtworkDirectoriesProvider>().AsSingle().WhenInjectedInto<NodeController<GenericNodeTag>>().NonLazy();
            _ = Container.Bind<NodeController<MarqueeNodeTag>>().AsSingle().NonLazy();
            _ = Container.Bind<NodeController<ScreenNodeTag>>().AsSingle().NonLazy();
            _ = Container.Bind<NodeController<GenericNodeTag>>().AsSingle().NonLazy();
            _ = Container.Bind<NodeControllers>().AsSingle().NonLazy();

            _ = Container.Bind<NormalModeInteractionRaycaster>().FromInstance(_normalModeInteractionRaycaster).AsSingle();
            _ = Container.Bind<NormalModeInteractionController>().FromInstance(_normalModeInteractionController).AsSingle();
            _ = Container.Bind<EditModeInteractionRaycaster>().FromInstance(_editModeInteractionRaycaster).AsSingle();
            _ = Container.Bind<EditModeInteractionController>().FromInstance(_editModeInteractionController).AsSingle();
            _ = Container.Bind<InteractionControllers>().AsSingle().NonLazy();

            _ = Container.Bind<UIManager>().FromInstance(_uiManager).AsSingle();

            _ = Container.Bind<Material>().WithId("LibretroScreenMaterial").FromResource("Materials/_libretroScreen");
            _ = Container.Bind<Material>().WithId("UDDScreenMaterial").FromResource("Materials/_uddScreen");
            _ = Container.Bind<ExternalGameController>().AsSingle().NonLazy();
            _ = Container.Bind<InternalGameController>().AsSingle().NonLazy();
            _ = Container.Bind<GameControllers>().AsSingle().NonLazy();

            _ = Container.Bind<ArcadeContext>().AsSingle().NonLazy();
        }
    }
}
