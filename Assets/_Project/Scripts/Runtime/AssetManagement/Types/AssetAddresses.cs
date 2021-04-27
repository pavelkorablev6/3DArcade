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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arcade
{
    public sealed class AssetAddresses : IEnumerable<string>
    {
        public readonly List<string> Addresses = new List<string>();

        private const string EDITOR_ADDRESSABLES_PATH = "Assets/_Project/Addressables/";

        private readonly string _extension;
        private readonly string _runtimePrefix;
        private readonly string _editorPrefix;

        public AssetAddresses(string extension, string runtimePrefix, string editorPrefix = null)
        {
            _extension     = extension;
            _runtimePrefix = runtimePrefix;
            _editorPrefix  = editorPrefix ?? _runtimePrefix;
        }

        public void TryAdd(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (!Application.isPlaying)
            {
                Addresses.Add($"{EDITOR_ADDRESSABLES_PATH}{_editorPrefix}{name}.{_extension}");
                return;
            }

            Addresses.Add($"{_runtimePrefix}{name}");
            return;
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string address in Addresses)
                yield return address;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
