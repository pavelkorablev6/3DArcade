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
    [CreateAssetMenu(menuName = "Arcade/StateMachine/State/Standard/ExternalGameState", fileName = "StandardExternalGameState")]
    public sealed class ArcadeStandardExternalGameState : ArcadeExternalGameState
    {
        protected override void OnEnterState()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            Context.VideoPlayerController.StopAllVideos();

#if UNITY_EDITOR_WIN
            SaveUnityWindow();
#endif
        }

        protected override void OnExitState()
        {
            Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

#if UNITY_EDITOR_WIN
            RestoreUnityWindow();
#endif
        }

#if UNITY_EDITOR_WIN
        [System.Runtime.InteropServices.DllImport("user32")] static extern uint GetActiveWindow();
        [System.Runtime.InteropServices.DllImport("user32")] static extern bool SetForegroundWindow(System.IntPtr hWnd);
        private System.IntPtr _editorHwnd;
        private void SaveUnityWindow() => _editorHwnd = (System.IntPtr)GetActiveWindow();
        private void RestoreUnityWindow()
        {
            _ = SetForegroundWindow(_editorHwnd);
            CursorUtils.ToggleMouseCursor();
            CursorUtils.ToggleMouseCursor();
        }
#endif
    }
}
