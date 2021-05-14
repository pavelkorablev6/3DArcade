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
using UnityEngine.SceneManagement;
using SK.Utilities.Unity;
using Zenject;

namespace Arcade
{
    [CreateAssetMenu(menuName = "Arcade/Scenes/EntitiesScene", fileName = "EntitiesScene")]
    public sealed class EntitiesScene : ScriptableObject
    {
        [SerializeField] private SceneCreatorBase _sceneCreator;

        public const string ARCADE_SETUP_SCENE_NAME = "ArcadeSetup";

        public static int ArcadeLayer = 0;
        public static int GamesLayer  = 0;
        public static int PropsLayer  = 0;

        [field: System.NonSerialized] public Transform GamesNodeTransform { get; private set; }
        [field: System.NonSerialized] public Transform PropsNodeTransform { get; private set; }

        [field: System.NonSerialized] private Scene _scene;
        [field: System.NonSerialized] private Transform _arcadeNodeTransform;

        [Inject]
        public void Construct(SceneCreatorBase sceneCreator) => _sceneCreator = sceneCreator;

        public void Initialize(ArcadeConfiguration arcadeConfiguration)
        {
            if (ArcadeLayer == 0)
            {
                ArcadeLayer = LayerMask.NameToLayer("Arcade");
                GamesLayer  = LayerMask.NameToLayer("Arcade/GameModels");
                PropsLayer  = LayerMask.NameToLayer("Arcade/PropModels");
            }

            if (_scene.IsValid() && _scene.isLoaded)
                ObjectUtils.DestroyObject(_arcadeNodeTransform.gameObject);
            else
                _scene = _sceneCreator.Create(ARCADE_SETUP_SCENE_NAME);

            SetupArcadeNode(arcadeConfiguration);
            SetupGamesNode();
            SetupPropsNode();
        }

        public static bool TryGetArcadeConfiguration(out ArcadeConfigurationComponent outComponent, bool logErrors = true)
        {
            outComponent = null;

            ArcadeConfigurationComponent[] arcadeConfigurationComponents = Object.FindObjectsOfType<ArcadeConfigurationComponent>();
            if (arcadeConfigurationComponents.Length > 1)
            {
                if (logErrors)
                    Debug.LogError("Multiple ArcadeConfigurationComponent nodes found! Only one can be present in the scene.");
                return false;
            }
            else if (arcadeConfigurationComponents.Length < 1)
            {
                if (logErrors)
                    Debug.LogError("No ArcadeConfigurationComponent found! Current arcade setup is not valid.");
                return false;
            }

            outComponent = arcadeConfigurationComponents[0];
            return true;
        }

        public static bool TryGetGamesNode(out Transform outTransform) => TryGetNode<GamesNodeTag>(out outTransform);

        public static bool TryGetPropsNode(out Transform outTransform) => TryGetNode<PropsNodeTag>(out outTransform);

        private static bool TryGetNode<T>(out Transform outTransform) where T : Component
        {
            outTransform = null;

            if (!TryGetArcadeConfiguration(out ArcadeConfigurationComponent parent))
                return false;

            T[] components = parent.GetComponentsInChildren<T>();
            if (components.Length > 1)
            {
                Debug.LogError($"Multiple {typeof(T).Name} components found! Only one can be present in the scene.");
                return false;
            }
            else if (components.Length < 1)
            {
                Debug.LogError($"No {typeof(T).Name} component found! Current arcade setup is not valid.");
                return false;
            }

            outTransform = components[0].transform;
            return true;
        }

        private void SetupArcadeNode(ArcadeConfiguration arcadeConfiguration)
        {
            GameObject arcadeGameObject = new GameObject("Arcade")
            {
                layer = ArcadeLayer
            };

            ArcadeConfigurationComponent arcadeConfigurationComponent = arcadeGameObject.AddComponent<ArcadeConfigurationComponent>();
            arcadeConfigurationComponent.SetArcadeConfiguration(arcadeConfiguration);

            SceneManager.MoveGameObjectToScene(arcadeGameObject, _scene);
            _arcadeNodeTransform = arcadeGameObject.transform;
        }

        private void SetupGamesNode() => GamesNodeTransform = SetupChildNode<GamesNodeTag>("Games");

        private void SetupPropsNode() => PropsNodeTransform = SetupChildNode<PropsNodeTag>("Props");

        private Transform SetupChildNode<T>(string name) where T : Component
        {
            GameObject nodeGameObject = new GameObject(name, typeof(T));
            nodeGameObject.transform.SetParent(_arcadeNodeTransform);
            return nodeGameObject.transform;
        }
    }
}
