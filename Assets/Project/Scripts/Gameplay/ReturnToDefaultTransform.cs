// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class ReturnToDefaultTransform : MonoBehaviour
    {
        [SerializeField]
        private Transform _objectToMove;

        [SerializeField] private float _maxDistance = 0.5f;

        [SerializeField]
        ReferenceActiveState _canTidy = ReferenceActiveState.Optional();

        private Pose _tidyPose;

        private void Start()
        {
            _tidyPose = _objectToMove.GetPose();
        }

        private void Update()
        {
            bool shouldTidy = ShouldTidy();
            bool isTidying = TweenRunner.IsTweening(this);
            if (shouldTidy == isTidying) return;

            if (shouldTidy) TweenRunner.DelayedCall(4f, Tidy).SetID(this);
            else TweenRunner.Kill(this);
        }

        private void Tidy()
        {
            _objectToMove.SetPose(_tidyPose);
            if (_objectToMove.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = rb.angularVelocity = Vector3.zero;
            }
            if (!IsTidy()) enabled = false;
        }

        bool ShouldTidy()
        {
            if (IsTidy()) return false;
            if (IsInViewActiveState.IsInFOV(_tidyPose.position)) return false;
            if (IsInViewActiveState.IsInFOV(_objectToMove.position)) return false;
            if (!_canTidy) return false;
            return true;
        }

        private bool IsTidy() => (_tidyPose.position - _objectToMove.position).IsMagnitudeLessThan(_maxDistance);
    }
}
