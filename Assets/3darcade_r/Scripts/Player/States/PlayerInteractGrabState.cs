﻿/* MIT License

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

namespace Arcade_r
{
    public class PlayerInteractGrabState : PlayerState
    {
        public PlayerInteractGrabState(PlayerStateContext context)
        : base(context)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("<color=green>Entered</color> PlayerInteractGrabState");

            _context.PlayerControls.FirstPersonActions.Enable();
            if (Cursor.visible)
            {
                _context.PlayerControls.FirstPersonActions.Look.Disable();
            }

            _context.CurrentGrabbable.OnGrab();
        }

        public override void OnExit()
        {
            Debug.Log("<color=orange>Exited</color> PlayerInteractGrabState");

            _context.PlayerControls.FirstPersonActions.Disable();
        }

        public override void Update(float dt)
        {
            if (_context.PlayerControls.GlobalActions.ToggleCursor.triggered)
            {
                Utils.ToggleMouseCursor();
                if (!Cursor.visible)
                {
                    _context.PlayerControls.FirstPersonActions.Look.Enable();
                }
                else
                {
                    _context.PlayerControls.FirstPersonActions.Look.Disable();
                }
            }

            if (_context.PlayerControls.FirstPersonActions.Interact.triggered)
            {
                _context.TransitionTo<PlayerInteractState>();
            }
        }
    }
}
