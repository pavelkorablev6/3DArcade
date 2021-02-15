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
using System.Linq;
using UnityEngine;

namespace Arcade
{
    public abstract class XMLDatabaseSingleFile<T, U> : XMLDatabase<T> where T : XMLDatabaseEntry where U : XMLDatabaseEntries<T>, new()
    {
        protected readonly string _filePath;

        protected XMLDatabaseSingleFile(IVirtualFileSystem virtualFileSystem, string directoryAlias, string fileName)
        : base(virtualFileSystem, directoryAlias)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new System.ArgumentNullException(nameof(fileName));

            _filePath = Path.GetFullPath(Path.Combine(_directory, $"{fileName}.xml"));
        }

        protected sealed override void PostAdd(T entry) => SaveAll();

        protected sealed override bool LoadAllFromDisk()
        {
            try
            {
                U entryList = Deserialize();
                if (entryList == null)
                    return false;

                foreach (T entry in entryList.Entries)
                    _ = Add(entry);

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
                U entryList = new U { Entries = _entries.Values.ToArray() };
                Serialize(entryList);

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        protected sealed override void DeleteAllFromDisk()
        {
            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }

        private void Serialize(U entry) => XMLUtils.Serialize(_filePath, entry);

        private U Deserialize() => XMLUtils.Deserialize<U>(_filePath);
    }
}
