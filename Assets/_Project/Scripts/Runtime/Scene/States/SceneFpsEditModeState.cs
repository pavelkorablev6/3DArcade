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
    public sealed class SceneFpsEditModeState : SceneState
    {
        private readonly SceneEditModeContext _editModeContext;

        public SceneFpsEditModeState(SceneContext context)
        : base(context) => _editModeContext = new SceneEditModeContext(/*_context.Main.PlayerFpsControls*/);

        public override void OnEnter()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            //_context.VideoPlayerController.StopAllVideos();

            //_context.Main.PlayerFpsControls.FpsArcadeActions.Enable();
            //if (Cursor.lockState != CursorLockMode.Locked)
            //    _context.Main.PlayerFpsControls.FpsArcadeActions.Look.Disable();
            //_context.Main.PlayerFpsControls.FpsArcadeActions.Interact.Disable();

            //_context.Main.PlayerFpsControls.FpsMoveCabActions.Enable();

            _editModeContext.TransitionTo<SceneEditModeAimState>();

            //_context.UIController.EnableMoveCabUI();
        }

        public override void OnExit()
        {
            Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

            //_context.Main.PlayerFpsControls.FpsArcadeActions.Disable();
            //_context.Main.PlayerFpsControls.FpsMoveCabActions.Disable();

            //_context.UIController.DisableMoveCabUI();
        }

        public override void Update(float dt)
        {
            //if (_context.Main.PlayerFpsControls.GlobalActions.Quit.triggered)
            //{
            //    _context.ReloadCurrentArcadeConfigurationModels();

            //    _editModeContext.TransitionTo<SceneEditModeNullState>();
            //    _context.TransitionTo<SceneFpsNormalState>();
            //}

            //if (_context.Main.PlayerFpsControls.FpsArcadeActions.ToggleMoveCab.triggered)
            //{
            //    _ = _context.SaveCurrentArcadeConfigurationModels();

            //    _editModeContext.TransitionTo<SceneEditModeNullState>();
            //    _context.TransitionTo<SceneFpsNormalState>();
            //}

            //if (_context.Main.PlayerFpsControls.GlobalActions.ToggleCursor.triggered)
            //{
            //    SystemUtils.ToggleMouseCursor();
            //    if (Cursor.lockState == CursorLockMode.Locked)
            //        _context.Main.PlayerFpsControls.FpsArcadeActions.Look.Enable();
            //    else
            //        _context.Main.PlayerFpsControls.FpsArcadeActions.Look.Disable();
            //}

            _editModeContext.Update(dt);
        }

        public override void FixedUpdate(float dt) => _editModeContext.FixedUpdate(dt);
    }
}
