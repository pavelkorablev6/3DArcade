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

using System;
using UnityEngine;

namespace Arcade
{
    [RequireComponent(typeof(CharacterController))]
	public sealed class VRPlayerController : MonoBehaviour
	{
		public event Action CameraUpdated;
		public event Action PreCharacterMove;

		public float Acceleration = 0.1f;
		public float Damping = 0.3f;
		public float BackAndSideDampen = 0.5f;
		public float JumpForce = 0.3f;
		public float RotationAmount = 1.5f;
		public float RotationRatchet = 45f;
		[Tooltip("The player will rotate in fixed steps if Snap Rotation is enabled.")]
		public bool SnapRotation = true;
		[Tooltip("How many fixed speeds to use with linear movement? 0=linear control")]
		public int FixedSpeedSteps;
		public bool HmdResetsY = true;
		public bool HmdRotatesY = true;
		public float GravityModifier = 1f;
		public bool useProfileData = true;
		[NonSerialized] public float CameraHeight;
		public event Action<Transform> TransformUpdated;
		[NonSerialized] public bool Teleported;
		public bool EnableLinearMovement = true;
		public bool EnableRotation = true;
		public bool RotationEitherThumbstick = false;

		public float InitialYRotation { get; private set; }

		private const float SIMULATION_RATE = 60f;

		private CharacterController _controller = null;
		private OVRCameraRig _cameraRig = null;
		private float _moveScale = 1f;
		private Vector3 _moveThrottle = Vector3.zero;
		private float _fallSpeed = 0f;
		private OVRPose? _initialPose;
		private float _moveScaleMultiplier = 0.5f;
		private float _rotationScaleMultiplier = 1f;
		private bool _skipMouseRotation = true;
		private bool _haltUpdateMovement = false;
		private bool _prevHatLeft = false;
		private bool _prevHatRight = false;
		private float _buttonRotation = 0f;
		private bool _readyToSnapTurn;
		private bool _playerControllerEnabled = false;

		private void Start()

		{
			Vector3 p = _cameraRig.transform.localPosition;
			p.z = OVRManager.profile.eyeDepth;
			_cameraRig.transform.localPosition = p;
		}

		private void Awake()
		{
			_controller = GetComponent<CharacterController>();
			if (_controller == null)
				Debug.LogWarning("OVRPlayerController: No CharacterController attached.");

			OVRCameraRig[] CameraRigs = GetComponentsInChildren<OVRCameraRig>();

			if (CameraRigs.Length == 0)
				Debug.LogWarning("OVRPlayerController: No OVRCameraRig attached.");
			else if (CameraRigs.Length > 1)
				Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraRig attached.");
			else
				_cameraRig = CameraRigs[0];

			InitialYRotation = transform.rotation.eulerAngles.y;
		}

		private void OnDisable()
		{
			if (_playerControllerEnabled)
			{
				OVRManager.display.RecenteredPose -= ResetOrientation;

				if (_cameraRig != null)
					_cameraRig.UpdatedAnchors -= UpdateTransform;

				_playerControllerEnabled = false;
			}
		}

		private void Update()
		{
			if (!_playerControllerEnabled)
			{
				if (OVRManager.OVRManagerinitialized)
				{
					OVRManager.display.RecenteredPose += ResetOrientation;

					if (_cameraRig != null)
						_cameraRig.UpdatedAnchors += UpdateTransform;

					_playerControllerEnabled = true;
				}
				else
					return;
			}
		}

		private void UpdateController()
		{
			if (useProfileData)
			{
				if (_initialPose == null)
				{
					_initialPose = new OVRPose()
					{
						position = _cameraRig.transform.localPosition,
						orientation = _cameraRig.transform.localRotation
					};
				}

				Vector3 p = _cameraRig.transform.localPosition;

				if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.EyeLevel)
					p.y = OVRManager.profile.eyeHeight - (0.5f * _controller.height) + _controller.center.y;
				else if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.FloorLevel)
					p.y = -(0.5f * _controller.height) + _controller.center.y;

