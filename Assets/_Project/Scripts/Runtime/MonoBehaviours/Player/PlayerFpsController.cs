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
    public sealed class PlayerFpsController : PlayerController
    {
        [SerializeField] private float _runSpeed     = 6f;
        [SerializeField] private float _jumpForce    = 10f;
        [SerializeField] private float _extraGravity = 40f;

        public override bool MovementEnabled
        {
            get => _inputActions.FpsArcade.Movement.enabled;
            set
            {
                if (value)
                    _inputActions.FpsArcade.Movement.Enable();
                else
                    _inputActions.FpsArcade.Movement.Disable();
            }
        }

        public override bool LookEnabled
        {
            get => _inputActions.FpsArcade.Look.enabled;
            set
            {
                if (value)
                    _inputActions.FpsArcade.Look.Enable();
                else
                    _inputActions.FpsArcade.Look.Disable();
            }
        }


        protected override void HandleHeight(float dt)
        {
            if (!_inputActions.FpsArcade.CameraHeight.enabled)
                return;

            float heightInput = _inputActions.FpsArcade.CameraHeight.ReadValue<float>();
            if (heightInput == 0f)
                return;

            heightInput *= dt;

            SetHeight(heightInput);
        }

        protected override void HandleMovement(float dt)
        {
            Vector2 movementInputValue;
            bool sprinting;
            bool performJump;

            if (_inputActions.FpsArcade.Movement.enabled)
            {
                movementInputValue = _inputActions.FpsArcade.Movement.ReadValue<Vector2>();
                sprinting          = _inputActions.FpsArcade.Sprint.ReadValue<float>() > 0f;
                performJump        = _inputActions.FpsArcade.Jump.triggered;
            }
            else
            {
                movementInputValue = Vector2.zero;
                sprinting          = false;
                performJump        = false;
            }

            if (_characterController.isGrounded)
            {
                _moveVelocity = new Vector3(movementInputValue.x, -0.1f, movementInputValue.y);
                _moveVelocity.Normalize();

                float speed   = sprinting ? _runSpeed : _walkSpeed;
                _moveVelocity = transform.TransformDirection(_moveVelocity) * speed;

                if (performJump)
                    _moveVelocity.y = _jumpForce;
            }

            if ((_characterController.collisionFlags & CollisionFlags.Above) != 0 && _moveVelocity.y > 0f)
                _moveVelocity.y -= _moveVelocity.y;

            _moveVelocity.y -= _extraGravity * dt;
            _ = _characterController.Move(_moveVelocity * dt);
        }

        protected override void HandleLook()
        {
            Vector2 lookInputValue = _inputActions.FpsArcade.Look.enabled ? _inputActions.FpsArcade.Look.ReadValue<Vector2>() * _turnSensitivity * 0.01f : Vector2.zero;

            float lookHorizontal = lookInputValue.x;
            _lookVertical       += lookInputValue.y;
            _lookVertical        = Mathf.Clamp(_lookVertical, _minVerticalLookAngle, _maxVerticalLookAngle);
            if (_virtualCamera != null)
                _virtualCamera.transform.localEulerAngles = new Vector3(-_lookVertical, 0f, 0f);
            transform.Rotate(new Vector3(0f, lookHorizontal, 0f));
        }
    }
}
