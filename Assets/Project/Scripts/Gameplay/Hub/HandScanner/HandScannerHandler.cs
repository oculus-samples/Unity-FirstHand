// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class HandScannerHandler : MonoBehaviour
    {
        [SerializeField]
        private Transform _handImage;
        [SerializeField]
        private List<Graphic> _fingerprintCircles = new List<Graphic>();
        [SerializeField]
        private Slider _verificationSlider;

        [Header("Additional")]
        [SerializeField]
        private ProgressTracker _progressTracker;
        [SerializeField]
        private int _scannedProgress = 1;
        [SerializeField]
        private HandScannerTriggerArea _triggerArea;
        [SerializeField]
        private ReferenceActiveState _inProgress;
        [SerializeField]
        private Color _activeColor, _inactiveColor;

        private float _totalVerificationTime = 2f;
        private float _currentVerificationTime;

        bool Active => _triggerArea.Active;

        private void Start()
        {
            _verificationSlider.maxValue = _totalVerificationTime;
        }

        private void Update()
        {
            if (!_inProgress) return;

            if (Active)
            {
                UpdateVerificationSliderProgress();
                UpdateHandImageRotation();
            }

            UpdateFingerprintColors();
        }

        private void UpdateVerificationSliderProgress()
        {
            _currentVerificationTime += Time.deltaTime;
            if (_currentVerificationTime >= _totalVerificationTime)
            {
                _progressTracker.SetProgress(_scannedProgress);
                _currentVerificationTime = _totalVerificationTime;
            }

            if (Mathf.Abs(_verificationSlider.value - _currentVerificationTime) > 0.01f) // so we dont dirty the UI too often
                _verificationSlider.value = _currentVerificationTime;
        }

        private void UpdateHandImageRotation()
        {
            if (_triggerArea.LeftHandInZone)
                _handImage.localScale = new Vector3(-1, 1, 1);

            if (_triggerArea.RightHandInZone)
                _handImage.localScale = new Vector3(1, 1, 1);
        }

        private void UpdateFingerprintColors()
        {
            var color = Active ? _activeColor : _inactiveColor;
            for (int i = 0; i < _fingerprintCircles.Count; i++)
                _fingerprintCircles[i].color = color;
        }
    }
}
