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
    public sealed class ArcadeEditModeAimState : ArcadeEditModeState
    {
        private static readonly float _movementSpeedMultiplier = 0.8f;
        private static readonly float _rotationSpeedMultiplier = 0.8f;

        public ArcadeEditModeAimState(ArcadeEditModeContext context)
        : base(context)
        {
        }

        public override void OnEnter()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            _context.InteractionRaycaster.ResetCurrentTarget();
        }

        public override void OnExit() => Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

        public override void OnUpdate(float dt)
        {
            _context.InteractionRaycaster.UpdateCurrentTarget();

            EditModeInteractionData interactionData = _context.InteractionController.InteractionData;
            if (interactionData.CurrentTarget == null || !interactionData.CurrentTarget.Configuration.MoveCabMovable)
                return;

            Vector2 positionInput = _context.InputActions.FpsMoveCab.Move.ReadValue<Vector2>();
            float rotationInput   = _context.InputActions.FpsMoveCab.Rotate.ReadValue<float>();
            Vector2 aimPosition   = positionInput * _movementSpeedMultiplier;
            float aimRotation     = rotationInput * _rotationSpeedMultiplier;
            interactionData.SetAimData(_context.Player.Camera, aimPosition, aimRotation);

            if (!interactionData.CurrentTarget.Configuration.MoveCabGrabbable)
                return;

            if (_context.InputActions.FpsMoveCab.Grab.triggered)
                _context.TransitionTo<ArcadeEditModeGrabState>();
        }

        public override void OnFixedUpdate(float dt)
        {
            if (_context.InteractionController.InteractionData.CurrentTarget == null || !_context.InteractionController.InteractionData.CurrentTarget.Configuration.MoveCabMovable)
                return;

            _context.InteractionController.ManualMoveAndRotate();
        }
    }
}
