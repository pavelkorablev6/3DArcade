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

namespace Arcade
{
    public abstract class ArcadeExternalGameState : ArcadeState
    {
        private bool _gameRunning;

        public ArcadeExternalGameState(ArcadeContext context)
        : base(context)
        {
        }

        public sealed override void OnEnter()
        {
            OnEnterState();

            ExternalGameController gameController = _context.GameControllers.External;

            gameController.OnGameStarted += OnAppStarted;
            gameController.OnGameExited  += OnAppExited;

            EmulatorConfiguration emulator = _context.InteractionControllers.NormalModeController.InteractionData.CurrentTarget.Configuration.EmulatorConfiguration;
            if (!gameController.StartGame(emulator, _context.InteractionControllers.NormalModeController.InteractionData.CurrentTarget.Configuration.Id))
            {
                _context.TransitionToPrevious();
                return;
            }

            _gameRunning = true;
        }

        public sealed override void OnExit()
        {
            ExternalGameController gameController = _context.GameControllers.External;

            gameController.StopCurrent();

            gameController.OnGameStarted -= OnAppStarted;
            gameController.OnGameExited  -= OnAppExited;

            OnExitState();
        }

        public sealed override void OnUpdate(float dt)
        {
            if (!_gameRunning)
                _context.TransitionToPrevious();
        }

        protected abstract void OnEnterState();

        protected abstract void OnExitState();

        private void OnAppStarted(OSUtils.ProcessStartedData data, EmulatorConfiguration emulator, string game)
        {
        }

        private void OnAppExited(OSUtils.ProcessExitedData data, EmulatorConfiguration emulator, string game)
            => _gameRunning = false;
    }
}
