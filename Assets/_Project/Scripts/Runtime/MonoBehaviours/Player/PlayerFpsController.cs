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
    public sealed class PlayerFpsController : PlayerController
    {
        [SerializeField] private float _runSpeed  = 6f;
        [SerializeField] private float _jumpForce = 10f;

        [SerializeField] private float _extraGravity = 40f;

        private InputAction _sprintAction;
        private InputAction _jumpAction;

        private Vector2 _movementInputValue;
        private bool _sprinting;
        private bool _performJump;

        private void Awake()
        {
            Construct();

            InputActionMap actionMap = _inputActionAsset.FindActionMap("FpsArcade");
            _movementAction          = actionMap.FindAction("Movement");
            _lookAction              = actionMap.FindAction("Look");
            _sprintAction            = actionMap.FindAction("Sprint");
            _jumpAction              = actionMap.FindAction("Jump");

            // TEMP
            actionMap.Enable();
        }

        private void Update()
        {
            if (_movementAction.enabled)
                GatherMovementInputValues();
            HandleMovement(Time.deltaTime);

            if (_lookAction.enabled)
            {
                GatherLookInputValues();
                HandleLook();
            }
        }

        protected override void GatherMovementInputValues()
        {
            _movementInputValue = _movementAction.ReadValue<Vector2>();
            _sprinting          = _sprintAction.ReadValue<float>() > 0f;
            _performJump        = _jumpAction.triggered;
        }

        protected override void GatherLookInputValues() => _lookInputValue = _lookAction.ReadValue<Vector2>();

        protected override void HandleMovement(float dt)
        {
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
            _lookHorizontal = _lookInputValue.x;
            _lookVertical  += _lookInputValue.y;
            _lookVertical   = Mathf.Clamp(_lookVertical, _minVerticalLookAngle, _maxVerticalLookAngle);
            if (_virtualCamera != null)
                _virtualCamera.transform.localEulerAngles = new Vector3(-_lookVertical, 0f, 0f);
            transform.Rotate(new Vector3(0f, _lookHorizontal, 0f));
        }
    }
}
