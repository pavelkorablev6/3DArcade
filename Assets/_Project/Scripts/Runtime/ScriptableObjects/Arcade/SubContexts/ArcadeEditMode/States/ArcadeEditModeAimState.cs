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
using UnityEngine.EventSystems;

namespace Arcade
{
    public sealed class ArcadeEditModeAimState : ArcadeEditModeState
    {
        [SerializeField] private float _movementSpeedMultiplier = 0.8f;
        [SerializeField] private float _rotationSpeedMultiplier = 0.8f;

        [System.NonSerialized] private Vector2 _aimPosition;
        [System.NonSerialized] private float _aimRotation;

        public override void OnEnter()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");
            //Context.ArcadeEditModeStateChangeEvent.Raise(this);

            Context.ArcadeContext.InteractionControllers.EditModeController.Reset();
        }

        public override void OnExit() => Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

        public override void OnUpdate(float dt)
        {
            ArcadeContext arcadeContext = Context.ArcadeContext;

            //if (arcadeContext.MouseOverUI.Value)
            //    return;

            bool pointerOverUI = EventSystem.current.IsPointerOverGameObject();
            Debug.Log(pointerOverUI);
            if (pointerOverUI)
                return;

            InteractionControllers interactionControllers    = arcadeContext.InteractionControllers;
            EditModeInteractionController editModeController = interactionControllers.EditModeController;

            editModeController.UpdateCurrentTarget(arcadeContext.Player.Camera);

            ModelConfigurationComponent currentTarget = editModeController.InteractionData.Current;
            if (currentTarget == null || !currentTarget.Configuration.MoveCabMovable)
                return;

            InputActions inputActions = arcadeContext.InputActions;

            Vector2 positionInput = inputActions.FpsEditActions.Move.ReadValue<Vector2>();
            float rotationInput   = inputActions.FpsEditActions.Rotate.ReadValue<float>();
            _aimPosition          = positionInput * _movementSpeedMultiplier;
            _aimRotation          = rotationInput * _rotationSpeedMultiplier;

            if (currentTarget.Configuration.MoveCabGrabbable && inputActions.FpsEditActions.Grab.triggered)
                Context.TransitionTo<ArcadeEditModeAutoMoveState>();
        }

        public override void OnFixedUpdate(float dt)
            => Context.ArcadeContext.InteractionControllers.EditModeController.ManualMoveAndRotate(_aimPosition, _aimRotation);
    }
}