				_cameraRig.transform.localPosition = p;
			}
			else if (_initialPose != null)
			{
				_cameraRig.transform.localPosition = _initialPose.Value.position;
				_cameraRig.transform.localRotation = _initialPose.Value.orientation;
				_initialPose = null;
			}

			CameraHeight = _cameraRig.centerEyeAnchor.localPosition.y;

			CameraUpdated?.Invoke();

			UpdateMovement();

			Vector3 moveDirection = Vector3.zero;

			float motorDamp = (1f + (Damping * SIMULATION_RATE * Time.deltaTime));

			_moveThrottle.x /= motorDamp;
			_moveThrottle.y = (_moveThrottle.y > 0f) ? (_moveThrottle.y / motorDamp) : _moveThrottle.y;
			_moveThrottle.z /= motorDamp;

			moveDirection += _moveThrottle * SIMULATION_RATE * Time.deltaTime;

			if (_controller.isGrounded && _fallSpeed <= 0f)
				_fallSpeed = ((Physics.gravity.y * (GravityModifier * 0.002f)));
			else
				_fallSpeed += ((Physics.gravity.y * (GravityModifier * 0.002f)) * SIMULATION_RATE * Time.deltaTime);

			moveDirection.y += _fallSpeed * SIMULATION_RATE * Time.deltaTime;

			if (_controller.isGrounded && _moveThrottle.y <= transform.lossyScale.y * 0.001f)
			{
				float bumpUpOffset = Mathf.Max(_controller.stepOffset, new Vector3(moveDirection.x, 0f, moveDirection.z).magnitude);
				moveDirection -= bumpUpOffset * Vector3.up;
			}

			if (PreCharacterMove != null)
			{
				PreCharacterMove();
				Teleported = false;
			}

			Vector3 predictedXZ = Vector3.Scale((_controller.transform.localPosition + moveDirection), new Vector3(1f, 0f, 1f));

				_ = _controller.Move(moveDirection);
			Vector3 actualXZ = Vector3.Scale(_controller.transform.localPosition, new Vector3(1f, 0f, 1f));

