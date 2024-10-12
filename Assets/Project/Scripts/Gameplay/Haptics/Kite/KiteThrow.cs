// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class KiteThrow : MonoBehaviour, ITransformer, IActiveState
    {
        [SerializeField] public float _minDistance;
        [SerializeField] public float _maxDistance;
        [SerializeField] public GameObject _holoThrower;
        [SerializeField] public ProgressTracker _progressTracker;
        [SerializeField] public int _progressAfterThrow;
        [SerializeField] public HapticClip _hapticClip;
        [SerializeField] float _amplitudeBoost = 1;
        [SerializeField] LineRenderer _leftLineRenderer;
        [SerializeField] LineRenderer _rightLineRenderer;
        [SerializeField] UVAnimation _leftLineAnimation;
        [SerializeField] UVAnimation _rightLineAnimation;
        [SerializeField] ReferenceActiveState _canThrow;
        [SerializeField] public HapticClip _throwHapticClip;

        private InteractionTracker _interactionTracker;
        private Vector3 _initialPos;
        private Quaternion _initialRotate;
        private HapticClipPlayer _player;
        private IGrabbable _grabbable;
        private Pose _grabDeltaInLocalSpace;
        private Color _lineColor;

        [SerializeField]
        private Transform _lookTarget;

        public bool Active => Vector3.Distance(_initialPos, _holoThrower.transform.position) > _minDistance && _canThrow;

        private void Awake()
        {
            _initialPos = _holoThrower.transform.position;
            _initialRotate = _holoThrower.transform.rotation;
            _grabbable = GetComponent<IGrabbable>();
            _interactionTracker = new InteractionTracker(_grabbable as Grabbable);
            _lineColor = _leftLineRenderer.material.color;
        }

        void Update()
        {
            float currentDistance = Vector3.Distance(_initialPos, _holoThrower.transform.position);
            float norm = Mathf.Clamp01(currentDistance / _minDistance);

            if (_player != null)
            {
                _player.amplitude = Mathf.Lerp(0.1f, 1, norm) * _amplitudeBoost;
                _player.frequencyShift = norm;
            }

            var color = _lineColor;
            color.a = Mathf.Lerp(0.5f, 1, norm);
            _leftLineRenderer.material.color = color;
            _rightLineRenderer.material.color = color;

            _leftLineAnimation.TimeMultiplier = _rightLineAnimation.TimeMultiplier = Mathf.Lerp(0.1f, 1, norm);
        }

        private void OnEnable()
        {
            _interactionTracker.WhenSelectAdded += HandleGrab;
            _interactionTracker.WhenSelectRemoved += HandleRelease;
        }

        void HandleGrab(IInteractorView _)
        {
            if (!_interactionTracker.TryGetHand(out var hand)) return;

            _player = HandHaptics.Get(hand).PlayHaptic(_hapticClip, true);
        }

        void HandleRelease(IInteractorView releaser)
        {
            if (_interactionTracker.IsGrabbed()) return;

            if (_player != null)
            {
                _player.TryStop();
                _player.Dispose();
                _player = null;
            }

            if (Active)
            {
                //throw kite
                Pose pose = new Pose(_holoThrower.transform.position + _holoThrower.transform.forward * 1.5f + Vector3.up * 0.4f, _holoThrower.transform.rotation);
                LerpToPositionAndRotation(0.2f, pose, () => _holoThrower.SetActive(false));

                TweenRunner.DelayedCall(0.1f, () =>
                {
                    _leftLineRenderer.enabled = false;
                    _rightLineRenderer.enabled = false;
                });

                //set progress here to trigger kite throw
                _progressTracker.SetProgress(_progressAfterThrow);

                if (_throwHapticClip != null && _interactionTracker.TryGetHand(releaser, out var hand))
                {
                    HandHaptics.Get(hand).PlayHaptic(_throwHapticClip);
                }
            }
            else
            {
                //kite floats back to location
                LerpToPositionAndRotation(1f, new Pose(_initialPos, _initialRotate));
            }
        }

        void LerpToPositionAndRotation(float duration, Pose pose, Action onComplete = null)
        {
            TweenRunner.TweenTransform(_holoThrower.transform, pose, duration).OnComplete(onComplete);
        }

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
        }

        public void BeginTransform()
        {
            Pose grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;
            _grabDeltaInLocalSpace = new Pose(targetTransform.InverseTransformVector(grabPoint.position - targetTransform.position),
                                            Quaternion.Inverse(grabPoint.rotation) * targetTransform.rotation);
        }

        public void UpdateTransform()
        {
            Pose grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;
            Vector3 newPosition = grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position);

            targetTransform.position = Vector3.MoveTowards(_initialPos, newPosition, _maxDistance);
            targetTransform.rotation = Quaternion.LookRotation(_lookTarget.position - targetTransform.position);
        }

        public void EndTransform()
        {
        }
    }
}
