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
using UnityEngine.InputSystem;

namespace Arcade
{
    [CreateAssetMenu(menuName = "Arcade/Interaction/Raycaster/EditMode", fileName = "EditModeInteractionRaycaster")]
    public sealed class EditModeInteractionRaycaster : InteractionRaycaster<EditModeInteractionData>
    {
        [SerializeField] private LayerMask _worldRaycastLayerMask;

        public LayerMask WorldRaycastLayerMask => _worldRaycastLayerMask;

        protected override Ray GetRay()
        {
            if (Cursor.lockState == CursorLockMode.Locked || Mouse.current == null)
                return _camera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            return _camera.ScreenPointToRay(mousePosition);
        }
    }
}
