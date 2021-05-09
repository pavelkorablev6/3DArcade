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
    [CreateAssetMenu(menuName = "Arcade/StateMachine/State/Standard/FpsNormalState", fileName = "StandardFpsNormalState")]
    public sealed class ArcadeStandardFpsNormalState : ArcadeState
    {
        public override void OnEnter()
        {
            Debug.Log($"> <color=green>Entered</color> {GetType().Name}");

            Context.InputActions.FpsArcade.Enable();
            if (Cursor.lockState != CursorLockMode.Locked)
                Context.InputActions.FpsArcade.Look.Disable();

            Context.UIStateTransitionEvent.Raise(typeof(UIStandardNormalState));
        }

        public override void OnExit()
        {
            Debug.Log($"> <color=orange>Exited</color> {GetType().Name}");

            Context.InputActions.FpsArcade.Disable();

            Context.UIStateTransitionEvent.Raise(typeof(UIDisabledState));
        }

        public override void OnUpdate(float dt)
        {
            if (Context.InputActions.Global.Quit.triggered)
                ApplicationUtils.ExitApp();

            if (Context.InputActions.Global.ToggleCursor.triggered)
            {
                CursorUtils.ToggleMouseCursor();
                if (Cursor.lockState == CursorLockMode.Locked)
                    Context.InputActions.FpsArcade.Look.Enable();
                else
                    Context.InputActions.FpsArcade.Look.Disable();
            }

            Context.VideoPlayerController.UpdateVideosState();

            Context.InteractionControllers.NormalModeRaycaster.UpdateCurrentTarget();

            if (Cursor.lockState == CursorLockMode.Locked && Context.InputActions.FpsArcade.Interact.triggered)
                Context.InteractionControllers.NormalModeController.HandleInteraction();

            if (Context.InputActions.FpsArcade.ToggleMoveCab.triggered)
                Context.TransitionTo<ArcadeStandardFpsEditModeState>();
        }
    }
}
