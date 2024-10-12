// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays haptic based on physics collisions
    /// </summary>
    public class PhysicsHapticTrigger : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [Tooltip("The shortest amount of time in seconds between collisions. Used to cull multiple fast collision events.")]
        [Range(0.0f, 2.0f)]
        [SerializeField] private float _timeBetweenCollisions = 0.2f;
        [SerializeField] ImpactHaptic _impactHapticEvents;
        [Tooltip("Collisions below this value will play a soft haptic event, and collisions above will play a hard haptic event.")]
        [Range(0.0f, 8.0f)]
        [SerializeField]
        private float _velocitySplit = 1.0f;
        [Tooltip("Collisions below this value will be ignored and will not play haptic.")]
        [Range(0.0f, 2.0f)]
        [SerializeField] private float _minimumVelocity = 0;
        private HapticClipPlayer _left;
        private HapticClipPlayer _right;
        [Range(1, 255)]
        [SerializeField] private int _upperAmplitude = 5;
        [Range(0, 254)]
        [SerializeField] private int _lowerAmplitude = 1;
        [SerializeField]
        ReferenceActiveState _canPlay = ReferenceActiveState.Optional();

        private float _timeAtLastCollision = 0f;

        protected bool _started = false;
        private CollisionEvents _collisionEvents;

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(_impactHapticEvents.SoftHapticResponse, nameof(_impactHapticEvents.SoftHapticResponse));
            this.AssertField(_impactHapticEvents.HardHapticResponse, nameof(_impactHapticEvents.HardHapticResponse));
            this.AssertField(_rigidbody, nameof(_rigidbody));
            _collisionEvents = _rigidbody.gameObject.AddComponent<CollisionEvents>();
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                _collisionEvents.WhenCollisionEnter += HandleCollisionEnter;
            }
        }

        private void HandleCollisionEnter(Collision collision) => TryPlayHaptic(collision);

        private void TryPlayHaptic(Collision collision)
        {
            if (collision.collider.gameObject == null) return;

            float deltaTime = Time.time - _timeAtLastCollision;
            if (_timeBetweenCollisions > deltaTime) return;

            if (_canPlay.HasReference && !_canPlay) return;

            _timeAtLastCollision = Time.time;

            float collisionMagnitude = collision.relativeVelocity.sqrMagnitude;
            PlayHapticFeedback(_impactHapticEvents, collisionMagnitude);
        }

        private void PlayHapticFeedback(ImpactHaptic impactHaptic, float magnitude)
        {
            if (magnitude <= _minimumVelocity) return;

            bool useHardResponse = magnitude > _velocitySplit && impactHaptic.HardHapticResponse != null;
            var clip = useHardResponse ? impactHaptic.HardHapticResponse : impactHaptic.SoftHapticResponse;

            if (clip == null) return;

            _left = new HapticClipPlayer(clip);
            _right = new HapticClipPlayer(clip);

            var amplitude = useHardResponse ? _upperAmplitude : _lowerAmplitude;
            _left.amplitude = _right.amplitude = amplitude;

            _left.Play(Controller.Left);
            _right.Play(Controller.Right);
        }

        public class CollisionEvents : MonoBehaviour
        {
            public event Action<Collision> WhenCollisionEnter = delegate { };
            private void OnCollisionEnter(Collision collision)
            {
                WhenCollisionEnter.Invoke(collision);
            }
        }

        [Serializable]
        public struct ImpactHaptic
        {
            [SerializeField] private HapticClip _hardHapticResponse;
            [SerializeField] private HapticClip _softHapticResponse;
            public HapticClip HardHapticResponse => _hardHapticResponse;
            public HapticClip SoftHapticResponse => _softHapticResponse;
        }
    }
}
