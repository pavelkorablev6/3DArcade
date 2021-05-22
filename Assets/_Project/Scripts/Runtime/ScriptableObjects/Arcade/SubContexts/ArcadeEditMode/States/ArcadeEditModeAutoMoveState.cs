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

using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Arcade
{
    public sealed class ArcadeEditModeAutoMoveState : ArcadeEditModeState
    {
        [SerializeField] private LayerMask _worldRaycastLayerMask;

        [System.NonSerialized] private Vector2 _screenPoint;

        public override void OnEnter()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            EditModeInteractionData interactionData = Context.ArcadeContext.InteractionControllers.EditModeController.InteractionData;
            _screenPoint = Context.ArcadeContext.Player.Camera.WorldToScreenPoint(interactionData.Current.transform.position);
            interactionData.InitAutoMove();
        }

        public override void OnExit()
        {
            Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

            Context.ArcadeContext.InteractionControllers.EditModeController.InteractionData.DeInitAutoMove();

            _screenPoint = Vector2.zero;
        }

        public override void OnUpdate(float dt)
        {
            if (Context.ArcadeContext.InputActions.FpsEditActions.Grab.triggered)
            {
                Context.TransitionToPrevious();
                return;
            }

            EditModeInteractionController editModeController = Context.ArcadeContext.InteractionControllers.EditModeController;
            Player player                                    = Context.ArcadeContext.Player;

            bool useMousePosition = Mouse.current != null && Cursor.lockState != CursorLockMode.Locked;
            Vector2 rayPosition   = useMousePosition ? Mouse.current.position.ReadValue() : _screenPoint;
            Ray ray               = player.Camera.ScreenPointToRay(rayPosition);

            editModeController.AutoMoveAndRotate(ray, player.ActiveTransform.forward, math.INFINITY, _worldRaycastLayerMask);
        }
    }
}
