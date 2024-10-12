// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the generators UI status
    /// </summary>
    public class GeneratorStatusUIManager : MonoBehaviour
    {
        [SerializeField] GameObject _statusImageDefault;
        [SerializeField] GameObject _statusLeftOnRightOff;
        [SerializeField] GameObject _statusLeftOffRightOn;
        [SerializeField] GameObject _statusBothOn;
        [SerializeField] GameObject _statusFailedButtonPress;

        [SerializeField] ReferenceActiveState _isLeftPowerOn;
        [SerializeField] ReferenceActiveState _isRightPowerOn;

        private void Awake() => UpdateActiveImage();
        private void Update() => UpdateActiveImage();

        public void UpdateActiveImage()
        {
            _statusBothOn.SetActive(_isLeftPowerOn && _isRightPowerOn);
            _statusLeftOnRightOff.SetActive(_isLeftPowerOn && !_isRightPowerOn);
            _statusLeftOffRightOn.SetActive(!_isLeftPowerOn && _isRightPowerOn);
            _statusImageDefault.SetActive(!_isLeftPowerOn && !_isRightPowerOn);
            _statusFailedButtonPress.SetActive(false);
        }
    }
}
