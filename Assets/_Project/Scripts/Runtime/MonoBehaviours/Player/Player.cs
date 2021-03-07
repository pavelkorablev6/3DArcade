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
    public sealed class Player : MonoBehaviour
    {
        public enum State
        {
            Disabled,
            NormalFPS,
            NormalCYL,
            VRFPS,
            VRCYL
        }

        [SerializeField] private PlayerControls _playerControls;
        [SerializeField] private PlayerControls _playerControlsVR;

        //private void Start() => SetState(State.Disabled);

        public void SetState(State state)
        {
            switch (state)
            {
                case State.NormalFPS:
                    EnableNormalController(PlayerControls.State.FPS);
                    break;
                case State.NormalCYL:
                    EnableNormalController(PlayerControls.State.CYL);
                    break;
                case State.VRFPS:
                    EnableVRController(PlayerControls.State.FPS);
                    break;
                case State.VRCYL:
                    EnableVRController(PlayerControls.State.CYL);
                    break;
                case State.Disabled:
                default:
                    Disable();
                    break;
            }
        }

        private void Disable()
        {
            DisableNormalController();
            DisableVRController();
        }

        private void EnableNormalController(PlayerControls.State state)
        {
            DisableVRController();
            _playerControls.gameObject.SetActive(true);
            _playerControls.SetState(state);
        }

        private void EnableVRController(PlayerControls.State state)
        {
            DisableNormalController();
            _playerControlsVR.gameObject.SetActive(true);
            _playerControlsVR.SetState(state);
        }

        private void DisableNormalController()
        {
            _playerControls.SetState(PlayerControls.State.Disabled);
            _playerControls.gameObject.SetActive(false);
        }

        private void DisableVRController()
        {
            _playerControlsVR.SetState(PlayerControls.State.Disabled);
            _playerControlsVR.gameObject.SetActive(false);
        }
    }
}
