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
    [CreateAssetMenu(menuName = "Arcade/Interaction/EditModeInteractionData", fileName = "EditModeInteractionData")]
    public sealed class EditModeInteractionData : InteractionData
    {
        private sealed class PhysicsState
        {
            public bool ColliderIsTrigger                        = false;
            public CollisionDetectionMode CollisionDetectionMode = CollisionDetectionMode.Discrete;
            public RigidbodyInterpolation RigidbodyInterpolation = RigidbodyInterpolation.None;
            public bool RigidbodyIsKinematic                     = false;
        }

        [SerializeField, Layer] private int _highlightLayer;

        [System.NonSerialized] public Vector2 ScreenPoint;
        [System.NonSerialized] public Vector2 AimPosition;
        [System.NonSerialized] public float AimRotation;
        [System.NonSerialized] public Collider Collider;
        [System.NonSerialized] public Rigidbody Rigidbody;

        [System.NonSerialized] private PhysicsState _savedData;

        public void SetAimData(Camera camera, Vector2 aimPosition, float aimRotation)
        {
            if (Current == null)
                return;

            ScreenPoint = camera.WorldToScreenPoint(Current.transform.position);
            AimPosition = aimPosition;
            AimRotation = aimRotation;
        }

        public override void Set(ModelConfigurationComponent target)
        {
            base.Set(target);

            Collider  = target.GetComponent<Collider>();
            Rigidbody = target.GetComponent<Rigidbody>();
            target.gameObject.SetLayerRecursively(_highlightLayer);
        }

        public void InitAutoMove()
        {
            SavePhysicsState();

            if (Collider != null)
                Collider.isTrigger = true;

            if (Rigidbody != null)
            {
                Rigidbody.velocity               = Vector3.zero;
                Rigidbody.angularVelocity        = Vector3.zero;
                Rigidbody.interpolation          = RigidbodyInterpolation.None;
                Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                Rigidbody.isKinematic            = true;
            }
        }

        public void DeInitAutoMove()
        {
            RestorePhysicsState();
            ResetFields();
        }

        private void SavePhysicsState()
        {
            _savedData = new PhysicsState();

            if (Collider != null)
                _savedData.ColliderIsTrigger = Collider.isTrigger;

            if (Rigidbody != null)
            {
                _savedData.CollisionDetectionMode = Rigidbody.collisionDetectionMode;
                _savedData.RigidbodyInterpolation = Rigidbody.interpolation;
                _savedData.RigidbodyIsKinematic   = Rigidbody.isKinematic;
            }
        }

        private void RestorePhysicsState()
        {
            if (_savedData is null)
                return;

            if (Collider != null)
                Collider.isTrigger = _savedData.ColliderIsTrigger;

            if (Rigidbody != null)
            {
                Rigidbody.collisionDetectionMode = _savedData.CollisionDetectionMode;
                Rigidbody.interpolation          = _savedData.RigidbodyInterpolation;
                Rigidbody.isKinematic            = _savedData.RigidbodyIsKinematic;
            }

            _savedData = null;
        }

        private void ResetFields()
        {
            ScreenPoint = Vector2.zero;
            AimPosition = Vector2.zero;
            AimRotation = 0f;
            Collider    = null;
            Rigidbody   = null;
        }
    }
}
