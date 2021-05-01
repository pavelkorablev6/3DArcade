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

using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Arcade
{
    public abstract class AssetCache<T> where T : class
    {
        private readonly Dictionary<string, T> _loadedAssets = new Dictionary<string, T>();

        public bool Get(string filePath, out T asset) => _loadedAssets.TryGetValue(filePath, out asset);

        public async UniTask<T> Load(string filePath)
        {
            if (Get(filePath, out T foundAsset))
                return foundAsset;

            T newAsset = await TryLoadAsset(filePath);
            if (newAsset == null)
                return null;

            _loadedAssets[filePath] = newAsset;
            return newAsset;
        }

        public async UniTask<T[]> LoadMultipleAsync(Files files)
        {
            if (files == null || files.Count == 0)
                return null;

            List<T> result = new List<T>();

            foreach (string filePath in files)
            {
                T asset = await Load(filePath);
                if (asset != null)
                    result.Add(asset);
            }

            return result.ToArray();
        }

        protected abstract UniTask<T> TryLoadAsset(string filePath);
    }
}
