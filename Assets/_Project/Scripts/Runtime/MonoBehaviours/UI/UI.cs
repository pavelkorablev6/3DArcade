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
    public sealed class UI : MonoBehaviour
    {
        [SerializeField] private UICanvasController _standardUI;
        [SerializeField] private UICanvasController _virtualRealityUI;
        [SerializeField] private UIContext _uiContext;

        private void Start()
        {
            _uiContext.Initialize(_standardUI, _virtualRealityUI);
            _uiContext.Start();
        }

        public void TransitionTo(System.Type type)
        {
            if (type.BaseType != typeof(UIState))
                return;

            System.Type uiContextType                  = typeof(UIContext);
            System.Reflection.MethodInfo methodInfo    = uiContextType.GetMethod(nameof(UIContext.TransitionTo));
            System.Reflection.MethodInfo genericMethod = methodInfo.MakeGenericMethod(type);
            _ = genericMethod.Invoke(_uiContext, new object[] { });
        }
    }
}
