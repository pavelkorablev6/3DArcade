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

using System.IO;

namespace Arcade
{
    public static class ArtworkDirectoriesResolver
    {
        public static string[] GetDirectoriesToTry(string[] gameArtworkDirectories, string[] platformArtworkDirectories, string[] defaultArtworkDirectories)
        {
            ArtworkDirectories artworkDirectories = new ArtworkDirectories();
            artworkDirectories.TryResolveDirectories(gameArtworkDirectories);
            artworkDirectories.TryResolveDirectories(platformArtworkDirectories);
            artworkDirectories.TryResolveDirectories(defaultArtworkDirectories);
            return artworkDirectories.Directories.ToArray();
        }

        private static void TryResolveDirectories(this ArtworkDirectories artworkDirectories, string[] directories)
        {
            if (directories == null || directories.Length == 0)
                return;

            foreach (string directory in directories)
            {
                string directoryPath = FileSystem.CorrectPath(directory);
                if (Directory.Exists(directoryPath))
                    artworkDirectories.Directories.Add(directoryPath);
            }
        }
    }
}
