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

using SK.Utilities;
using SK.Utilities.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace Arcade
{
    public sealed class ArcadeVirtualRealityExternalGameState : ArcadeState
    {
        public static Material UDDMaterial;

        private ScreenNodeTag[] _screenNodes;
        private Queue<Material> _savedMaterials;
        private bool _gameRunning;

        public ArcadeVirtualRealityExternalGameState(ArcadeContext context)
        : base(context)
        {
        }

        public override void OnEnter()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            _context.VideoPlayerController.StopCurrentVideo();

            _context.ExternalGameController.OnAppStarted += OnAppStarted;
            _context.ExternalGameController.OnAppExited  += OnAppExited;

            _savedMaterials = new Queue<Material>();

            _screenNodes = _context.InteractionController.CurrentTargetComponent.GetComponentsInChildren<ScreenNodeTag>();
            if (_screenNodes == null)
                return;

            foreach (ScreenNodeTag screenNode in _screenNodes)
            {
                if (screenNode.TryGetComponent(out DynamicArtworkComponent dynamicArtworkComponent))
                    dynamicArtworkComponent.enabled = false;

                Renderer renderer = screenNode.GetComponent<Renderer>();
                _savedMaterials.Enqueue(renderer.material);
                renderer.material = UDDMaterial;

                uDesktopDuplication.Texture uddComponent = screenNode.gameObject.AddComponent<uDesktopDuplication.Texture>();
                uddComponent.invertY = true;
            }

            EmulatorConfiguration emulator = _context.InteractionController.CurrentTarget.EmulatorConfiguration;
            if (!_context.ExternalGameController.StartGame(emulator, _context.InteractionController.CurrentTarget.Id))
            {
                _context.TransitionToPrevious();
                return;
            }

            _gameRunning = true;
        }

        public override void OnUpdate(float dt)
        {
            if (_gameRunning)
                return;

            _context.ExternalGameController.StopCurrent();

            if (_screenNodes == null)
                return;

            foreach (ScreenNodeTag screenNode in _screenNodes)
            {
                if (screenNode.TryGetComponent(out uDesktopDuplication.Texture texture))
                    ObjectUtils.DestroyObject(texture);

                Renderer renderer = screenNode.GetComponent<Renderer>();
                renderer.material = _savedMaterials.Dequeue();

                if (screenNode.TryGetComponent(out DynamicArtworkComponent dynamicArtworkComponent))
                    dynamicArtworkComponent.enabled = true;
            }

            _context.TransitionToPrevious();
        }

        public override void OnExit()
        {
            Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

            _context.ExternalGameController.OnAppStarted -= OnAppStarted;
            _context.ExternalGameController.OnAppExited  -= OnAppExited;
        }

        private void OnAppStarted(OSUtils.ProcessStartedData data, EmulatorConfiguration emulator, string game)
        {
        }

        private void OnAppExited(OSUtils.ProcessExitedData data, EmulatorConfiguration emulator, string game) => _gameRunning = false;
    }
}
