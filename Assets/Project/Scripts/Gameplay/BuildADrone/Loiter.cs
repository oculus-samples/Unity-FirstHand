// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oculus.Interaction.ComprehensiveSample
{
    // Adds subtle 'zero gravity' movement
    public class Loiter : MonoBehaviour
    {
        [SerializeField]
        ReferenceActiveState _active;

        InteractionTracker _tracker;
        private FollowTransform _follow;

        Pose _lastPose;

        void Awake()
        {
            _tracker = new InteractionTracker(GetComponent<IPointableElement>());
            _tracker.WhenSelectRemoved += UpdateLoiterPosition;

            Transform target = new GameObject().transform;
            target.gameObject.hideFlags = HideFlags.HideAndDontSave;
            SceneManager.MoveGameObjectToScene(target.gameObject, gameObject.scene);

            _follow = gameObject.AddComponent<FollowTransform>();
            _follow.enabled = false;

            _follow.Source = target;

            _follow.positionSettings.enabled = true;
            _follow.positionSettings.PositionSmoothing = 0.2f;
            _follow.positionSettings.PositionMask = (FollowTransform.Vector3Mask)~0;
            _follow.positionSettings.Noise.amplitude = Vector3.one * 0.1f;
            _follow.positionSettings.Noise.frequency = 0.2f;

            _follow.rotationSettings.enabled = true;
            _follow.rotationSettings.RotationSmoothing = 0.2f;
            _follow.rotationSettings.UpDirection = FollowTransform.OffsetSpace.Target;
            _follow.rotationSettings.RotationType = FollowTransform.Rotation.Rotation;
            _follow.rotationSettings.Noise.amplitude = Vector3.one * 20;
            _follow.rotationSettings.Noise.frequency = 0.2f;
        }

        private void UpdateLoiterPosition(IInteractorView _)
        {
            if (_tracker.IsGrabbed()) return;

            UpdateLoiterPosition();
        }

        private void UpdateLoiterPosition()
        {
            _follow.Source.SetPose(transform.GetPose());

            if (_tracker.Subject is MonoBehaviour b && b.TryGetComponent<Rigidbody>(out var rb) && !rb.isKinematic)
            {
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
            }
        }

        private void LateUpdate()
        {
            bool movedExternally = TweenRunner.IsTweening(transform) || transform.GetPose() != _lastPose;
            if (movedExternally)
            {
                UpdateLoiterPosition();
            }

            if (_active && _tracker.SelectingInteractors.Count == 0)
            {
                _follow.UpdatePose(true);
                _lastPose = transform.GetPose();
            }
        }
    }
}
