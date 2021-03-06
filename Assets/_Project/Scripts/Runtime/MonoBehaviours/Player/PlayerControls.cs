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
    public sealed class PlayerControls : MonoBehaviour
    {
        [SerializeField] private GameObject _fpsController;
        [SerializeField] private GameObject _cylController;

        public bool Active { get; private set; } = true;

        public Transform ActiveTransform => _fpsController.activeInHierarchy
                                          ? _fpsController.transform
                                          : _cylController.transform;

        public void EnableFpsController()
        {
            _cylController.SetActive(false);
            _fpsController.SetActive(true);
            Active = true;
        }

        public void EnableCylController()
        {
            _fpsController.SetActive(false);
            _cylController.SetActive(true);
            Active = true;
        }

        public void Disable()
        {
            _fpsController.transform.position = new Vector3(0f, 1f, 0f);
            _fpsController.SetActive(false);

            _cylController.transform.position = new Vector3(0f, 1f, 0f);
            _cylController.SetActive(false);

             Active = false;
        }
    }
}
