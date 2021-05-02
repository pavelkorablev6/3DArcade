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
    public static class FileSystemUtils
    {
        public static string CorrectPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (!path.StartsWith("@"))
                return path;

            return PathCombine(SystemUtils.GetDataPath(), path.TrimStart('@'));
        }

        public static string[] CorrectPaths(string[] paths)
        {
            if (paths == null)
                return null;

            for (int i = 0; i < paths.Length; ++i)
            {
                if (string.IsNullOrEmpty(paths[i]))
                    paths[i] = null;
                else if (paths[i].StartsWith("@"))
                    paths[i] = PathCombine(SystemUtils.GetDataPath(), paths[i].TrimStart('@'));
            }

            return paths;
        }

        public static string PathCombine(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }
            return Path.Combine(path1, path2);
        }

        public static string[] GetFiles(string dirPath, string searchPattern, bool searchAllDirectories)
        {
            if (!Directory.Exists(dirPath))
                return null;

            SearchOption searchOption;
            if (string.IsNullOrEmpty(searchPattern))
                searchOption = SearchOption.TopDirectoryOnly;
            else
                searchOption = searchAllDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            string[] files = Directory.GetFiles(dirPath, searchPattern, searchOption);
            return files.Length > 0 ? files : null;
        }
    }
}
