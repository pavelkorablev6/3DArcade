/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using UnityEngine;

namespace Arcade
{
    /// <summary>
    /// Simple helper script that conditionally enables rendering of a controller if it is connected.
    /// </summary>
    public sealed class VRControllerHelper : MonoBehaviour
    {
        private enum ControllerType
        {
            QuestAndRiftS = 1,
            Rift          = 2,
            Quest2        = 3,
        }

        /// <summary>
        /// The root GameObject that represents the Oculus Touch for Quest And RiftS Controller model (Left).
        /// </summary>
        [SerializeField] private GameObject _modelOculusTouchQuestAndRiftSLeftController;

        /// <summary>
        /// The root GameObject that represents the Oculus Touch for Quest And RiftS Controller model (Right).
        /// </summary>
        [SerializeField] private GameObject _modelOculusTouchQuestAndRiftSRightController;

        /// <summary>
        /// The root GameObject that represents the Oculus Touch for Rift Controller model (Left).
        /// </summary>
        [SerializeField] private GameObject _modelOculusTouchRiftLeftController;

        /// <summary>
        /// The root GameObject that represents the Oculus Touch for Rift Controller model (Right).
        /// </summary>
        [SerializeField] private GameObject _modelOculusTouchRiftRightController;

        /// <summary>
        /// The root GameObject that represents the Oculus Touch for Quest 2 Controller model (Left).
        /// </summary>
        [SerializeField] private GameObject _modelOculusTouchQuest2LeftController;

        /// <summary>
        /// The root GameObject that represents the Oculus Touch for Quest 2 Controller model (Right).
        /// </summary>
        [SerializeField] private GameObject _modelOculusTouchQuest2RightController;

        /// <summary>
        /// The controller that determines whether or not to enable rendering of the controller model.
        /// </summary>
        [SerializeField] private OVRInput.Controller _controller;

        private const string ANIMATOR_BUTTON_1_STRING   = "Button 1";
        private const string ANIMATOR_BUTTON_2_STRING   = "Button 2";
        private const string ANIMATOR_BUTTON_3_STRING   = "Button 3";
        private const string ANIMATOR_JOYSTICK_X_STRING = "Joy X";
        private const string ANIMATOR_JOYSTICK_Y_STRING = "Joy Y";
        private const string ANIMATOR_TRIGGER_STRING    = "Trigger";
        private const string ANIMATOR_GRIP_STRING       = "Grip";

        private static bool _animatorIdsSet = false;
        private static int _animatorButton1Id;
        private static int _animatorButton2Id;
        private static int _animatorButton3Id;
        private static int _animatorJoystickXId;
        private static int _animatorJoystickYId;
        private static int _animatorTriggerId;
        private static int _animatorGripId;

        /// <summary>
        /// The animator component that contains the controller animation controller for animating buttons and triggers.
        /// </summary>
        private ControllerType _activeControllerType = ControllerType.Rift;
        private bool _prevControllerConnected = false;
        private bool _prevControllerConnectedCached = false;
        private Animator _animator;

        private void Awake()
        {
            if (!_animatorIdsSet)
            {
                _animatorButton1Id   = Animator.StringToHash(ANIMATOR_BUTTON_1_STRING);
                _animatorButton2Id   = Animator.StringToHash(ANIMATOR_BUTTON_2_STRING);
                _animatorButton3Id   = Animator.StringToHash(ANIMATOR_BUTTON_3_STRING);
                _animatorJoystickXId = Animator.StringToHash(ANIMATOR_JOYSTICK_X_STRING);
                _animatorJoystickYId = Animator.StringToHash(ANIMATOR_JOYSTICK_Y_STRING);
                _animatorTriggerId   = Animator.StringToHash(ANIMATOR_TRIGGER_STRING);
                _animatorGripId      = Animator.StringToHash(ANIMATOR_GRIP_STRING);

                _animatorIdsSet = true;
            }
        }