			if (predictedXZ != actualXZ)
				_moveThrottle += (actualXZ - predictedXZ) / (SIMULATION_RATE * Time.deltaTime);
		}

		public void UpdateMovement()
		{
			if (_haltUpdateMovement)
				return;

			if (EnableLinearMovement)
			{
				_moveScale = 1.0f;

				if (!_controller.isGrounded)
					_moveScale = 0.0f;

				_moveScale *= SIMULATION_RATE * Time.deltaTime;

				float moveInfluence = Acceleration * 0.1f * _moveScale * _moveScaleMultiplier;

				Quaternion ort = transform.rotation;
				Vector3 ortEuler = ort.eulerAngles;
				ortEuler.z = ortEuler.x = 0f;
				ort = Quaternion.Euler(ortEuler);

				moveInfluence = Acceleration * 0.1f * _moveScale * _moveScaleMultiplier;

	#if !UNITY_ANDROID // LeftTrigger not avail on Android game pad
				moveInfluence *= 1.0f + OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch);
	#endif
				Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch);

				if (FixedSpeedSteps > 0)
				{
					primaryAxis.y = Mathf.Round(primaryAxis.y * FixedSpeedSteps) / FixedSpeedSteps;
					primaryAxis.x = Mathf.Round(primaryAxis.x * FixedSpeedSteps) / FixedSpeedSteps;
				}

				if (primaryAxis.y > 0.0f)
					_moveThrottle += ort * (primaryAxis.y * transform.lossyScale.z * moveInfluence * Vector3.forward);

				if (primaryAxis.y < 0.0f)
					_moveThrottle += ort * (Mathf.Abs(primaryAxis.y) * transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back);

				if (primaryAxis.x < 0.0f)
					_moveThrottle += ort * (Mathf.Abs(primaryAxis.x) * transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left);

				if (primaryAxis.x > 0.0f)
					_moveThrottle += ort * (primaryAxis.x * transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right);
			}

			if (EnableRotation)
			{
				Vector3 euler = transform.rotation.eulerAngles;
				float rotateInfluence = SIMULATION_RATE * Time.deltaTime * RotationAmount * _rotationScaleMultiplier;

				bool curHatLeft = OVRInput.Get(OVRInput.Button.PrimaryShoulder, OVRInput.Controller.Touch);

				if (curHatLeft && !_prevHatLeft)
					euler.y -= RotationRatchet;

				_prevHatLeft = curHatLeft;

				bool curHatRight = OVRInput.Get(OVRInput.Button.SecondaryShoulder, OVRInput.Controller.Touch);

				if (curHatRight && !_prevHatRight)
					euler.y += RotationRatchet;

				_prevHatRight = curHatRight;

				euler.y += _buttonRotation;
				_buttonRotation = 0f;

	#if !UNITY_ANDROID || UNITY_EDITOR
				if (!_skipMouseRotation)
					euler.y += Input.GetAxis("Mouse X") * rotateInfluence * 3.25f;
	#endif
				if (SnapRotation)
				{
					if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft, OVRInput.Controller.Touch) || (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.Touch)))
					{
						if (_readyToSnapTurn)
						{
							euler.y -= RotationRatchet;
							_readyToSnapTurn = false;
						}
					}
					else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight, OVRInput.Controller.Touch) || (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.Touch)))
					{
						if (_readyToSnapTurn)
						{
							euler.y += RotationRatchet;
							_readyToSnapTurn = false;
						}
					}
					else
						_readyToSnapTurn = true;
				}
				else
				{
					Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch);
					if (RotationEitherThumbstick)
					{
						Vector2 altSecondaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch);
						if (secondaryAxis.sqrMagnitude < altSecondaryAxis.sqrMagnitude)
							secondaryAxis = altSecondaryAxis;
					}
					euler.y += secondaryAxis.x * rotateInfluence;
				}

				transform.rotation = Quaternion.Euler(euler);
			}
		}

		public void UpdateTransform(OVRCameraRig rig)
		{
			Transform root = _cameraRig.trackingSpace;
			Transform centerEye = _cameraRig.centerEyeAnchor;

			if (HmdRotatesY && !Teleported)
			{
				Vector3 prevPos = root.position;
				Quaternion prevRot = root.rotation;

				transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

				root.position = prevPos;
				root.rotation = prevRot;
			}

			UpdateController();
			TransformUpdated?.Invoke(root);
		}

		public bool Jump()
		{
			if (!_controller.isGrounded)
				return false;

			_moveThrottle += new Vector3(0, transform.lossyScale.y * JumpForce, 0);

			return true;
		}

		public void Stop()
		{
			_ = _controller.Move(Vector3.zero);
			_moveThrottle = Vector3.zero;
			_fallSpeed = 0.0f;
		}

		public void GetMoveScaleMultiplier(ref float moveScaleMultiplier) => moveScaleMultiplier = _moveScaleMultiplier;

		public void SetMoveScaleMultiplier(float moveScaleMultiplier) => _moveScaleMultiplier = moveScaleMultiplier;

		public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier) => rotationScaleMultiplier = _rotationScaleMultiplier;

		public void SetRotationScaleMultiplier(float rotationScaleMultiplier) => _rotationScaleMultiplier = rotationScaleMultiplier;

		public void GetSkipMouseRotation(ref bool skipMouseRotation) => skipMouseRotation = _skipMouseRotation;

		public void SetSkipMouseRotation(bool skipMouseRotation) => _skipMouseRotation = skipMouseRotation;

		public void GetHaltUpdateMovement(ref bool haltUpdateMovement) => haltUpdateMovement = _haltUpdateMovement;

		public void SetHaltUpdateMovement(bool haltUpdateMovement) => _haltUpdateMovement = haltUpdateMovement;

		public void ResetOrientation()
		{
			if (HmdResetsY && !HmdRotatesY)
			{
				Vector3 euler = transform.rotation.eulerAngles;
				euler.y = InitialYRotation;
				transform.rotation = Quaternion.Euler(euler);
			}
		}
	}
}
