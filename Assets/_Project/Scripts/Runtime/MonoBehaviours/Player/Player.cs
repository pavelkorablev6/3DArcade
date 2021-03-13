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

using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Zenject;

namespace Arcade
{
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private PlayerControls _normalControls;
        [SerializeField] private PlayerControls _virtualRealityControls;

        public PlayerControls NormalControls => _normalControls;
        public PlayerControls VirtualRealityControls => _virtualRealityControls;

        private PlayerContext _playerContext;

        [Inject, SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "DI")]
        private void Construct(PlayerContext context) => _playerContext = context;

        private void Start() => TransitionTo<PlayerDisabledState>();

        public void TransitionTo<T>() where T : PlayerState => _playerContext.TransitionTo<T>();
    }
}
