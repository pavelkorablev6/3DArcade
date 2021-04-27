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
using UnityEngine.Events;

namespace Arcade
{
    public sealed class InteractableRaycaster : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _interactionLayerMask;
        [SerializeField] private float _interactionMaxDistance = 1.8f;
        [SerializeField] private InteractionData _interactionData;
        [SerializeField] private UnityEvent<string> _onHoverEnter;
        [SerializeField] private UnityEvent<string> _onHoverExit;

        public void FindInteractable()
        {
            Ray ray = _camera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, _interactionMaxDistance, _interactionLayerMask))
            {
                _onHoverExit.Invoke(string.Empty);
                _interactionData.Reset();
                return;
            }

            ModelConfigurationComponent currentTarget = hitInfo.transform.GetComponent<ModelConfigurationComponent>();
            if (currentTarget != _interactionData.CurrentTarget)
            {
                _interactionData.CurrentTarget = currentTarget;
                _onHoverEnter.Invoke(currentTarget.Configuration.Id);
            }
        }
    }
}
