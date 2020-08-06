﻿/* MIT License

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
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Arcade
{
    public abstract class NodeController
    {
        protected abstract string[] DefaultImageDirectories { get; }
        protected abstract string[] DefaultVideoDirectories { get; }

        protected static readonly string _defaultMediaDirectory = $"{SystemUtils.GetDataPath()}/3darcade~/Configuration/Media";

        protected readonly ArcadeController _arcadeController;
        protected readonly AssetCache<Texture> _textureCache;

        protected Renderer _renderer;

        protected NodeController(ArcadeController arcadeController, AssetCache<Texture> textureCache)
        {
            _arcadeController = arcadeController;
            _textureCache     = textureCache;
        }

        public void Setup(GameObject model, ModelConfiguration modelConfiguration, EmulatorConfiguration emulator, float emissionIntensity)
        {
            Renderer renderer = GetNodeRenderer(model);
            if (renderer == null)
            {
                return;
            }

            List<string> namesToTry  = ArtworkMatcher.GetNamesToTry(modelConfiguration, emulator);
            List<string> directories = ArtworkMatcher.GetDirectoriesToTry(GetModelVideoDirectories(modelConfiguration), GetEmulatorVideoDirectories(emulator), DefaultVideoDirectories);

            // TODO(Tom): Setup image cycling

            if (_arcadeController.SetupVideo(renderer, directories, namesToTry, emissionIntensity))
            {
                renderer.material.ClearBaseColorAndTexture();
                PostSetup(renderer, null, emissionIntensity);
            }
            else
            {
                directories     = ArtworkMatcher.GetDirectoriesToTry(GetModelImageDirectories(modelConfiguration), GetEmulatorImageDirectories(emulator), DefaultImageDirectories);
                Texture texture = _textureCache.Load(directories, namesToTry);
                PostSetup(renderer, texture, emissionIntensity);
            }
        }

        protected abstract void PostSetup(Renderer renderer, Texture texture, float emissionIntensity);

        protected abstract Renderer GetNodeRenderer(GameObject model);

        protected abstract string[] GetModelImageDirectories(ModelConfiguration modelConfiguration);

        protected abstract string[] GetModelVideoDirectories(ModelConfiguration modelConfiguration);

        protected abstract string[] GetEmulatorImageDirectories(EmulatorConfiguration emulator);

        protected abstract string[] GetEmulatorVideoDirectories(EmulatorConfiguration emulator);

        [SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "Analyzer is dumb")]
        protected static Renderer GetNodeRenderer<T>(GameObject model) where T : NodeTag
        {
            if (model == null)
            {
                return null;
            }

            T nodeTag = model.GetComponentInChildren<T>(true);
            if (nodeTag != null)
            {
                Renderer renderer = nodeTag.GetComponent<Renderer>();
                if (renderer != null)
                {
                    return renderer;
                }
            }
            return null;
        }
    }
}
