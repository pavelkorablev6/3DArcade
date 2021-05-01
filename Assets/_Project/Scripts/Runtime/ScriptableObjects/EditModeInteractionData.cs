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
using UnityEngine;

namespace Arcade
{
    [CreateAssetMenu(menuName = "Arcade/EditModeInteractionData", fileName = "EditModeInteractionData")]
    public sealed class EditModeInteractionData : InteractionData
    {
        private sealed class SavedData
        {
            public bool ColliderIsTrigger;
            public CollisionDetectionMode CollisionDetectionMode;
            public RigidbodyInterpolation RigidbodyInterpolation;
            public bool RigidbodyIsKinematic;
        }

        [SerializeField, Layer] private int _highlightLayer;

        public Collider Collider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Vector2 ScreenPoint { get; private set; }
        public Vector2 AimPosition { get; private set; }
        public float AimRotation { get; private set; }

        private SavedData _savedData;

        public override void Set(RaycastHit raycastHit)
        {
            base.Set(raycastHit);

            Collider  = raycastHit.collider;
            Rigidbody = raycastHit.rigidbody;
            if (Rigidbody != null)
                Rigidbody.angularVelocity = Vector3.zero;
        }

        public override void Reset()
        {
            base.Reset();

            Collider      = null;
            if (Rigidbody != null)
                Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody   = null;
            ScreenPoint = Vector2.zero;
            AimPosition = Vector2.zero;
            AimRotation = 0f;
        }

        public void InitGrabMode()
        {
            _savedData = new SavedData
            {
                ColliderIsTrigger      = Collider.isTrigger,
                RigidbodyInterpolation = Rigidbody.interpolation,
                CollisionDetectionMode = Rigidbody.collisionDetectionMode,
                RigidbodyIsKinematic   = Rigidbody.isKinematic
            };

            CurrentTarget.gameObject.layer   = 0;
            Collider.isTrigger               = true;
            Rigidbody.interpolation          = RigidbodyInterpolation.Interpolate;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Rigidbody.isKinematic            = true;
        }

        public void RestoreValues()
        {
            if (_savedData == null)
                return;

            Collider.isTrigger               = _savedData.ColliderIsTrigger;
            Rigidbody.interpolation          = _savedData.RigidbodyInterpolation;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Rigidbody.isKinematic            = _savedData.RigidbodyIsKinematic;
            Rigidbody.collisionDetectionMode = _savedData.CollisionDetectionMode;

            _savedData = null;
        }

        public void SetAimData(Camera camera, Vector2 aimPosition, float aimRotation)
        {
            ScreenPoint = camera.WorldToScreenPoint(CurrentTarget.transform.position);
            AimPosition = aimPosition;
            AimRotation = aimRotation;
        }
    }
}
