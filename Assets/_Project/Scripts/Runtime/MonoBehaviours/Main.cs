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
using UnityEngine.InputSystem;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class Main : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private InputActionAsset _inputActionAsset;

        public InputActionAsset InputActionAsset => _inputActionAsset;

        private InputActionMap _globalInputActions;
        private UIController _uiController;

        private SceneContext _sceneContext;

        private void Awake()
        {
            QualitySettings.vSyncCount  = 0;
            Application.targetFrameRate = -1;
            Time.timeScale              = 1f;

            _globalInputActions = _inputActionAsset.FindActionMap("Global");
            _uiController       = new UIController();
        }

        private void Start()
        {
            SystemUtils.HideMouseCursor();

            _sceneContext = new SceneContext(_player, _uiController, _globalInputActions);
            _sceneContext.TransitionTo<SceneLoadState>();
        }

        private void OnEnable() => _globalInputActions.Enable();

        private void OnDisable() => _globalInputActions.Disable();

#if !UNITY_EDITOR
        private const bool SLEEP_TIME = 200;
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
    }
}
