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
        [SerializeField] private NormalModeInteractionRaycaster _normalModeInteractionRaycaster;
        [SerializeField] private NormalModeInteractionController _normalModeInteractionController;
        [SerializeField] private EditModeInteractionRaycaster _editModeInteractionRaycaster;
        [SerializeField] private EditModeInteractionController _editModeInteractionController;
        [SerializeField] private TypeEvent _uiStateTransitionEvent;
        [SerializeField] private StringVariable _arcadeNameVariable;
        [SerializeField] private BoolVariable _mouseOverUIVariable;
        [SerializeField] private ArcadeContext _arcadeContext;

        public override void InstallBindings()
        {
            _ = Container.Bind<InputActions>().AsSingle().NonLazy();

            _ = Container.Bind<VirtualFileSystem>().FromScriptableObject(_virtualFileSystem).AsSingle();
            _ = Container.Bind<GeneralConfigurationVariable>().FromScriptableObject(_generalConfigurationVariable).AsSingle();

            _ = Container.Bind<MultiFileDatabase<EmulatorConfiguration>>().To<EmulatorDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<MultiFileDatabase<PlatformConfiguration>>().To<PlatformDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<MultiFileDatabase<ArcadeConfiguration>>().To<ArcadeDatabase>().AsSingle().NonLazy();
            _ = Container.Bind<GameDatabase>().AsSingle().NonLazy();

            _ = Container.Bind<Databases>().FromScriptableObject(_databases).AsSingle();

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

            _ = Container.Bind<NormalModeInteractionRaycaster>().FromScriptableObject(_normalModeInteractionRaycaster).AsSingle();
            _ = Container.Bind<NormalModeInteractionController>().FromScriptableObject(_normalModeInteractionController).AsSingle();
            _ = Container.Bind<EditModeInteractionRaycaster>().FromScriptableObject(_editModeInteractionRaycaster).AsSingle();
            _ = Container.Bind<EditModeInteractionController>().FromScriptableObject(_editModeInteractionController).AsSingle();
            _ = Container.Bind<InteractionControllers>().AsSingle().NonLazy();

            _ = Container.Bind<Material>().FromResource("Materials/_libretroScreen").WhenInjectedInto<InternalGameController>();
            _ = Container.Bind<Material>().FromResource("Materials/_uddScreen").WhenInjectedInto<ExternalGameController>();
            _ = Container.Bind<ExternalGameController>().AsSingle().NonLazy();
            _ = Container.Bind<InternalGameController>().AsSingle().NonLazy();
            _ = Container.Bind<GameControllers>().AsSingle().NonLazy();

            _ = Container.Bind<TypeEvent>().FromScriptableObject(_uiStateTransitionEvent).AsSingle();
            _ = Container.Bind<StringVariable>().WithId("arcade_name").FromScriptableObject(_arcadeNameVariable).AsSingle();
            _ = Container.Bind<BoolVariable>().WithId("mouse_over_ui").FromScriptableObject(_mouseOverUIVariable).AsSingle();

            _ = Container.Bind<ArcadeContext>().FromScriptableObject(_arcadeContext).AsSingle();
        }
    }
}
