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

        protected readonly MultiFileDatabase<EmulatorConfiguration> _emulatorDatabase;
        protected readonly MultiFileDatabase<PlatformConfiguration> _platformDatabase;

        public NodeController(MultiFileDatabase<EmulatorConfiguration> emulatorDatabase, MultiFileDatabase<PlatformConfiguration> platformDatabase)
        {
            _emulatorDatabase = emulatorDatabase;
            _platformDatabase = platformDatabase;
        }

        public void Setup(ArcadeController arcadeController, ModelConfigurationComponent modelConfigurationComponent, float emissionIntensity)
        {
            Renderer[] renderers = GetNodeRenderers(modelConfigurationComponent.gameObject);
            if (renderers == null)
                return;

            //foreach (Renderer renderer in renderers)
            //{
            //    renderer.material.EnableEmission(true);
            //    renderer.material.SetBaseColor(Color.black);
            //    renderer.material.SetBaseTexture(null);
            //    renderer.material.SetEmissionColor(Color.white * emissionIntensity);
            //}

            //List<string> namesToTry = _artworkMatcher.GetNamesToTry(modelConfigurationComponent);

            //PlatformConfiguration platform = !string.IsNullOrEmpty(modelConfiguration.Platform) ? _platformDatabase.Get(modelConfiguration.Platform) : null;

            //List<string> directories = _artworkMatcher.GetDirectoriesToTry(GetModelImageDirectories(modelConfiguration), GetPlatformImageDirectories(platform), DefaultImageDirectories);
            //ArtworkUtils.SetupImages(directories, namesToTry, renderers, this is MarqueeNodeController);

            //directories = _artworkMatcher.GetDirectoriesToTry(GetModelVideoDirectories(modelConfiguration), GetPlatformVideoDirectories(platform), DefaultVideoDirectories);
            //ArtworkUtils.SetupVideos(directories, namesToTry, renderers, arcadeController.AudioMinDistance, arcadeController.AudioMaxDistance, arcadeController.VolumeCurve);
        }

        protected abstract string[] GetModelImageDirectories(ModelConfiguration modelConfiguration);

        protected abstract string[] GetModelVideoDirectories(ModelConfiguration modelConfiguration);

        protected abstract string[] GetPlatformImageDirectories(PlatformConfiguration platform);

        protected abstract string[] GetPlatformVideoDirectories(PlatformConfiguration platform);

        protected static string[] GetDirectories(string[] array)
        {
            if (array == null)
                return null;

            List<string> result = new List<string>();
            AddPossibleDirectories(result, array);
            return result.Count > 0 ? result.ToArray() : null;
        }

        protected static string[] GetDirectories(string[] array1, string[] array2)
        {
            if (array1 == null)
                return null;

            List<string> result = new List<string>();
            AddPossibleDirectories(result, array1);

            if (array2 == null)
                return result.Count > 0 ? result.ToArray() : null;

            AddPossibleDirectories(result, array2);
            return result.Count > 0 ? result.ToArray() : null;
        }

        protected static void AddPossibleDirectories(List<string> list, string[] directories)
        {
            if (directories != null && directories.Length > 0)
                list.AddRange(FileSystem.CorrectPaths(directories));
        }

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
