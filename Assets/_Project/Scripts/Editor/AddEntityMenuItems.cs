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

using UnityEditor;
using UnityEngine;

namespace Arcade.UnityEditor
{
    internal sealed class AddEntityMenuItems
    {
        private static GameObject _dummyGamePrefab;
        private static GameObject _dummyPropPrefab;

        [MenuItem("3DArcade/Add New Game #1", false, 41)]
        public static void AddNewGameMenuItem() => SpawnGame();

        [MenuItem("3DArcade/Add New Prop #2", false, 42)]
        public static void AddNewPropMenuItem() => SpawnProp();

        private static void SpawnGame()
        {
            if (ValidatePrefabStatus("pfDummyCabModel", ref _dummyGamePrefab))
                SpawnEntity<GamesNodeTag>("Games", _dummyGamePrefab);
        }

        private static void SpawnProp()
        {
            if (ValidatePrefabStatus("pfDummyPropModel", ref _dummyPropPrefab))
                SpawnEntity<PropsNodeTag>("Props", _dummyPropPrefab);
        }

        private static bool ValidatePrefabStatus(string prefabName, ref GameObject outPrefab)
        {
            if (string.IsNullOrEmpty(prefabName))
                return false;

            if (outPrefab == null)
                outPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_Project/Prefabs/{prefabName}.prefab");
            return outPrefab != null;
        }

        private static void SpawnEntity<T>(string name, GameObject prefab) where T : Component
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (prefab == null)
                return;

            Transform parent = GetNodeTransform<T>(name);
            if (parent == null)
                return;

            GameObject gameObject = Object.Instantiate(prefab, parent);
            gameObject.name = "set_proper_id";

            ModelConfiguration modelConfiguration = new ModelConfiguration
            {
                Id          = "id",
                Description = "Description"
            };
            gameObject.AddComponent<ModelConfigurationComponent>()
                      .SetModelConfiguration(modelConfiguration);

            Selection.activeGameObject = gameObject;
            EditorGUIUtility.PingObject(gameObject);
        }

        private static Transform GetNodeTransform<T>(string name) where T : Component
        {
            ArcadeConfigurationComponent arcadeConfigurationComponent = Object.FindObjectOfType<ArcadeConfigurationComponent>();
            if (arcadeConfigurationComponent == null)
                return null;

            T[] nodes = arcadeConfigurationComponent.GetComponentsInChildren<T>();

            if (nodes.Length > 1)
            {
                Debug.LogError($"Multiple {name} child nodes found! Please remove one of them.");
                return null;
            }

            if (nodes.Length == 1)
                return nodes[0].transform;

            T node = new GameObject(name).AddComponent<T>();
            node.transform.SetParent(arcadeConfigurationComponent.transform);
            return node.transform;
        }
    }
}
