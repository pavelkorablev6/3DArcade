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
    public abstract class NodeController<T> where T : NodeTag
    {
        protected static readonly string _defaultMediaDirectory = $"{SystemUtils.GetDataPath()}/3darcade~/Configuration/Media";

        protected abstract string[] DefaultImageDirectories { get; }
        protected abstract string[] DefaultVideoDirectories { get; }

        private static XMLDatabaseMultiFile<EmulatorConfiguration> _emulatorDatabase;
        private static XMLDatabaseMultiFile<PlatformConfiguration> _platformDatabase;

        private readonly ArtworkMatcher _artworkMatcher;

        public NodeController(XMLDatabaseMultiFile<EmulatorConfiguration> emulatorDatabase, XMLDatabaseMultiFile<PlatformConfiguration> platformDatabase)
        {
            if (_emulatorDatabase == null)
            {
                _emulatorDatabase = emulatorDatabase;
                _platformDatabase = platformDatabase;
            }
            _artworkMatcher = new ArtworkMatcher(emulatorDatabase, platformDatabase);
        }

        public void Setup(ArcadeController arcadeController, GameObject model, ModelConfiguration modelConfiguration, float emissionIntensity)
        {
            Renderer[] renderers = GetNodeRenderers(model);
            if (renderers == null)
                return;

            foreach (Renderer renderer in renderers)
            {
                renderer.material.EnableEmission(true);
                renderer.material.SetBaseColor(Color.black);
                renderer.material.SetBaseTexture(null);
                renderer.material.SetEmissionColor(Color.white * emissionIntensity);
            }

            List<string> namesToTry = _artworkMatcher.GetNamesToTry(modelConfiguration);

            PlatformConfiguration platform = !string.IsNullOrEmpty(modelConfiguration.Platform) ? _platformDatabase.Get(modelConfiguration.Platform) : null;

            List<string> directories = _artworkMatcher.GetDirectoriesToTry(GetModelImageDirectories(modelConfiguration), GetPlatformImageDirectories(platform), DefaultImageDirectories);
            ArtworkUtils.SetupImages(directories, namesToTry, renderers, this as MarqueeNodeController != null);

            directories = _artworkMatcher.GetDirectoriesToTry(GetModelVideoDirectories(modelConfiguration), GetPlatformVideoDirectories(platform), DefaultVideoDirectories);
            ArtworkUtils.SetupVideos(directories, namesToTry, renderers, arcadeController.AudioMinDistance, arcadeController.AudioMaxDistance, arcadeController.VolumeCurve);
        }

        protected abstract string[] GetModelImageDirectories(ModelConfiguration modelConfiguration);

        protected abstract string[] GetModelVideoDirectories(ModelConfiguration modelConfiguration);

        protected abstract string[] GetPlatformImageDirectories(PlatformConfiguration platform);

        protected abstract string[] GetPlatformVideoDirectories(PlatformConfiguration platform);

        private static Renderer[] GetNodeRenderers(GameObject model)
        {
            if (model == null)
                return null;

            List<Renderer> renderers = new List<Renderer>();

            T[] nodeTags = model.GetComponentsInChildren<T>();
            if (nodeTags != null)
            {
                foreach (T nodeTag in nodeTags)
                {
                    Renderer renderer = nodeTag.GetComponent<Renderer>();
                    if (renderer != null)
                        renderers.Add(renderer);
                }
            }

            return renderers.Count > 0 ? renderers.ToArray() : null;
        }
    }
}
