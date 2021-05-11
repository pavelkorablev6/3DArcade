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

namespace Arcade
{
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private PlayerControls _standardControls;
        [SerializeField] private PlayerControls _virtualRealityControls;
        [SerializeField] private PlayerContext _playerContext;

        public Camera Camera => _mainCamera;

        private void Awake() => Initialize();

        private void Start() => TransitionTo<PlayerDisabledState>();

        public Transform ActiveTransform
        {
            get
            {
                if (_standardControls.Active)
                    return _standardControls.ActiveTransform;
                if (_virtualRealityControls.Active)
                    return _virtualRealityControls.ActiveTransform;
                return null;
            }
        }

        public void Initialize() => _playerContext.Initialize(this);

        public void TransitionTo<T>() where T : PlayerState => _playerContext.TransitionTo<T>();

        public void EnableNormalFpsControls() => _standardControls.EnableFpsController();
        public void EnableNormalCylControls() => _standardControls.EnableCylController();
        public void DisableNormalControls()   => _standardControls.Disable();

        public void EnableVirtualRealityFpsControls() => _virtualRealityControls.EnableFpsController();
        public void EnableVirtualRealityCylControls() => _virtualRealityControls.EnableCylController();
        public void DisableVirtualRealityControls()   => _virtualRealityControls.Disable();

        public void Disable()
        {
            DisableNormalControls();
            DisableVirtualRealityControls();
        }
    }
}
