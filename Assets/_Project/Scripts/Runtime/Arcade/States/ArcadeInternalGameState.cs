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
    public abstract class ArcadeInternalGameState : ArcadeState
    {
        private ScreenNodeTag _screenNode;
        private Renderer _screenRenderer;
        private Material _savedMaterial;
        private DynamicArtworkComponent _dynamicArtworkComponent;

        public ArcadeInternalGameState(ArcadeContext context)
        : base(context)
        {
        }

        public override void OnEnter()
        {
            Debug.Log($">> <color=green>Entered</color> {GetType().Name}");

            if (_context.InteractionController.CurrentTargetComponent == null)
                _context.TransitionToPrevious();

            ScreenNodeTag screenNodeTag = _context.InteractionController.CurrentTargetComponent.GetComponentInChildren<ScreenNodeTag>(true);
            if (screenNodeTag == null)
                _context.TransitionToPrevious();

            _context.VideoPlayerController.StopCurrentVideo();

            if (!_context.InternalGameController.StartGame(screenNodeTag, _context.InteractionController.CurrentTarget))
                _context.TransitionToPrevious();

            _screenNode = screenNodeTag;

            if (_screenNode.TryGetComponent(out _dynamicArtworkComponent))
                _dynamicArtworkComponent.enabled = false;

            _screenRenderer = _screenNode.GetComponent<Renderer>();
            _savedMaterial  = _screenRenderer.material;
            _screenRenderer.material = _context.InternalGameController.ScreenMaterial;
        }

        public override void OnExit()
        {
            Debug.Log($">> <color=orange>Exited</color> {GetType().Name}");

            if (_screenRenderer != null)
                _screenRenderer.material = _savedMaterial;

            if (_dynamicArtworkComponent != null)
                _dynamicArtworkComponent.enabled = true;

            _context.InternalGameController.StopGame();
        }

        public override void OnUpdate(float dt)
        {
            if (_context.InputActions.Global.Quit.triggered)
                _context.TransitionToPrevious();

            _context.InternalGameController.UpdateGame();
        }
    }
}
