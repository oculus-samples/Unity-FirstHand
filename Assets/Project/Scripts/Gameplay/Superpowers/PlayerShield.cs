/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls the deploying and positioning of the player's "shield" object when it is enabled via the active state.
    /// </summary>
    public class PlayerShield : MonoBehaviour
    {
        [SerializeField] private Transform _shieldTransform;
        private bool _isEnabled;

        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _leftHandComponent;

        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _rightHandComponent;

        private IHand _leftHand;
        private IHand _rightHand;

        [SerializeField] private float _maxScale = 1f;
        [SerializeField] private float _scalingSpeed = 20.0f;
        private float _currentScale;

        [SerializeField] private float _moveLerpSpeed = 20.0f;
        private Pose _currentPose;

        [SerializeField, Interface(typeof(IActiveState))]
        private MonoBehaviour _activeState;

        public IActiveState ActiveState { get; private set; }

        [SerializeField]
        UnityEvent _whenStarted;
        [SerializeField]
        UnityEvent _whenEnded;

        private void Awake()
        {
            _leftHand = _leftHandComponent as IHand;
            _rightHand = _rightHandComponent as IHand;
            ActiveState = _activeState as IActiveState;
        }

        private void Start()
        {
            Assert.IsNotNull(_shieldTransform);
            Assert.IsNotNull(_leftHandComponent);
            Assert.IsNotNull(_rightHandComponent);
            Assert.IsNotNull(ActiveState);
        }

        public void OnShieldPoseStarted()
        {
            if (ActiveState.Active)
            {
                _isEnabled = true;
                _currentPose = CalculateDesiredShieldPose();
                _shieldTransform.gameObject.SetActive(true);
                _shieldTransform.SetPose(_currentPose);
                _whenStarted.Invoke();
            }
        }

        public void OnShieldPoseEnded()
        {
            if (_isEnabled)
            {
                _isEnabled = false;
                _whenEnded.Invoke();
            }
        }

        private void Update()
        {
            if (!_isEnabled && _currentScale <= Mathf.Epsilon)
            {
                if (_shieldTransform.gameObject.activeSelf)
                {
                    _shieldTransform.gameObject.SetActive(false);
                }

                return;
            }

            // update scale
            float desiredScale = _isEnabled ? _maxScale : 0f;
            _currentScale = Mathf.Lerp(_currentScale, desiredScale, Time.deltaTime * _scalingSpeed);
            _shieldTransform.localScale = Vector3.one * _currentScale;

            // update transform
            if (_isEnabled)
            {
                Pose desiredPose = CalculateDesiredShieldPose();
                _currentPose.position =
                    Vector3.Lerp(_currentPose.position, desiredPose.position, Time.deltaTime * _moveLerpSpeed);
                _currentPose.rotation = Quaternion.Slerp(_currentPose.rotation, desiredPose.rotation,
                    Time.deltaTime * _moveLerpSpeed);
                _shieldTransform.SetPose(_currentPose);
            }
        }

        /// <summary>
        /// Calculates the ideal pose for the shield position, which is between the player's two hands and facing
        /// forward, away from the player.
        /// </summary>
        private Pose CalculateDesiredShieldPose()
        {
            _leftHand.GetRootPose(out Pose leftHandPose);
            _rightHand.GetRootPose(out Pose rightHandPose);

            Pose outPose = new Pose();

            Vector3 forward = (rightHandPose.up - leftHandPose.up) / 2f;
            Vector3 up = (leftHandPose.right - rightHandPose.right) / 2f;
            if (forward != Vector3.zero)
            {
                outPose.rotation = Quaternion.LookRotation(forward, up);
            }

            outPose.position = (leftHandPose.position + rightHandPose.position) / 2f;

            return outPose;
        }
    }
}
