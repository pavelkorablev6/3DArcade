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
using UnityEditor;
using UnityEngine;

namespace Arcade.UnityEditor
{
    internal static class FindAndRemoveMissingScripts
    {
        [MenuItem("Auto/Remove Missing Scripts Recursively Visit Prefabs")]
        public static void FindAndRemoveMissingInSelected()
        {
            IEnumerable<GameObject> deeperSelection = Selection.gameObjects.SelectMany(go => go.GetComponentsInChildren<Transform>(true))
                                                                           .Select(t => t.gameObject);
            HashSet<Object> prefabs = new HashSet<Object>();
            int compCount = 0;
            int goCount = 0;
            foreach (GameObject go in deeperSelection)
            {
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (count > 0)
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(go))
                    {
                        RecursivePrefabSource(go, prefabs, ref compCount, ref goCount);
                        count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                        if (count == 0)
                            continue;
                    }

                    _ = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    compCount += count;
                    goCount++;
                }
            }

            Debug.Log($"Found and removed {compCount} missing scripts from {goCount} GameObjects");
        }

        private static void RecursivePrefabSource(GameObject instance, HashSet<Object> prefabs, ref int compCount, ref int goCount)
        {
            GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(instance);
            if (source == null || !prefabs.Add(source))
                return;

            RecursivePrefabSource(source, prefabs, ref compCount, ref goCount);

            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(source);
            if (count > 0)
            {
                _ = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(source);
                compCount += count;
                goCount++;
            }
        }
    }
}
