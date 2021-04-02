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
using UnityEngine;

namespace Arcade
{
    public sealed class ModelInstance
    {
        private readonly ModelConfiguration _modelConfiguration;
        private readonly IModelSpawner _modelSpawner;
        private readonly System.Action<ModelConfigurationComponent> _onModelSpawned;

        public ModelInstance(ModelConfiguration modelConfiguration, System.Action<ModelConfigurationComponent> onModelSpawned)
        {
            _modelConfiguration = modelConfiguration;
            _onModelSpawned     = onModelSpawned;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                _modelSpawner = new EditorModelSpawner();
                return;
            }
#endif
            _modelSpawner = new ModelSpawner();
        }

        public void SpawnModel(IEnumerable<string> namesToTry, Transform parent, bool atPositionWithRotation)
        {
            Vector3 position;
            Quaternion orientation;

            if (atPositionWithRotation)
            {
                position    = _modelConfiguration.Position;
                orientation = Quaternion.Euler(_modelConfiguration.Rotation);
            }
            else
            {
                position    = Vector3.zero;
                orientation = Quaternion.identity;
            }

            _modelSpawner.Spawn(namesToTry, position, orientation, parent, SetupModel);
        }

        private void SetupModel(GameObject gameObject)
        {
            gameObject.name                 = _modelConfiguration.Id;
            gameObject.transform.localScale = _modelConfiguration.Scale;

            ModelConfigurationComponent modelConfigurationComponent = gameObject.AddComponent<ModelConfigurationComponent>();
            modelConfigurationComponent.SetModelConfiguration(_modelConfiguration);

            _onModelSpawned?.Invoke(modelConfigurationComponent);
        }
    }
}
