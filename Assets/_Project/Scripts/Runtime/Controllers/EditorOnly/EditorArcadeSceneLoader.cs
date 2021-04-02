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

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Arcade
{
    public sealed class EditorArcadeSceneLoader : IArcadeSceneLoader
    {
        public bool IsSceneLoading => false;
        public float LoadPercentCompleted => 100f;

        public void Load(IEnumerable<string> namesToTry, System.Action onComplete)
        {
            foreach (string nameToTry in namesToTry)
            {
                System.Type assetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(nameToTry);
                if (assetType == typeof(UnityEditor.SceneAsset))
                {
                    Scene scene = EditorSceneManager.OpenScene(nameToTry, OpenSceneMode.Additive);
                    onComplete?.Invoke();
                    _ = SceneManager.SetActiveScene(scene);
                    UnityEditor.SceneVisibilityManager.instance.DisablePicking(scene);
                    return;
                }
            }
        }
    }
}
#endif
