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
    [CreateAssetMenu(menuName = "3DArcade/Interaction/EditModeController", fileName = "EditModeInteractionController")]
    public sealed class EditModeInteractionController : InteractionController<EditModeInteractionRaycaster, EditModeInteractionData>
    {
        public void ManualMoveAndRotate(Vector2 direction, float rotation)
        {
            EditModeInteractionData interactionData = _raycaster.InteractionData;

            if (interactionData.Rigidbody == null)
                return;

            interactionData.Rigidbody.constraints = RigidbodyConstraints.None;

            // Position
            if (direction.sqrMagnitude > 0.001f)
            {
                interactionData.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                interactionData.Rigidbody.AddForce(-interactionData.Current.transform.forward * direction.y, ForceMode.VelocityChange);
                interactionData.Rigidbody.AddForce(-interactionData.Current.transform.right * direction.x, ForceMode.VelocityChange);
            }
            interactionData.Rigidbody.AddForce(Vector3.right * -interactionData.Rigidbody.velocity.x, ForceMode.VelocityChange);
            interactionData.Rigidbody.AddForce(Vector3.forward * -interactionData.Rigidbody.velocity.z, ForceMode.VelocityChange);

            // Rotation
            if (rotation < -0.5f || rotation > 0.5f)
            {
                interactionData.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                float angle           = Mathf.Atan2(interactionData.Current.transform.forward.x, interactionData.Current.transform.forward.z) * Mathf.Rad2Deg;
                float targetAngle     = angle + rotation;
                float angleDifference = (targetAngle - angle);

                if (Mathf.Abs(angleDifference) > 180f)
                {
                    if (angleDifference < 0f)
                        angleDifference = (360f + angleDifference);
                    else if (angleDifference > 0f)
                        angleDifference = (360f - angleDifference) * -1f;
                }

                interactionData.Rigidbody.AddTorque(Vector3.up * angleDifference, ForceMode.VelocityChange);
                interactionData.Rigidbody.AddTorque(Vector3.up * -interactionData.Rigidbody.angularVelocity.y, ForceMode.VelocityChange);
            }
        }

        public void AutoMoveAndRotate(Ray ray, Vector3 forward, float maxDistance, LayerMask layerMask)
        {
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, layerMask))
                return;

            EditModeInteractionData interactionData = _raycaster.InteractionData;

            Vector3 newPosition;
            Transform transform = interactionData.Current.transform;
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
            Collider collider = interactionData.Collider;
            if (collider == null)
                return;

            Vector3 positionOffset  = normal * Mathf.Max(collider.bounds.extents.x + 0.1f, collider.bounds.extents.z + 0.1f);
            newPosition             = new Vector3(position.x, transform.position.y, position.z) + positionOffset;
            transform.position      = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 12f);
            transform.localRotation = Quaternion.LookRotation(normal);
        }
    }
}
