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
    public sealed class ArcadeSceneAddressesProvider : IArcadeSceneAddressesProvider
    {
        private const string FILE_EXTENSION      = "unity";
        private const string ADDRESSABLES_PREFIX = "Arcades/";
        private const string DEFAULT_SCENE_NAME  = "_cylinder";

        public IEnumerable<AssetAddress> GetAddressesToTry(ArcadeConfiguration cfg, ArcadeType arcadeType)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Id))
                return null;

            AssetAddresses assetAddresses = new AssetAddresses(FILE_EXTENSION, ADDRESSABLES_PREFIX);

            string cfgScene = GetSceneName(cfg, arcadeType);

            string editorPrefix = $"{ADDRESSABLES_PREFIX}{cfgScene}/";

            assetAddresses.Add(cfgScene, editorPrefix);
            assetAddresses.Add(cfg.Id, editorPrefix);
            assetAddresses.Add(DEFAULT_SCENE_NAME, editorPrefix);

            return assetAddresses.Addresses;
        }

        private static string GetSceneName(ArcadeConfiguration cfg, ArcadeType arcadeType) => arcadeType switch
        {
            ArcadeType.Fps => cfg.FpsArcadeProperties.Scene,
            ArcadeType.Cyl => cfg.CylArcadeProperties.Scene,
            _              => throw new System.NotImplementedException($"Unhandled switch case for ArcadeType: {arcadeType}")
        };
    }
}
