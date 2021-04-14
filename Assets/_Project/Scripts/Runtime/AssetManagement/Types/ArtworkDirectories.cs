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
using System.IO;
using System.Linq;

namespace Arcade
{
    public sealed class ArtworkDirectories
    {
        public string[] Directories => _directories.ToArray();

        private readonly List<string> _directories;

        public ArtworkDirectories(params string[] directories) => _directories = new List<string>(directories);

        public static ArtworkDirectories GetCorrectedDirectories(params string[][] arrays)
        {
            ArtworkDirectories artworkDirectories = new ArtworkDirectories();
            foreach (string[] array in arrays)
                artworkDirectories.TryAdd(array);
            return artworkDirectories;
        }

        public void TryResolve(ArtworkDirectories directories)
        {
            if (directories == null)
                return;

            foreach (string directory in directories.Directories)
            {
                string directoryPath = FileSystem.CorrectPath(directory);
                if (Directory.Exists(directoryPath))
                    _directories.Add(directoryPath);
            }
        }

        private void TryAdd(string[] directories)
        {
            string[] correctedPaths = FileSystem.CorrectPaths(directories)?.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (correctedPaths != null)
                _directories.AddRange(correctedPaths);
        }
    }
}
