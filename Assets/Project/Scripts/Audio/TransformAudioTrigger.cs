// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays audio played on the distance rotation
    /// </summary>
    public class TransformAudioTrigger : MonoBehaviour
    {
        [SerializeField]
        private AudioTrigger _audioTrigger;
        [SerializeField]
        private Axis _axis = Axis.X;
        [SerializeField]
        private int _segements = 0;
        [SerializeField]
        private bool _rotationTrigger, _positionTrigger, _shouldOffsetPitch;

        private Vector3 _previousRotation, _previousPosition;
        private const float _pitchOffset = 0.1f;

        private void Start()
        {
            _previousRotation = transform.localEulerAngles;
            _previousPosition = transform.localPosition;
        }

        private void Update()
        {
            if (_rotationTrigger)
                PlayAudio(transform.localEulerAngles, ref _previousRotation);
            if (_positionTrigger)
                PlayAudio(transform.localPosition, ref _previousPosition);
        }

        private void PlayAudio(Vector3 current, ref Vector3 previous)
        {
            float rotationValue = TransformAxis(current);
            float previousValue = TransformAxis(previous);
            float distanceMoved = Mathf.Abs(rotationValue - previousValue);

            if (distanceMoved >= _segements)
            {
                IncrementPitch();
                _audioTrigger.Play();
                previous = current;
            }
        }

        private void IncrementPitch()
        {
            if (_audioTrigger.Pitch >= 3 || !_shouldOffsetPitch) return;
            _audioTrigger.Pitch += _pitchOffset;
        }

        private float TransformAxis(Vector3 current)
        {
            switch (_axis)
            {
                case Axis.X:
                    return current.x;

                case Axis.Y:
                    return current.y;

                case Axis.Z:
                    return current.z;
            }

            return 0;
        }

        public enum Axis
        {
            X, Y, Z
        }
    }
}