        private void Start()
        {
            OVRPlugin.SystemHeadset headset = OVRPlugin.GetSystemHeadsetType();

            switch (headset)
            {
                case OVRPlugin.SystemHeadset.Rift_CV1:
                    _activeControllerType = ControllerType.Rift;
                    break;
                case OVRPlugin.SystemHeadset.Oculus_Quest_2:
                case OVRPlugin.SystemHeadset.Oculus_Link_Quest_2:
                    _activeControllerType = ControllerType.Quest2;
                    break;
                default:
                    _activeControllerType = ControllerType.QuestAndRiftS;
                    break;
            }

            Debug.LogFormat("OVRControllerHelp: Active controller type: {0} for product {1}", _activeControllerType, OVRPlugin.productName);

            // Hide all controller models until controller get connected
            _modelOculusTouchQuestAndRiftSLeftController.SetActive(false);
            _modelOculusTouchQuestAndRiftSRightController.SetActive(false);
            _modelOculusTouchRiftLeftController.SetActive(false);
            _modelOculusTouchRiftRightController.SetActive(false);
            _modelOculusTouchQuest2LeftController.SetActive(false);
            _modelOculusTouchQuest2RightController.SetActive(false);
        }

        private void Update()
        {
            OVRInput.Update();

            bool controllerConnected = OVRInput.IsControllerConnected(_controller);

            if ((controllerConnected != _prevControllerConnected) || !_prevControllerConnectedCached)
            {
                if (_activeControllerType == ControllerType.Rift)
                {
                    _modelOculusTouchQuestAndRiftSLeftController.SetActive(false);
                    _modelOculusTouchQuestAndRiftSRightController.SetActive(false);
                    _modelOculusTouchRiftLeftController.SetActive(controllerConnected && (_controller == OVRInput.Controller.LTouch));
                    _modelOculusTouchRiftRightController.SetActive(controllerConnected && (_controller == OVRInput.Controller.RTouch));
                    _modelOculusTouchQuest2LeftController.SetActive(false);
                    _modelOculusTouchQuest2RightController.SetActive(false);

                    _animator = _controller == OVRInput.Controller.LTouch
                        ? _modelOculusTouchRiftLeftController.GetComponent<Animator>()
                        : _modelOculusTouchRiftRightController.GetComponent<Animator>();
                }
                else if (_activeControllerType == ControllerType.Quest2)
                {
                    _modelOculusTouchQuestAndRiftSLeftController.SetActive(false);
                    _modelOculusTouchQuestAndRiftSRightController.SetActive(false);
                    _modelOculusTouchRiftLeftController.SetActive(false);
                    _modelOculusTouchRiftRightController.SetActive(false);
                    _modelOculusTouchQuest2LeftController.SetActive(controllerConnected && (_controller == OVRInput.Controller.LTouch));
                    _modelOculusTouchQuest2RightController.SetActive(controllerConnected && (_controller == OVRInput.Controller.RTouch));

                    _animator = _controller == OVRInput.Controller.LTouch
                        ? _modelOculusTouchQuest2LeftController.GetComponent<Animator>()
                        : _modelOculusTouchQuest2RightController.GetComponent<Animator>();
                }
                else /*if (activeControllerType == ControllerType.QuestAndRiftS)*/
                {
                    _modelOculusTouchQuestAndRiftSLeftController.SetActive(controllerConnected && (_controller == OVRInput.Controller.LTouch));
                    _modelOculusTouchQuestAndRiftSRightController.SetActive(controllerConnected && (_controller == OVRInput.Controller.RTouch));
                    _modelOculusTouchRiftLeftController.SetActive(false);
                    _modelOculusTouchRiftRightController.SetActive(false);
                    _modelOculusTouchQuest2LeftController.SetActive(false);
                    _modelOculusTouchQuest2RightController.SetActive(false);

                    _animator = _controller == OVRInput.Controller.LTouch
                            ? _modelOculusTouchQuestAndRiftSLeftController.GetComponent<Animator>()
                            : _modelOculusTouchQuestAndRiftSRightController.GetComponent<Animator>();
                }

                _prevControllerConnected       = controllerConnected;
                _prevControllerConnectedCached = true;
            }

            if (_animator != null)
            {
                _animator.SetFloat(_animatorButton1Id, OVRInput.Get(OVRInput.Button.One, _controller) ? 1.0f : 0.0f);
                _animator.SetFloat(_animatorButton2Id, OVRInput.Get(OVRInput.Button.Two, _controller) ? 1.0f : 0.0f);
                _animator.SetFloat(_animatorButton3Id, OVRInput.Get(OVRInput.Button.Start, _controller) ? 1.0f : 0.0f);

                _animator.SetFloat(_animatorJoystickXId, OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, _controller).x);
                _animator.SetFloat(_animatorJoystickYId, OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, _controller).y);

                _animator.SetFloat(_animatorTriggerId, OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, _controller));
                _animator.SetFloat(_animatorGripId, OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller));
            }
        }
    }
}
