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
using System.IO;

namespace Arcade
{
    public sealed class Files : IEnumerable<string>
    {
        public int Count => _paths.Count;

        private readonly Directories _directories;
        private readonly string[] _extensions;

        private readonly List<string> _paths = new List<string>();

        public Files(string[] extensions, Directories directories, params string[] fileNames)
        : this(extensions, directories)
        {
            TryAdd(fileNames);
            Resolve();
        }

        private Files(string[] extensions, Directories directories)
        {
            _extensions  = extensions;
            _directories = directories;
        }

        public string[] ToArray() => _paths.ToArray();

        private void TryAdd(params string[] names)
        {
            foreach (string directory in _directories)
                foreach (string extension in _extensions)
                    foreach (string name in names)
                        if (!string.IsNullOrEmpty(name))
                            _paths.Add(Path.Combine(directory, $"{name}.{extension}"));
        }

        private void Resolve()
        {
            for (int i = _paths.Count - 1; i >= 0; --i)
                if (!File.Exists(_paths[i]))
                    _paths.RemoveAt(i);
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string path in _paths)
                yield return path;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
