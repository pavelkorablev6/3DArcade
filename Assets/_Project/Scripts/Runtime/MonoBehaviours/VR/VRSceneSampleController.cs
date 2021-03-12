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
    public sealed class VRSceneSampleController : MonoBehaviour
	{
		private OVRCameraRig _cameraController = null;
        private OVRGridCube _gridCube = null;

        private void Awake()
		{
    		OVRCameraRig[] cameraControllers = GetComponentsInChildren<OVRCameraRig>();
			if (cameraControllers.Length == 0)
				Debug.LogWarning("OVRMainMenu: No OVRCameraRig attached.");
			else if (cameraControllers.Length > 1)
				Debug.LogWarning("OVRMainMenu: More then 1 OVRCameraRig attached.");
			else
				_cameraController = cameraControllers[0];
		}

        private void Start()
		{
			if (Application.isEditor == false)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}

			if (_cameraController != null)
			{
				_gridCube = gameObject.AddComponent<OVRGridCube>();
				_gridCube.SetOVRCameraController(ref _cameraController);
			}
		}
	}
}
