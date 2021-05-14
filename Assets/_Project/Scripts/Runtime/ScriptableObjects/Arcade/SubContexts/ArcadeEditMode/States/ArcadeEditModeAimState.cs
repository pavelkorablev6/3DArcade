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
        [SerializeField] private float _movementSpeedMultiplier = 0.8f;
        [SerializeField] private float _rotationSpeedMultiplier = 0.8f;

        public override void OnEnter()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            Context.ArcadeContext.InteractionControllers.Reset();
        }

        public override void OnExit() => Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

        public override void OnUpdate(float dt)
        {
            ArcadeContext arcadeContext = Context.ArcadeContext;

            if (arcadeContext.MouseOverUI)
                return;

            InteractionControllers interactionControllers    = arcadeContext.InteractionControllers;
            EditModeInteractionController editModeController = interactionControllers.EditModeController;
            Player player                                    = arcadeContext.Player;

            editModeController.UpdateCurrentTarget(false);
            if (editModeController.InteractionData.Current == null)
                return;

            float dot = Vector3.Dot(editModeController.InteractionData.Current.transform.forward, -player.ActiveTransform.forward);
            if (dot < 0.6f)
            {
                editModeController.UpdateCurrentTarget(true);
                if (editModeController.InteractionData.Current == null)
                    return;
            }

            if (editModeController.InteractionData.Current == null || !editModeController.InteractionData.Current.Configuration.MoveCabMovable)
                return;

            InputActions inputActions = arcadeContext.InputActions;

            Vector2 positionInput = inputActions.FpsMoveCab.Move.ReadValue<Vector2>();
            float rotationInput   = inputActions.FpsMoveCab.Rotate.ReadValue<float>();
            Vector2 aimPosition   = positionInput * _movementSpeedMultiplier;
            float aimRotation     = rotationInput * _rotationSpeedMultiplier;

            interactionControllers.EditModeController.InteractionData.SetAimData(player.Camera, aimPosition, aimRotation);

            if (editModeController.InteractionData.Current.Configuration.MoveCabGrabbable && inputActions.FpsMoveCab.Grab.triggered)
                Context.TransitionTo<ArcadeEditModeAutoMoveState>();
        }

        public override void OnFixedUpdate(float dt)
        {
            EditModeInteractionController editModeController = Context.ArcadeContext.InteractionControllers.EditModeController;
            ModelConfigurationComponent currentTarget        = editModeController.InteractionData.Current;

            if (currentTarget == null || !currentTarget.Configuration.MoveCabMovable)
                return;

            editModeController.ManualMoveAndRotate();
        }
    }
}
