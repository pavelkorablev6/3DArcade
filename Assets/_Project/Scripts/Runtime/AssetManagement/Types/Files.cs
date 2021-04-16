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

        private readonly List<string> _paths = new List<string>();

        public Files(string[] extensions, Directories directories, params string[] fileNames) => TryAdd(extensions, directories, fileNames);

        public string[] ToArray() => _paths.ToArray();

        public string this[int i]
        {
            get => _paths[i];
            set => _paths[i] = value;
        }

        private void TryAdd(string[] extensions, Directories directories, params string[] names)
        {
            foreach (string directory in directories)
            {
                foreach (string name in names)
                {
                    if (string.IsNullOrEmpty(name))
                        System.Diagnostics.Debugger.Break();

                    foreach (string extension in extensions)
                    {
                        string path = Path.GetFullPath(Path.Combine(directory, $"{name}.{extension}"));
                        if (File.Exists(path))
                        {
                            _paths.Add(path);
                            break;
                        }
                    }
                }
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string path in _paths)
                yield return path;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
