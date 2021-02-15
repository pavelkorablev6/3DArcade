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

using UnityEngine;

namespace Arcade
{
    [ExecuteAlways]
    public abstract class DatabaseEditorBase<T> : MonoBehaviour
        where T : XMLDatabaseEntry
    {
        public T[] Entries;

        protected static IVirtualFileSystem _vfs;
        protected XMLDatabaseMultiFile<T> _database;

        protected abstract string VFSAlias { get; }
        protected abstract string VFSPath { get; }
        protected abstract XMLDatabaseMultiFile<T> DerivedDatabase { get; }

        private void OnEnable() => Load();

        public void Save()
        {
            if (_database == null)
                return;

            _database.DeleteAll();

            foreach (T entry in Entries)
                if (!string.IsNullOrEmpty(entry.Id))
                    _ = _database.Add(entry);
        }

        public void Load()
        {
            if (_vfs == null)
                _vfs = new VirtualFileSystem().MountDirectory(VFSAlias, VFSPath);
            else
                _ = _vfs.MountDirectory(VFSAlias, VFSPath);

            if (_database == null)
                _database = DerivedDatabase;
            else
                _ = _database.LoadAll();

            Entries = _database.GetValues();
        }
    }
}
