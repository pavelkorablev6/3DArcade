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
        [SerializeField, Layer] private int _grabLayer;

        [field: System.NonSerialized] public Collider Collider { get; private set; }
        [field: System.NonSerialized] public Rigidbody Rigidbody { get; private set; }

        [System.NonSerialized] private PhysicsState _savedData;

        public void InitAutoMove()
        {
            if (Collider == null || Rigidbody == null)
                return;

            _savedData = SavePhysicsState();

            Current.gameObject.SetLayerRecursively(_grabLayer);

            Collider.isTrigger = true;

            Rigidbody.velocity               = Vector3.zero;
            Rigidbody.angularVelocity        = Vector3.zero;
            Rigidbody.interpolation          = RigidbodyInterpolation.None;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Rigidbody.isKinematic            = true;
        }

        public void DeInitAutoMove()
        {
            RestorePhysicsState();

            Current.gameObject.SetLayerRecursively(_highlightLayer);

            Collider   = null;
            Rigidbody  = null;
            _savedData = null;
        }

        protected override void OnSet(ModelConfigurationComponent previous)
        {
            if (previous != null && previous.TryGetComponent(out Rigidbody previousRb))
            {
                previousRb.velocity        = Vector3.zero;
                previousRb.angularVelocity = Vector3.zero;
            }

            Current.gameObject.SetLayerRecursively(_highlightLayer);

            Collider  = Current.GetComponent<Collider>();
            Rigidbody = Current.GetComponent<Rigidbody>();
        }

        private PhysicsState SavePhysicsState() => new PhysicsState
        {
            ColliderIsTrigger      = Collider.isTrigger,
            CollisionDetectionMode = Rigidbody.collisionDetectionMode,
            RigidbodyInterpolation = Rigidbody.interpolation,
            RigidbodyIsKinematic   = Rigidbody.isKinematic
        };

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
        }
    }
}
