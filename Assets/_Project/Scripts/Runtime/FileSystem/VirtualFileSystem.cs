﻿/* MIT License

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

namespace Arcade
{
    public sealed class VirtualFileSystem : IVirtualFileSystem
    {
        private readonly Dictionary<string, string> _mountedDirectories = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _mountedFiles       = new Dictionary<string, string>();

        public IVirtualFileSystem MountDirectory(string alias, string path)
        {
            if (!_mountedDirectories.ContainsKey(alias))
                _mountedDirectories.Add(alias, path);
            return this;
        }

        public IVirtualFileSystem MountFile(string alias, string path)
        {
            if (!_mountedFiles.ContainsKey(alias))
                _mountedFiles.Add(alias, path);
            return this;
        }

        public string GetDirectory(string alias)
        {
            if (_mountedDirectories.TryGetValue(alias, out string result))
                return result;
            return null;
        }

        public string GetFile(string alias)
        {
            if (_mountedFiles.TryGetValue(alias, out string result))
                return result;
            return null;
        }

        public string[] GetFiles(string alias, string searchPattern, bool searchAllDirectories)
        {
            string directory = GetDirectory(alias);
            if (directory != null)
                return FileSystem.GetFiles(directory, searchPattern, searchAllDirectories);
            return new string[0];
        }
    }
}
