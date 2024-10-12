// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Play audio trigger based on the speed of transform
    /// </summary>
    public class TransformAudioTriggerModulation : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Transform _space;
        [SerializeField]
        private AudioTrigger _audioTrigger;
        [SerializeField]
        private FloatRange _speed = new FloatRange(0, 5);
        [SerializeField]
        private AnimationCurve _speedCurve = AnimationCurve.EaseInOut(0, 1, 1, 2);
        [SerializeField]
        private float _minSpeed = -1;

        private Vector3 _velocity;
        private float _acceleration;
        private Vector3 _lastPosition;
        private float _basePitch = 1;


        private void Reset()
        {
            _target = transform;
            _space = transform.parent;
            _audioTrigger = GetComponent<AudioTrigger>();
        }

        private void Awake()
        {
            _target = _target ? _target : transform;
            _space = _space ? _space : transform.parent;
            _basePitch = _audioTrigger.Pitch;
        }

        private void Update()
        {
            if (!_target) return;

            UpdateMotion();
            UpdateAudioTrigger();
        }

        private void UpdateAudioTrigger()
        {
            var speed = _velocity.magnitude;
            UpdatePitch(speed);
            UpdatePlaying(speed);
        }

        private void UpdatePitch(float speed)
        {
            var speedCurveInput = _speed.InverseLerp(speed);
            var speedMultiplier = _speedCurve.Evaluate(speedCurveInput);

            _audioTrigger.Pitch = _basePitch * speedMultiplier;
        }

        private void UpdatePlaying(float speed)
        {
            if (_minSpeed < 0) return;

            var shouldPlay = speed > _minSpeed;
            if (_audioTrigger.IsPlaying() == shouldPlay) return;

            if (shouldPlay) _audioTrigger.Play();
            else _audioTrigger.Stop();
        }

        private void UpdateMotion()
        {
            var position = _space ? _space.InverseTransformPoint(_target.position) : _target.position;
            var deltaPosition = position - _lastPosition;
            _lastPosition = position;

            if (PauseHandler.IsTimeStopped) return;

            var velocity = deltaPosition / Time.deltaTime;
            var deltaVelocity = velocity - _velocity;
            _velocity = velocity;
            _acceleration = Vector3.Dot(deltaVelocity, _velocity.normalized) / Time.deltaTime;
        }
    }
}
