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
using System.Linq;

namespace Arcade
{
    public sealed class Directories : IEnumerable<string>
    {
        private readonly List<string> _paths = new List<string>();

        public Directories(params string[] paths)
        {
            TryAdd(paths);
            Resolve();
        }

        public Directories(params string[][] pathsArray)
        {
            TryAdd(pathsArray);
            Resolve();
        }

        private void TryAdd(params string[] paths)
        {
            string[] correctedPaths = FileSystem.CorrectPaths(paths);
            if (correctedPaths != null)
                _paths.AddRange(correctedPaths.Where(x => !string.IsNullOrEmpty(x)));
        }

        private void TryAdd(params string[][] pathsArray)
        {
            foreach (string[] pathArray in pathsArray)
                TryAdd(pathArray);
        }

        private void Resolve()
        {
            for (int i = _paths.Count - 1; i >= 0; --i)
                if (!Directory.Exists(_paths[i]))
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
