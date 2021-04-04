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

using System.Collections.Generic;
using UnityEngine;

namespace Arcade
{
    public sealed class NodeController<T> where T : NodeTag
    {
        private readonly System.Threading.SynchronizationContext _mainThread;

        private readonly IArtworkDirectoryNamesProvider _directoryNamesProvider;
        private readonly ArtworkFileNamesProvider _artworkNameProvider;
        private readonly ArtworkController _artworkController;
        private readonly MultiFileDatabase<PlatformConfiguration> _platformDatabase;

        public NodeController(IArtworkDirectoryNamesProvider directoryNamesProvider, ArtworkFileNamesProvider artworkNameProvider, ArtworkController artworkController, MultiFileDatabase<PlatformConfiguration> platformDatabase)
        {
            _mainThread = System.Threading.SynchronizationContext.Current;

            _directoryNamesProvider = directoryNamesProvider;
            _artworkNameProvider    = artworkNameProvider;
            _artworkController      = artworkController;
            _platformDatabase       = platformDatabase;
        }

        public void Setup(GameObject gameObject, ModelConfiguration modelConfiguration, float emissionIntensity)
        {
            if (gameObject == null)
                return;

            Renderer[] renderers = GetNodeRenderers(gameObject);
            if (renderers == null)
                return;

            string[] namesToTry = _artworkNameProvider.GetNamesToTry(modelConfiguration);

            PlatformConfiguration platform = !string.IsNullOrEmpty(modelConfiguration.Platform) ? _platformDatabase.Get(modelConfiguration.Platform) : null;

            string[] gameArtworkDirectories     = _directoryNamesProvider.GetModelImageDirectories(modelConfiguration);
            string[] platformArtworkDirectories = _directoryNamesProvider.GetPlatformImageDirectories(platform);
            string[] defaultArtworkDirectories  = _directoryNamesProvider.DefaultImageDirectories;
            string[] imagesDirectories          = ArtworkDirectoriesResolver.GetDirectoriesToTry(gameArtworkDirectories, platformArtworkDirectories, defaultArtworkDirectories);

            _mainThread.Post(x => _artworkController.SetupImages(imagesDirectories, namesToTry, renderers, emissionIntensity), this);

            string[] gameVideoDirectories     = _directoryNamesProvider.GetModelVideoDirectories(modelConfiguration);
            string[] platformVideoDirectories = _directoryNamesProvider.GetPlatformVideoDirectories(platform);
            string[] defaultVideoDirectories  = _directoryNamesProvider.DefaultVideoDirectories;
            string[] videosDirectories        = ArtworkDirectoriesResolver.GetDirectoriesToTry(gameVideoDirectories, platformVideoDirectories, defaultVideoDirectories);
            //_mainThread.Post(x => _artworkController.SetupVideos(directories, namesToTry, renderers, arcadeController.AudioMinDistance, arcadeController.AudioMaxDistance, arcadeController.VolumeCurve), this);
        }

        private static Renderer[] GetNodeRenderers(GameObject model)
        {
            T[] nodeTags = model.GetComponentsInChildren<T>(true);
            if (nodeTags == null || nodeTags.Length == 0)
                return null;

            List<Renderer> renderers = new List<Renderer>();

            foreach (T nodeTag in nodeTags)
            {
                if (nodeTag.TryGetComponent(out Renderer renderer))
                    renderers.Add(renderer);
            }

            return renderers.Count > 0 ? renderers.ToArray() : null;
        }
    }
}
