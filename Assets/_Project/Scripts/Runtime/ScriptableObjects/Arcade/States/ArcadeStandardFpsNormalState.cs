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
    public sealed class ArcadeStandardFpsNormalState : ArcadeState
    {
        public override void OnEnter()
        {
            Debug.Log($"> <color=green>Entered</color> {GetType().Name}");

            Context.InputActions.FpsActions.Enable();
            if (Cursor.lockState != CursorLockMode.Locked)
                Context.InputActions.FpsActions.Look.Disable();

            Context.InteractionControllers.Reset();

            Context.UIStateTransitionEvent.Raise(typeof(UIStandardNormalState));
        }

        public override void OnExit()
        {
            Debug.Log($"> <color=orange>Exited</color> {GetType().Name}");

            Context.InputActions.FpsActions.Disable();

            Context.UIStateTransitionEvent.Raise(typeof(UIDisabledState));
        }

        public override void OnUpdate(float dt)
        {
            Context.VideoPlayerController.Value.UpdateVideosState();

            if (Context.InputActions.GlobalActions.Quit.triggered)
            {
                ApplicationUtils.ExitApp();
                return;
            }

            if (Context.InputActions.GlobalActions.ToggleCursor.triggered)
                HandleCursorToggle();

            if (Context.InputActions.FpsActions.ToggleEditMode.triggered)
            {
                Context.TransitionTo<ArcadeStandardFpsEditModeState>();
                return;
            }

            Context.InteractionControllers.NormalModeController.UpdateCurrentTarget();

            if (Cursor.lockState == CursorLockMode.Locked && Context.InputActions.FpsActions.Interact.triggered)
                Context.InteractionControllers.NormalModeController.HandleInteraction();
        }

        private void HandleCursorToggle()
        {
            CursorUtils.ToggleMouseCursor();
            if (Cursor.lockState == CursorLockMode.Locked)
                Context.InputActions.FpsActions.Look.Enable();
            else
                Context.InputActions.FpsActions.Look.Disable();
        }
    }
}
