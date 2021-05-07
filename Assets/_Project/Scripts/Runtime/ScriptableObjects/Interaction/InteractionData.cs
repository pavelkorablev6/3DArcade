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
    public abstract class InteractionData : ScriptableObject
    {
        [SerializeField, Layer] protected int _selectionLayer;

        public ModelConfigurationComponent CurrentTarget { get; protected set; }
        public ModelConfigurationComponent LastTarget { get; protected set; }

        private int _savedLayer;

        public virtual void Set(RaycastHit raycastHit) => Set(raycastHit.transform.GetComponent<ModelConfigurationComponent>());

        public virtual void Reset()
        {
            ResetCurrentTargetLayer();

            CurrentTarget = null;
            LastTarget    = null;
            _savedLayer   = 0;
        }

        protected void Set(ModelConfigurationComponent target)
        {
            CurrentTarget = target;
            if (CurrentTarget == null)
                return;

            _savedLayer = CurrentTarget.gameObject.layer;
            CurrentTarget.gameObject.SetLayersRecursively(_selectionLayer);

            LastTarget = CurrentTarget;
        }

        protected void ResetCurrentTargetLayer()
        {
            if (CurrentTarget != null)
                CurrentTarget.gameObject.SetLayersRecursively(_savedLayer);
        }
    }
}
