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

using Cinemachine;
using UnityEngine;

namespace Arcade
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class PlayerController : MonoBehaviour
    {
        [SerializeField] protected Camera _camera;
        [SerializeField] protected CinemachineVirtualCamera _virtualCamera;
        [SerializeField] protected float _walkSpeed = 3f;
        [SerializeField] protected float _minVerticalLookAngle = -89f;
        [SerializeField] protected float _maxVerticalLookAngle = 89f;

        public Camera Camera => _camera;
        public CinemachineVirtualCamera VirtualCamera => _virtualCamera;

        protected CharacterController _characterController;
        protected InputActions _inputActions;
        protected Vector2 _lookInputValue;
        protected Vector3 _moveVelocity;
        protected float _lookHorizontal;
        protected float _lookVertical;

        private void Awake() => _characterController = GetComponent<CharacterController>();

        private void Start() => _inputActions = FindObjectOfType<Main>().InputActions;

        public void SetVerticalLookLimits(float min, float max)
        {
            _minVerticalLookAngle = Mathf.Clamp(min, -89f, 0f);
            _maxVerticalLookAngle = Mathf.Clamp(max, 0f, 89f);
        }

        private void OnEnable()
        {
            _lookInputValue = Vector2.zero;
            _moveVelocity   = Vector3.zero;
            _lookHorizontal = 0f;
            _lookVertical   = 0f;
        }

        private void Update()
        {
            GatherMovementInputValues();
            HandleMovement(Time.deltaTime);

            GatherLookInputValues();
            HandleLook();
        }

        protected virtual void GatherMovementInputValues()
        {
        }

        protected virtual void GatherLookInputValues()
        {
        }

        protected virtual void HandleMovement(float dt)
        {
        }

        protected virtual void HandleLook()
        {
        }
    }
}
