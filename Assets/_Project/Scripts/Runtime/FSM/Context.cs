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

using System.Collections.Generic;

namespace Arcade.FSM
{
    public abstract class Context<T> where T : State<T>
    {
        private readonly List<T> _states = new List<T>();

        public T CurrentState  { get; private set; } = null;
        public T PreviousState { get; private set; } = null;

        public void Start() => OnStart();

        public void Update(float dt)
        {
            OnUpdate(dt);
            CurrentState?.OnUpdate(dt);
        }

        public void FixedUpdate(float dt)
        {
            OnFixedUpdate(dt);
            CurrentState?.OnFixedUpdate(dt);
        }

        public void TransitionTo<U>() where U : T
        {
            T foundState = _states.Find(x => x.GetType() == typeof(U));
            if (foundState != null)
            {
                PreviousState = CurrentState;
                PreviousState?.OnExit();
                CurrentState = foundState;
                CurrentState.OnEnter();
            }
            else
            {
                U newState = System.Activator.CreateInstance(typeof(U), this) as U;
                _states.Add(newState);
                TransitionTo<U>();
            }
        }

        public void TransitionToPrevious()
        {
            if (PreviousState == null)
                return;

            PreviousState.OnExit();
            CurrentState = PreviousState;
            CurrentState.OnEnter();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnUpdate(float dt)
        {
        }

        protected virtual void OnFixedUpdate(float dt)
        {
        }
    }
}
