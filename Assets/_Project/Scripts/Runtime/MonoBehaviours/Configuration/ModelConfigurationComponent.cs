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

using SK.Utilities.Unity;
using UnityEngine;

namespace Arcade
{
    [DisallowMultipleComponent, SelectionBase]
    public sealed class ModelConfigurationComponent : MonoBehaviour
    {
        [SerializeField] private ModelConfiguration _modelConfiguration;

        public ModelConfiguration Configuration => _modelConfiguration;

        private int _originalLayer;

        public void InitialSetup(ModelConfiguration modelConfiguration, int layer)
        {
            _modelConfiguration = modelConfiguration;
            _originalLayer      = layer;

            transform.name       = modelConfiguration.Id;
            transform.localScale = modelConfiguration.Scale;

            transform.SetLayerRecursively(layer);
        }

        public ModelConfiguration GetModelConfigurationWithUpdatedTransforms()
        {
            _modelConfiguration.Position = transform.localPosition;
            _modelConfiguration.Rotation = MathUtils.ClampEulerAngles(transform.localEulerAngles);
            _modelConfiguration.Scale    = transform.localScale;
            return _modelConfiguration;
        }

        public void ResetLayer() => transform.SetLayerRecursively(_originalLayer);
    }
}
