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
    public sealed class ArcadeNormalFpsState : ArcadeState
    {
        private const float INTERACTION_MAX_DISTANCE = 2.5f;

        private static readonly LayerMask _interactionLayerMask = LayerMask.GetMask("Arcade/ArcadeModels", "Arcade/GameModels", "Arcade/PropModels");

        public ArcadeNormalFpsState(ArcadeContext context)
        : base(context)
        {
        }

        public override void OnEnter()
        {
            Debug.Log($"> <color=green>Entered</color> {GetType().Name}");

            _context.InputActions.FpsArcade.Enable();
            if (Cursor.lockState != CursorLockMode.Locked)
                _context.InputActions.FpsArcade.Look.Disable();

            _context.UIManager.TransitionTo<UINormalSceneNormalState>();
            _context.VideoPlayerController.SetPlayer(_context.ArcadeType, _context.Player);
        }

        public override void OnExit()
        {
            Debug.Log($"> <color=orange>Exited</color> {GetType().Name}");

            _context.InputActions.FpsArcade.Disable();

            _context.UIManager.TransitionTo<UIDisabledState>();
        }

        public override void OnUpdate(float dt)
        {
            if (_context.InputActions.Global.Quit.triggered)
                ApplicationUtils.ExitApp();

            if (_context.InputActions.Global.ToggleCursor.triggered)
            {
                CursorUtils.ToggleMouseCursor();
                if (Cursor.lockState == CursorLockMode.Locked)
                    _context.InputActions.FpsArcade.Look.Enable();
                else
                    _context.InputActions.FpsArcade.Look.Disable();
            }

            _context.VideoPlayerController.UpdateVideosState();

            _context.InteractionController.FindInteractable(_interactionLayerMask, INTERACTION_MAX_DISTANCE);

            if (Cursor.lockState == CursorLockMode.Locked && _context.InputActions.FpsArcade.Interact.triggered)
                _context.InteractionController.HandleInteraction(_context);

            //if (_context.Main.PlayerFpsControls.FpsArcadeActions.ToggleMoveCab.triggered)
            //    _context.TransitionTo<FpsMoveCabState>();
        }
    }
}
