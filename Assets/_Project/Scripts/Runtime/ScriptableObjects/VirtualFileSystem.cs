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
using UnityEngine;

namespace Arcade
{
    [CreateAssetMenu(menuName = "Arcade/VirtualFileSystem", fileName = "VirtualFileSystem")]
    public sealed class VirtualFileSystem : ScriptableObject
    {
        private Dictionary<string, string> MountedDirectories { get; } = new Dictionary<string, string>();
        private Dictionary<string, string> MountedFiles       { get; } = new Dictionary<string, string>();

        public VirtualFileSystem MountDirectory(string alias, string path)
        {
            if (!MountedDirectories.ContainsKey(alias))
                MountedDirectories.Add(alias, path);
            return this;
        }

        public VirtualFileSystem MountFile(string alias, string path)
        {
            if (!MountedFiles.ContainsKey(alias))
                MountedFiles.Add(alias, path);
            return this;
        }

        public string GetDirectory(string alias)
        {
            if (MountedDirectories.TryGetValue(alias, out string result))
                return result;
            return null;
        }

        public string GetFile(string alias)
        {
            if (MountedFiles.TryGetValue(alias, out string result))
                return result;
            return null;
        }

        public string[] GetFiles(string alias, string searchPattern, bool searchAllDirectories)
        {
            string directory = GetDirectory(alias);
            if (directory != null)
                return FileSystemUtils.GetFiles(directory, searchPattern, searchAllDirectories);
            return new string[0];
        }
    }
}
