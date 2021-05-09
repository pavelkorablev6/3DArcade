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
    [CreateAssetMenu(menuName = "Arcade/StateMachine/SubContexts/EditMode/States/AimState", fileName = "EditModeAimState")]
    public sealed class ArcadeEditModeAimState : ArcadeEditModeState
    {
        private static readonly float _movementSpeedMultiplier = 0.8f;
        private static readonly float _rotationSpeedMultiplier = 0.8f;

        public override void OnEnter()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            Context.InteractionRaycaster.ResetCurrentTarget();

            Context.UITransitionEvent.Raise(typeof(UIStandardEditModeState));
        }

        public override void OnExit() => Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

        public override void OnUpdate(float dt)
        {
            Context.InteractionRaycaster.UpdateCurrentTarget();

            EditModeInteractionData interactionData = Context.InteractionController.InteractionData;
            if (interactionData.CurrentTarget == null || !interactionData.CurrentTarget.Configuration.MoveCabMovable)
                return;

            Vector2 positionInput = Context.InputActions.FpsMoveCab.Move.ReadValue<Vector2>();
            float rotationInput   = Context.InputActions.FpsMoveCab.Rotate.ReadValue<float>();
            Vector2 aimPosition   = positionInput * _movementSpeedMultiplier;
            float aimRotation     = rotationInput * _rotationSpeedMultiplier;
            interactionData.SetAimData(Context.Player.Camera, aimPosition, aimRotation);

            if (!interactionData.CurrentTarget.Configuration.MoveCabGrabbable)
                return;

            if (Context.InputActions.FpsMoveCab.Grab.triggered)
                Context.TransitionTo<ArcadeEditModeGrabState>();
        }

        public override void OnFixedUpdate(float dt)
        {
            if (Context.InteractionController.InteractionData.CurrentTarget == null || !Context.InteractionController.InteractionData.CurrentTarget.Configuration.MoveCabMovable)
                return;

            Context.InteractionController.ManualMoveAndRotate();
        }
    }
}
