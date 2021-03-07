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
    public sealed class XRHand : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _speed = 10;

        private float _flexTarget;
        private float _pinchTarget;
        private float _flexCurrent;
        private float _pinchCurrent;

        private int _animatorFlexHash;
        private int _animatorPinchHash;

        private void Awake()
        {
            _animatorFlexHash  = Animator.StringToHash("Flex");
            _animatorPinchHash = Animator.StringToHash("Pinch");
        }

        private void Update() => AnimateHand();

        public void SetFlex(float value) => _flexTarget = value;

        public void SetPinch(float value) => _pinchTarget = value;

        private void AnimateHand()
        {
            if (_flexCurrent != _flexTarget)
            {
                _flexCurrent = Mathf.MoveTowards(_flexCurrent, _flexTarget, Time.deltaTime * _speed);
                _animator.SetFloat(_animatorFlexHash, _flexCurrent);
            }

            if (_pinchCurrent != _pinchTarget)
            {
                _pinchCurrent = Mathf.MoveTowards(_pinchCurrent, _pinchTarget, Time.deltaTime * _speed);
                _animator.SetFloat(_animatorPinchHash, _pinchCurrent);
            }
        }
    }
}
