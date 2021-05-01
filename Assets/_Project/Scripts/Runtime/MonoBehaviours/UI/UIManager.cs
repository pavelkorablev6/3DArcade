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
    [DisallowMultipleComponent]
    public sealed class UIManager : MonoBehaviour
    {
        [SerializeField] private UICanvasController _normalUI;
        [SerializeField] private UICanvasController _virtualRealityUI;

        public UICanvasController NormalUI => _normalUI;
        public UICanvasController VirtualRealityUI => _virtualRealityUI;

        private UIContext _uiContext;

        private void Awake() => _uiContext = new UIContext(this);

        private void Start() => TransitionTo<UIDisabledState>();

        public void TransitionTo<T>() where T : UIState => _uiContext.TransitionTo<T>();

        public void InitStatusBar(string message)
        {
            _normalUI.InitStatusBar(message);
            _virtualRealityUI.InitStatusBar(message);
        }

        public void UpdateStatusBar(float percentComplete)
        {
            _normalUI.UpdateStatusBar(percentComplete);
            _virtualRealityUI.UpdateStatusBar(percentComplete);
        }
    }
}
