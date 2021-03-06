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
    [SelectionBase, DisallowMultipleComponent]
    public sealed class ModelConfigurationComponent : MonoBehaviour
    {
        private sealed class PhysicsState
        {
            public bool IsTrigger                                = false;
            public CollisionDetectionMode CollisionDetectionMode = CollisionDetectionMode.Discrete;
            public RigidbodyInterpolation Interpolation          = RigidbodyInterpolation.None;
            public bool IsKinematic                              = false;
        }

        [SerializeField] private ModelConfiguration _modelConfiguration;

        public ModelConfiguration Configuration => _modelConfiguration;
        public Collider Collider   { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public bool MoveCabMovable   => _modelConfiguration.MoveCabMovable && Collider != null && Rigidbody != null;
        public bool MoveCabGrabbable => _modelConfiguration.MoveCabGrabbable && Collider != null && Rigidbody != null;

        private int _originalLayer;
        private int _previousLayer;
        private TransformState _savedTransformState;
        private PhysicsState _savedPhysicsState;

        private void OnEnable()
        {
            Collider  = GetComponent<Collider>();
            Rigidbody = GetComponent<Rigidbody>();
        }

        public void InitialSetup(ModelConfiguration modelConfiguration, int layer)
        {
            _modelConfiguration = modelConfiguration;
            _originalLayer      = layer;

            gameObject.name = modelConfiguration.Id;
            SetLayer(layer);

            transform.localScale = modelConfiguration.Scale;
        }

        public void SetLayer(int layer) => gameObject.SetLayerRecursively(layer);

        public void RestoreLayerToOriginal() => SetLayer(_originalLayer);

        public ModelConfiguration GetModelConfigurationWithUpdatedTransforms()
        {
            _modelConfiguration.Position = transform.localPosition;
            _modelConfiguration.Rotation = MathUtils.ClampEulerAngles(transform.localEulerAngles);
            _modelConfiguration.Scale    = transform.localScale;
            return _modelConfiguration;
        }

        public bool InitAutoMove(int grabLayer)
        {
            if (Collider == null || Rigidbody == null)
                return false;

            SavePhysicsState();

            _previousLayer = gameObject.layer;
            SetLayer(grabLayer);

            Collider.isTrigger = true;

            Rigidbody.velocity               = Vector3.zero;
            Rigidbody.angularVelocity        = Vector3.zero;
            Rigidbody.interpolation          = RigidbodyInterpolation.None;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Rigidbody.isKinematic            = true;

            return true;
        }

        public void DeInitAutoMove()
        {
            RestorePhysicsState();
            RestoreLayerToPrevious();
        }

        public void SaveTransformState() => _savedTransformState = new TransformState
        {
            Position = transform.position,
            Rotation = transform.localEulerAngles,
            Scale    = transform.localScale
        };

        public void RestoreTransformState()
        {
            if (_savedTransformState is null)
                return;

            transform.position         = _savedTransformState.Position;
            transform.localEulerAngles = _savedTransformState.Rotation;
            transform.localScale       = _savedTransformState.Scale;

            _savedTransformState = null;
        }

        private void SavePhysicsState() => _savedPhysicsState = new PhysicsState
        {
            IsTrigger              = Collider.isTrigger,
            CollisionDetectionMode = Rigidbody.collisionDetectionMode,
            Interpolation          = Rigidbody.interpolation,
            IsKinematic            = Rigidbody.isKinematic
        };

        private void RestorePhysicsState()
        {
            if (_savedPhysicsState is null)
                return;

            if (Collider != null)
                Collider.isTrigger = _savedPhysicsState.IsTrigger;

            if (Rigidbody != null)
            {
                Rigidbody.collisionDetectionMode = _savedPhysicsState.CollisionDetectionMode;
                Rigidbody.interpolation          = _savedPhysicsState.Interpolation;
                Rigidbody.isKinematic            = _savedPhysicsState.IsKinematic;
            }

            _savedPhysicsState = null;
        }

        private void RestoreLayerToPrevious() => SetLayer(_previousLayer);
    }
}
