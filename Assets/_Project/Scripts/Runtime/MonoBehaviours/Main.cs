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

using SK.Utilities.Unity;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Zenject;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class Main : MonoBehaviour
    {
        private InputActions _inputActions;
        private ArcadeContext _sceneContext;

        [Inject, SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "DI")]
        private void Construct(InputActions inputActions, ArcadeContext sceneContext)
        {
            _inputActions = inputActions;
            _sceneContext = sceneContext;
        }

        private void Start()
        {
            QualitySettings.vSyncCount  = 0;
            Application.targetFrameRate = -1;
            Time.timeScale              = 1f;

            CheckCurrentOS();

            _sceneContext.Start();
        }

        private void OnEnable() => _inputActions.Global.Enable();

        private void OnDisable() => _inputActions.Global.Disable();

#if !UNITY_EDITOR
        private const int SLEEP_TIME = 200;
        private bool _focused = true;
        private void OnApplicationFocus(bool focus) => _focused = focus;
        private void Update()
        {
            if (!_focused)
            {
                System.Threading.Thread.Sleep(SLEEP_TIME);
                return;
            }
            _sceneContext.Update(Time.deltaTime);
        }
#else
        private void Update() => _sceneContext.Update(Time.deltaTime);
#endif
        private void FixedUpdate() => _sceneContext.FixedUpdate(Time.fixedDeltaTime);

        private void CheckCurrentOS()
        {
            try
            {
                OS currentOS = SystemUtils.GetCurrentOS();
                Debug.Log($"Current OS: {currentOS}");
            }
            catch (System.Exception e)
            {
                ApplicationUtils.ExitApp(e.Message);
            }
        }
    }
}
