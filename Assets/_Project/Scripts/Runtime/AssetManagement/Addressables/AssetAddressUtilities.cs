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

namespace Arcade
{
    public static class AssetAddressUtilities
    {
        public const string EDITOR_ADDRESSABLES_PATH = "Assets/_Project/Addressables/";
        public const string SCENE_FILE_EXTENSION     = "unity";
        public const string PREFAB_FILE_EXTENSION    = "prefab";

        public static void TryAdd(this AssetAddresses addresses, string name, string extension, string runtimePrefix, string editorPrefix = null)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (!Application.isPlaying)
            {
                editorPrefix ??= runtimePrefix;
                addresses.Addresses.Add($"{EDITOR_ADDRESSABLES_PATH}{editorPrefix}{name}.{extension}");
                return;
            }

            addresses.Addresses.Add($"{runtimePrefix}{name}");
        }
    }
}
