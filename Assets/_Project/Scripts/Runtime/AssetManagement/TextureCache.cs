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

using UnityEngine;

namespace Arcade
{
    public sealed class TextureCache : AssetCache<Texture>
    {
        protected override bool TryLoadAsset(string filePath, out Texture outAsset) => TryLoadFromFile(filePath, true, out outAsset);

        private static bool TryLoadFromFile(string filePath, bool filtering, out Texture outTexture)
        {
            byte[] data = FileSystem.ReadAllBytes(filePath);
            if (data.Length > 0)
            {
                Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
                if (texture.LoadImage(data))
                {
                    texture.filterMode = filtering ? FilterMode.Bilinear : FilterMode.Point;
                    outTexture = texture;
                    return true;
                }
            }

            outTexture = null;
            return false;
        }
    }
}
