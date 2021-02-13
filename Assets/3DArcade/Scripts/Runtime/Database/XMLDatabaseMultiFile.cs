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
using UnityEngine;

namespace Arcade
{
    public abstract class XMLDatabaseMultiFile<T> : XMLDatabase<T> where T : XMLDatabaseEntry
    {
        protected XMLDatabaseMultiFile(IVirtualFileSystem virtualFileSystem, string directoryAlias)
        : base(virtualFileSystem, directoryAlias)
        {
        }

        public bool Save(T item)
        {
            if (item == null || string.IsNullOrEmpty(item.Id))
            {
                Debug.LogWarning($"[{GetType().Name}] Failed to save configuration, data is null or invalid");
                return false;
            }

            string filePath = Path.Combine(_directory, $"{item.Id}.xml");
            return Serialize(filePath, item);
        }

        protected sealed override void PostAdd(T item)
        {
            if (item == null || string.IsNullOrEmpty(item.Id))
            {
                Debug.LogWarning($"[{GetType().Name}] Entry is null or invalid");
                return;
            }

            try
            {
                string filePath = Path.Combine(_directory, $"{item.Id}.xml");
                _ = Serialize(filePath, item);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected sealed override void PostDelete(string name)
        {
            string filePath = Path.Combine(_directory, $"{name}.xml");
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        protected sealed override bool LoadAllFromDisk()
        {
            try
            {
                string[] filePaths = Directory.GetFiles(_directory, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (string filePath in filePaths)
                {
                    T entry = Deserialize(filePath);
                    if (entry != null)
                        _entries.Add(entry.Id, entry);
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        protected sealed override bool SaveAllToDisk()
        {
            try
            {
                foreach (KeyValuePair<string, T> keyValuePair in _entries)
                    _ = Save(keyValuePair.Value);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        private static bool Serialize(string filePath, T entry) => XMLUtils.Serialize(filePath, entry);

        private static T Deserialize(string filePath) => XMLUtils.Deserialize<T>(filePath);
    }
}
