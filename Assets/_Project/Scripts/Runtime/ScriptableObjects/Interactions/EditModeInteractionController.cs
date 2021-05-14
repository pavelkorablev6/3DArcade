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
    [CreateAssetMenu(menuName = "Arcade/Interaction/EditModeInteractionController", fileName = "EditModeInteractionController")]
    public sealed class EditModeInteractionController : InteractionController<EditModeInteractionData>
    {
        protected override Ray GetRay()
        {
            if (Cursor.lockState == CursorLockMode.Locked || Mouse.current == null)
                return Camera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            return Camera.ScreenPointToRay(mousePosition);
        }

        public void ManualMoveAndRotate()
        {
            if (_interactionData.Rigidbody == null)
                return;

            _interactionData.Rigidbody.constraints = RigidbodyConstraints.None;

            // Position
            Vector2 aimPosition = _interactionData.AimPosition;
            if (aimPosition.sqrMagnitude > 0.001f)
            {
                _interactionData.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                _interactionData.Rigidbody.AddForce(-_interactionData.Current.transform.forward * aimPosition.y, ForceMode.VelocityChange);
                _interactionData.Rigidbody.AddForce(-_interactionData.Current.transform.right * aimPosition.x, ForceMode.VelocityChange);
            }
            _interactionData.Rigidbody.AddForce(Vector3.right * -_interactionData.Rigidbody.velocity.x, ForceMode.VelocityChange);
            _interactionData.Rigidbody.AddForce(Vector3.forward * -_interactionData.Rigidbody.velocity.z, ForceMode.VelocityChange);

            // Rotation
            float aimRotation = _interactionData.AimRotation;
            if (aimRotation < -0.5f || aimRotation > 0.5f)
            {
                _interactionData.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                float angle           = Mathf.Atan2(_interactionData.Current.transform.forward.x, _interactionData.Current.transform.forward.z) * Mathf.Rad2Deg;
                float targetAngle     = angle + aimRotation;
                float angleDifference = (targetAngle - angle);

                if (Mathf.Abs(angleDifference) > 180f)
                {
                    if (angleDifference < 0f)
                        angleDifference = (360f + angleDifference);
                    else if (angleDifference > 0f)
                        angleDifference = (360f - angleDifference) * -1f;
                }

                _interactionData.Rigidbody.AddTorque(Vector3.up * angleDifference, ForceMode.VelocityChange);
                _interactionData.Rigidbody.AddTorque(Vector3.up * -_interactionData.Rigidbody.angularVelocity.y, ForceMode.VelocityChange);
            }
        }

        public void AutoMoveAndRotate(Ray ray, Vector3 forward, float maxDistance, LayerMask layerMask)
        {
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, layerMask))
                return;

            Vector3 newPosition;
            Transform transform = _interactionData.Current.transform;
            Vector3 position    = hitInfo.point;
            Vector3 normal      = hitInfo.normal;
            float dot           = Vector3.Dot(Vector3.up, normal);

            // Floor
            if (dot > 0.05f)
            {
                newPosition             = new Vector3(position.x, position.y + 0.1f, position.z);
                transform.position      = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 12f);
                transform.localRotation = Quaternion.FromToRotation(Vector3.up, normal) * Quaternion.LookRotation(-forward);
                return;
            }

            // Ceiling
            if (dot < -0.05f)
            {
                newPosition             = new Vector3(position.x, transform.position.y, position.z);
                transform.position      = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 12f);
                transform.localRotation = Quaternion.FromToRotation(Vector3.up, -normal) * Quaternion.LookRotation(-forward);
                return;
            }

            // Vertical surface
            Collider collider       = _interactionData.Collider;
            Vector3 positionOffset  = normal * Mathf.Max(collider.bounds.extents.x + 0.1f, collider.bounds.extents.z + 0.1f);
            newPosition             = new Vector3(position.x, transform.position.y, position.z) + positionOffset;
            transform.position      = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 12f);
            transform.localRotation = Quaternion.LookRotation(normal);
        }
    }
}
