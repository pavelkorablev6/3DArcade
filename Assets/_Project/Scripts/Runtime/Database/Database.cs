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
using UnityEngine;

namespace Arcade
{
    public abstract class Database<T> where T : DatabaseEntry
    {
        public string Directory { get; private set; }

        protected abstract T DefaultConfiguration { get; }

        protected readonly SortedDictionary<string, T> _entries = new SortedDictionary<string, T>();

        private readonly IVirtualFileSystem _virtualFileSystem;
        private readonly string _directoryAlias;

        public Database(IVirtualFileSystem virtualFileSystem, string directoryAlias)
        {
            _virtualFileSystem = virtualFileSystem;
            _directoryAlias    = directoryAlias;
        }

        public void Initialize()
        {
            Directory = _virtualFileSystem.GetDirectory(_directoryAlias);
            if (Directory == null)
            {
                Debug.LogWarning($"[{GetType().Name}.Initialize] Directory not mapped in VirtualFileSystem, using default values");
                return;
            }

            PostInitialize();
        }

        public bool Contains(string name) => _entries.ContainsKey(name);

        public string[] GetNames() => _entries.Keys.ToArray();

        public T[] GetValues() => _entries.Values.ToArray();

        public T Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[{GetType().Name}.Get] Passed null for configuration ID");
                return null;
            }

            if (!_entries.ContainsKey(id))
            {
                Debug.LogWarning($"[{GetType().Name}.Get] Configuration not found: {id}");
                return DefaultConfiguration;
            }

            return _entries[id];
        }

        public bool Get(string id, out T outResult)
        {
            outResult = Get(id);
            return outResult != null;
        }

        public T Add(T entry)
        {
            if (entry == null)
            {
                Debug.LogWarning($"[{GetType().Name}.Add] Passed null entry");
                return null;
            }

            if (Contains(entry.Id))
            {
                Debug.LogWarning($"[{GetType().Name}.Add] Entry already exists: {entry.Id}");
                return null;
            }

            _entries.Add(entry.Id, entry);

            PostAdd(entry);

            return entry;
        }

        public bool Delete(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning($"[{GetType().Name}.Delete] Passed null or empty entry ID");
                return false;
            }

            if (!Contains(name))
            {
                Debug.LogWarning($"[{GetType().Name}.Delete] Entry not found: {name}");
                return false;
            }

            if (!_entries.Remove(name))
            {
                Debug.LogWarning($"[{GetType().Name}.Delete] Dictionary error");
                return false;
            }

            PostDelete(name);

            return true;
        }

        public void DeleteAll()
        {

            DeleteAllFromDisk();
            _entries.Clear();
        }

        public bool LoadAll()
        {
            if (!System.IO.Directory.Exists(Directory))
            {
                Debug.LogWarning($"[{GetType().Name}.LoadAll] Directory doesn't exists: {Directory}");
                return false;
            }

            _entries.Clear();
            return LoadAllFromDisk();
        }

        public abstract bool SaveAll();

        protected abstract void PostInitialize();

        protected abstract void PostAdd(T entry);

        protected abstract void PostDelete(string name);

        protected abstract bool LoadAllFromDisk();

        protected abstract void DeleteAllFromDisk();
    }
}
