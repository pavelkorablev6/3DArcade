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
        private readonly IArtworkFileNamesProvider _fileNamesProvider;
        private readonly IArtworkDirectoriesProvider _directoryNamesProvider;
        private readonly ArtworkController _artworkController;

        private readonly System.Threading.SynchronizationContext _mainThread;

        public NodeController(IArtworkFileNamesProvider fileNamesProvider,
                              IArtworkDirectoriesProvider directoryNamesProvider,
                              ArtworkController artworkController)
        {
            _fileNamesProvider      = fileNamesProvider;
            _directoryNamesProvider = directoryNamesProvider;
            _artworkController      = artworkController;

            _mainThread = System.Threading.SynchronizationContext.Current;
        }

        public void Setup(GameObject gameObject, ModelConfiguration modelConfiguration, float emissionIntensity)
        {
            if (gameObject == null || modelConfiguration == null)
                return;

            Renderer[] renderers = GetNodeRenderers(gameObject);
            if (renderers == null)
                return;

            _artworkController.Initialize();
            _directoryNamesProvider.Initialize();

            _mainThread.Post(x =>
            {
                string[] namesToTry = _fileNamesProvider.GetNamesToTry(modelConfiguration);
                _artworkController.SetupImages(_directoryNamesProvider, modelConfiguration, namesToTry, renderers, emissionIntensity);
                //_artworkController.SetupVideos(_directoryNamesProvider, modelConfiguration, namesToTry, renderers, arcadeController.AudioMinDistance, arcadeController.AudioMaxDistance, arcadeController.VolumeCurve);
            }, this);
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
