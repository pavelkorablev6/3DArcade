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

        public ModelConfigurationComponentPair TargetPair { get; } = new ModelConfigurationComponentPair();

        private int _savedLayer;

        public virtual void Set(RaycastHit raycastHit) => Set(raycastHit.transform.GetComponent<ModelConfigurationComponent>());

        public virtual void Reset()
        {
            ResetCurrentTargetLayer();

            TargetPair.SetCurrent(null);
            _savedLayer = 0;
        }

        protected void Set(ModelConfigurationComponent target)
        {
            TargetPair.SetCurrent(target);
            if (TargetPair.Current == null)
                return;

            _savedLayer = TargetPair.Current.gameObject.layer;
            TargetPair.Current.gameObject.SetLayersRecursively(_selectionLayer);

            TargetPair.UpdatePrevious();
        }

        protected void ResetCurrentTargetLayer()
        {
            if (TargetPair.Current != null)
                TargetPair.Current.gameObject.SetLayersRecursively(_savedLayer);
        }
    }
}
