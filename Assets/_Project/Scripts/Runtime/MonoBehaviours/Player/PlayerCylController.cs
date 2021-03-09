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
using UnityEngine.InputSystem;

namespace Arcade
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerCylController : PlayerController
    {
        [SerializeField] private float _minHorizontalLookAngle = -40f;
        [SerializeField] private float _maxHorizontalLookAngle = 40f;

        public override bool MovementEnabled
        {
            get => _movementAction != null && _movementAction.enabled;
            set
            {
                if (value)
                    _movementAction.Enable();
                else
                    _movementAction.Disable();
            }
        }

        public override bool LookEnabled
        {
            get => _inputActions.CylArcade.Look.enabled;
            set
            {
                if (value)
                    _inputActions.CylArcade.Look.Enable();
                else
                    _inputActions.CylArcade.Look.Disable();
            }
        }

        private InputAction _movementAction;
        private float _movementInputValue;

        public void SetupForHorizontalWheel() => _movementAction = _inputActions.CylArcade.NavigationUpDown;

        public void SetupForVerticalWheel() => _movementAction = _inputActions.CylArcade.NavigationLeftRight;

        public void SetHorizontalLookLimits(float min, float max)
        {
            _minHorizontalLookAngle = Mathf.Clamp(min, -90f, 0f);
            _maxHorizontalLookAngle = Mathf.Clamp(max, 0f, 90f);
        }

        protected override void HandleMovement(float dt)
        {
            _movementInputValue = _movementAction != null && _movementAction.enabled ? _movementAction.ReadValue<float>() : 0f;

            _ = _characterController.Move(new Vector3(0f, 0f, _movementInputValue) * _walkSpeed * dt);

            if (transform.localPosition.z < 0f)
                transform.localPosition = new Vector3(0f, transform.localPosition.y, 0f);
        }

        protected override void HandleLook()
        {
            _lookInputValue = _inputActions.CylArcade.Look.enabled ? _inputActions.CylArcade.Look.ReadValue<Vector2>(): Vector2.zero;

            _lookHorizontal += _lookInputValue.x;
            _lookVertical   += _lookInputValue.y;
            _lookHorizontal  = Mathf.Clamp(_lookHorizontal, _minHorizontalLookAngle, _maxHorizontalLookAngle);
            _lookVertical    = Mathf.Clamp(_lookVertical, _minVerticalLookAngle, _maxVerticalLookAngle);
            if (_virtualCamera != null)
                _virtualCamera.transform.localEulerAngles = new Vector3(-_lookVertical, 0f, 0f);
            transform.localEulerAngles = new Vector3(0f, _lookHorizontal, 0f);
        }
    }
}
