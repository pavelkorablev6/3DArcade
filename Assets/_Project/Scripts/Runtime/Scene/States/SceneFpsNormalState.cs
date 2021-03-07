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

using System;
using UnityEngine;

namespace Arcade
{
    public sealed class SceneFpsNormalState : SceneState
    {
        //private const float INTERACT_MAX_DISTANCE = 2.5f;

        public SceneFpsNormalState(SceneContext context)
        : base(context)
        {
        }

        public override void OnEnter()
        {
            Debug.Log($"> <color=green>Entered</color> {GetType().Name}");

            //_context.Main.PlayerFpsControls.FpsArcadeActions.Enable();
            //if (Cursor.lockState != CursorLockMode.Locked)
            //    _context.Main.PlayerFpsControls.FpsArcadeActions.Look.Disable();

            //_context.CurrentModelConfiguration = null;

            //_context.UIController.EnableNormalUI();

            //_context.CurrentPlayerControls = _context.Main.PlayerFpsControls;

            //_context.VideoPlayerController.SetPlayer(_context.Main.PlayerFpsControls.transform);
        }

        public override void OnExit()
        {
            Debug.Log($"> <color=orange>Exited</color> {GetType().Name}");

            //_context.Main.PlayerFpsControls.FpsArcadeActions.Disable();

            //_context.UIController.DisableNormalUI();
        }

        //public override void Update(float dt)
        //{
        //    if (_context.Main.PlayerFpsControls.GlobalActions.Quit.triggered)
        //        SystemUtils.ExitApp();

        //    if (_context.Main.PlayerFpsControls.GlobalActions.ToggleCursor.triggered)
        //    {
        //        SystemUtils.ToggleMouseCursor();
        //        if (Cursor.lockState == CursorLockMode.Locked)
        //            _context.Main.PlayerFpsControls.FpsArcadeActions.Look.Enable();
        //        else
        //            _context.Main.PlayerFpsControls.FpsArcadeActions.Look.Disable();
        //    }

        //    InteractionController.FindInteractable(ref _context.CurrentModelConfiguration,
        //                                           _context.Main.PlayerFpsControls.Camera,
        //                                           INTERACT_MAX_DISTANCE,
        //                                           _context.RaycastLayers);

        //    _context.VideoPlayerController.UpdateVideosState();

        //    if (Cursor.lockState == CursorLockMode.Locked && _context.Main.PlayerFpsControls.FpsArcadeActions.Interact.triggered)
        //        HandleInteraction();

        //    if (_context.Main.PlayerFpsControls.FpsArcadeActions.ToggleMoveCab.triggered)
        //        _context.TransitionTo<FpsMoveCabState>();
        //}

        //private void HandleInteraction()
        //{
        //    if (_context.CurrentModelConfiguration == null)
        //        return;

        //    switch (_context.CurrentModelConfiguration.InteractionType)
        //    {
        //        case InteractionType.Undefined:
        //        case InteractionType.Inherited:
        //        {
        //            if (!string.IsNullOrEmpty(_context.CurrentModelConfiguration.Emulator)
        //             && _context.EmulatorDatabase.Get(_context.CurrentModelConfiguration.Emulator, out EmulatorConfiguration emulator))
        //                HandleEmulatorInteraction(emulator.InteractionType);
        //            else if (!string.IsNullOrEmpty(_context.CurrentModelConfiguration.Platform)
        //                  && _context.PlatformDatabase.Get(_context.CurrentModelConfiguration.Platform, out PlatformConfiguration platform)
        //                  && _context.EmulatorDatabase.Get(platform.Emulator, out emulator))
        //                HandleEmulatorInteraction(emulator.InteractionType);
        //        }
        //        break;
        //        case InteractionType.GameInternal:
        //            _context.TransitionTo<SceneInternalGameState>();
        //            break;
        //        case InteractionType.GameExternal:
        //            _context.TransitionTo<ExternalGameState>();
        //            break;
        //        case InteractionType.FpsArcadeConfiguration:
        //            _context.SetAndStartCurrentArcadeConfiguration(_context.CurrentModelConfiguration.Id, ArcadeType.Fps);
        //            break;
        //        case InteractionType.CylArcadeConfiguration:
        //            _context.SetAndStartCurrentArcadeConfiguration(_context.CurrentModelConfiguration.Id, ArcadeType.Cyl);
        //            break;
        //        case InteractionType.FpsMenuConfiguration:
        //        case InteractionType.CylMenuConfiguration:
        //        case InteractionType.URL:
        //        default:
        //            break;
        //    }
        //}

        //private void HandleEmulatorInteraction(InteractionType interactionType)
        //{
        //    switch (interactionType)
        //    {
        //        case InteractionType.GameInternal:
        //            _context.TransitionTo<SceneInternalGameState>();
        //            break;
        //        case InteractionType.GameExternal:
        //            _context.TransitionTo<ExternalGameState>();
        //            break;
        //        case InteractionType.FpsArcadeConfiguration:
        //            _context.SetAndStartCurrentArcadeConfiguration(_context.CurrentModelConfiguration.Id, ArcadeType.Fps);
        //            break;
        //        case InteractionType.CylArcadeConfiguration:
        //            _context.SetAndStartCurrentArcadeConfiguration(_context.CurrentModelConfiguration.Id, ArcadeType.Cyl);
        //            break;
        //        case InteractionType.Undefined:
        //        case InteractionType.Inherited:
        //        case InteractionType.URL:
        //        case InteractionType.FpsMenuConfiguration:
        //        case InteractionType.CylMenuConfiguration:
        //        default:
        //            break;
        //    }
        //}
    }
}
