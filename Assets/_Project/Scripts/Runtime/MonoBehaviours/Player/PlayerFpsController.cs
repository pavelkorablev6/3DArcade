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
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerFpsController : PlayerController
    {
        [SerializeField] private float _runSpeed  = 6f;
        [SerializeField] private float _jumpForce = 10f;

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

        private Vector2 _movementInputValue;
        private bool _sprinting;
        private bool _performJump;

        protected override void HandleMovement(float dt)
        {
            if (_inputActions.FpsArcade.Movement.enabled)
            {
                _movementInputValue = _inputActions.FpsArcade.Movement.ReadValue<Vector2>();
                _sprinting          = _inputActions.FpsArcade.Sprint.ReadValue<float>() > 0f;
                _performJump        = _inputActions.FpsArcade.Jump.triggered;
            }
            else
            {
                _movementInputValue = Vector2.zero;
                _sprinting          = false;
                _performJump        = false;
            }

            if (_characterController.isGrounded)
            {
                _moveVelocity = new Vector3(_movementInputValue.x, -0.1f, _movementInputValue.y);
                _moveVelocity.Normalize();

                float speed   = _sprinting ? _runSpeed : _walkSpeed;
                _moveVelocity = transform.TransformDirection(_moveVelocity) * speed;

                if (_performJump)
                    _moveVelocity.y = _jumpForce;
            }

            if ((_characterController.collisionFlags & CollisionFlags.Above) != 0 && _moveVelocity.y > 0f)
                _moveVelocity.y -= _moveVelocity.y;

            _moveVelocity.y -= _extraGravity * dt;
            _ = _characterController.Move(_moveVelocity * dt);
        }

        protected override void HandleLook()
        {
            _lookInputValue = _inputActions.FpsArcade.Look.enabled ? _inputActions.FpsArcade.Look.ReadValue<Vector2>() : Vector2.zero;

            _lookHorizontal = _lookInputValue.x;
            _lookVertical  += _lookInputValue.y;
            _lookVertical   = Mathf.Clamp(_lookVertical, _minVerticalLookAngle, _maxVerticalLookAngle);
            if (_virtualCamera != null)
                _virtualCamera.transform.localEulerAngles = new Vector3(-_lookVertical, 0f, 0f);
            transform.Rotate(new Vector3(0f, _lookHorizontal, 0f));
        }
    }
}
