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
using System.Linq;

namespace Arcade
{
    public static class ArtworkUtils
    {
        private const int NUM_MAX_VARIANTS = 20;

        public static string[] GetDirectories(string[] array)
        {
            ArtworkDirectories artworkDirectories = new ArtworkDirectories { Directories = new List<string>() };
            artworkDirectories.TryAdd(array);
            return artworkDirectories.Directories.ToArray();
        }

        public static string[] GetDirectories(string[] array1, string[] array2)
        {
            ArtworkDirectories artworkDirectories = new ArtworkDirectories { Directories = new List<string>() };
            artworkDirectories.TryAdd(array1);
            artworkDirectories.TryAdd(array2);
            return artworkDirectories.Directories.ToArray();
        }

        public static void TryAdd(this ImageSequence imageSequence, string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            imageSequence.Images.Add(name);
            for (int i = 0; i < NUM_MAX_VARIANTS; ++i)
                imageSequence.Images.Add($"{name}_{i}");
        }

        private static void TryAdd(this ArtworkDirectories artworkDirectories, string[] directories)
        {
            if (directories == null || directories.Length == 0)
                return;

            artworkDirectories.Directories.AddRange(FileSystem.CorrectPaths(directories).Where(x => !string.IsNullOrEmpty(x)));
        }
    }
}
