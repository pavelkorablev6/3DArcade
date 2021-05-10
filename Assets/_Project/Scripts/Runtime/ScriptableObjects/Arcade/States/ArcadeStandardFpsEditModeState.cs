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
    public sealed class ArcadeStandardFpsEditModeState : ArcadeState
    {
        [SerializeField] private ArcadeEditModeContext _editModeContext;

        public override void OnEnter()
        {
            Debug.Log($"> <color=green>Entered</color> {GetType().Name}");

            _editModeContext.Start();

            Context.VideoPlayerController.StopAllVideos();

            Context.ArcadeController.StoreModelPositions();

            Context.InputActions.FpsArcade.Enable();
            Context.InputActions.FpsArcade.Interact.Disable();
            if (Cursor.lockState != CursorLockMode.Locked)
                Context.InputActions.FpsArcade.Look.Disable();

            Context.InputActions.FpsMoveCab.Enable();

            Context.InteractionControllers.NormalModeRaycaster.ResetCurrentTarget();

            _editModeContext.TransitionTo<ArcadeEditModeAimState>();

            Context.UIStateTransitionEvent.Raise(typeof(UIStandardEditModeState));
        }

        public override void OnExit()
        {
            Debug.Log($"> <color=orange>Exited</color> {GetType().Name}");

            Context.InputActions.FpsArcade.Disable();
            Context.InputActions.FpsMoveCab.Disable();

            _editModeContext.TransitionTo<ArcadeEditModeNullState>();

            Context.UIStateTransitionEvent.Raise(typeof(UIDisabledState));
        }

        public override void OnUpdate(float dt)
        {
            if (Context.MouseOverUI)
            {
                Context.InputActions.FpsArcade.Disable();
                Context.InputActions.FpsMoveCab.Grab.Disable();
            }
            else
            {
                Context.InputActions.FpsArcade.Enable();
                Context.InputActions.FpsArcade.Interact.Disable();
                if (Cursor.lockState != CursorLockMode.Locked)
                    Context.InputActions.FpsArcade.Look.Disable();
                Context.InputActions.FpsMoveCab.Grab.Enable();
            }

            if (Context.InputActions.Global.Quit.triggered)
            {
                RevertChanges();
                Context.TransitionToPrevious();
                return;
            }

            if (Context.InputActions.FpsArcade.ToggleMoveCab.triggered)
            {
                SaveChanges();
                Context.TransitionToPrevious();
                return;
            }

            if (Context.InputActions.Global.ToggleCursor.triggered)
                HandleCursorToggle();

            _editModeContext.OnUpdate(dt);
        }

        public override void OnFixedUpdate(float dt) => _editModeContext.OnFixedUpdate(dt);

        private void RevertChanges()
        {
            Context.ArcadeController.RestoreModelPositions();
            _editModeContext.TransitionTo<ArcadeEditModeNullState>();
        }

        private void SaveChanges()
        {
            _ = Context.SaveCurrentArcade(true);
            _editModeContext.TransitionTo<ArcadeEditModeNullState>();
        }

        private void HandleCursorToggle()
        {
            CursorUtils.ToggleMouseCursor();
            if (Cursor.lockState == CursorLockMode.Locked)
                Context.InputActions.FpsArcade.Look.Enable();
            else
                Context.InputActions.FpsArcade.Look.Disable();
        }
    }
}
