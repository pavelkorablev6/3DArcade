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

namespace Arcade
{
    public abstract class InteractionRaycaster<T> : ScriptableObject
        where T : InteractionData
    {
        [SerializeField] private LayerMask _raycastMask;
        [SerializeField] private float _raycastMaxDistance = Mathf.Infinity;
        [SerializeField] private T _interactionData;
        [SerializeField] private StringEvent _targetChangedEvent;

        protected Camera _camera;

        public Camera Camera => _camera;
        public float RaycastMaxDistance => _raycastMaxDistance;

        public void Initialize(Camera camera) => _camera = camera;

        public void UpdateCurrentTarget()
        {
            Ray ray = GetRay();
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, _raycastMaxDistance, _raycastMask) || !hitInfo.transform.TryGetComponent(out ModelConfigurationComponent modelConfigurationComponent))
            {
                _targetChangedEvent.Raise(null);
                _interactionData.Reset();
                return;
            }

            if (_interactionData.CurrentTarget == modelConfigurationComponent)
                return;

            _interactionData.Set(hitInfo);

            string description;

            ModelConfiguration modelConfiguration = _interactionData.CurrentTarget.Configuration;

            if (!string.IsNullOrEmpty(modelConfiguration.Overrides.Description))
                description = modelConfiguration.Overrides.Description;
            else if (modelConfiguration.GameConfiguration != null && !string.IsNullOrEmpty(modelConfiguration.GameConfiguration.Description))
                description = modelConfiguration.GameConfiguration.Description;
            else
                description = modelConfiguration.Id;

            _targetChangedEvent.Raise(description);
        }

        public void ResetCurrentTarget() => _interactionData.Reset();

        protected abstract Ray GetRay();
    }
}
