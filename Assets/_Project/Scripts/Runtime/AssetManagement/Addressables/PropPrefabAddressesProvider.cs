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

namespace Arcade
{
    public sealed class PropPrefabAddressesProvider : IPropPrefabAddressesProvider
    {
        public const string PROPS_ADDRESSABLES_PREFIX = "Props/";
        public const string DEFAULT_PROP_PREFAB_NAME  = "_pink_cube";

        public IEnumerable<string> GetNamesToTry(ModelConfiguration cfg)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id))
                return null;

            AssetAddresses assetAddresses = new AssetAddresses { Addresses = new List<string>() };

            TryAdd(cfg.Model);
            TryAdd(cfg.Id);
            TryAdd(DEFAULT_PROP_PREFAB_NAME);

            return assetAddresses.Addresses;

            void TryAdd(string name) => assetAddresses.TryAdd(name, AssetAddressUtilities.PREFAB_FILE_EXTENSION, PROPS_ADDRESSABLES_PREFIX);
        }
    }
}
