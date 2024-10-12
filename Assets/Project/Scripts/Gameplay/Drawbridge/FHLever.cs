// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adds auto complete methods for the levers on the bridge and platforms
    /// </summary>
    public class FHLever : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("_endPos")]
        private Transform _startPos;
        [SerializeField, FormerlySerializedAs("_autoCompletePos")]
        private Transform _endPos;
        [SerializeField]
        private OneGrabRotateTransformer _rotateTransformer;
        [SerializeField]
        private GameObject _interactables;

        [SerializeField] ReferenceActiveState _interactable = ReferenceActiveState.Optional();
        [SerializeField] ReferenceActiveState _canBeLowered = ReferenceActiveState.Optional();
        [SerializeField] ReferenceActiveState _canBeRaised = ReferenceActiveState.Optional();
        [SerializeField] ReferenceActiveState _canBeMiddle = ReferenceActiveState.Optional();

        private InteractionTracker _tracker;
        public State CurrentState { get; private set; }

        private void Start()
        {
            _tracker = new InteractionTracker(_interactables.GetComponentInParent<IPointableElement>());
            _tracker.WhenSelectRemoved += HandleReleased;

            Pose startPose = GetInverseTransformedBy(_startPos.GetPose(), transform.parent);

            Debug.Log($"{startPose} {_startPos.GetPose(Space.Self)}");

            transform.SetPose(startPose, Space.Self);

            CurrentState = State.Raised;
        }

        private void Update()
        {
            if (!TweenRunner.IsTweening(this))
            {
                _interactables.SetActive(_interactable);
            }

            State newState = State.Middle;

            bool isLowered = ApproxEqual(_endPos) && _canBeLowered;
            if (isLowered) { newState = State.Lowered; }

            bool isRaised = ApproxEqual(_startPos) && _canBeRaised;
            if (isRaised) { newState = State.Raised; }

            if (newState != CurrentState)
            {
                CurrentState = newState;
            }
        }

        private void HandleReleased(IInteractorView _)
        {
            if (_tracker.SelectingInteractors.Count > 0) return;

            var isPastHalfway = _rotateTransformer.GetRelativeAngle().y > 60;

            State state =
                isPastHalfway && _canBeLowered ? State.Lowered :
                !isPastHalfway && _canBeRaised ? State.Raised :
                _canBeMiddle ? State.Middle :
                _canBeRaised ? State.Raised :
                _canBeLowered ? State.Lowered : throw new System.Exception();

            TweenTo(state);
        }

        private void TweenTo(State state)
        {
            _interactables.SetActive(false);

            float t = state == State.Raised ? 0 : state == State.Middle ? 0.5f : 1;
            Pose parentPose = transform.parent.GetPose();
            Pose startPose = GetInverseTransformedBy(_startPos.GetPose(), transform.parent);
            Pose endPose = GetInverseTransformedBy(_endPos.GetPose(), transform.parent);
            PoseUtils.Lerp(ref startPose, endPose, t);

            TweenRunner.TweenTransformLocal(transform, startPose, 0.3f)
                .OnUpdate(x => UpdateTransformer())
                .SetID(this);
        }

        private void UpdateTransformer()
        {
            Transform targetTransform = _rotateTransformer.Grabbable.Transform;
            Transform pivotTransform = _rotateTransformer.Pivot;
            Quaternion relativeOrientation = Quaternion.Inverse(pivotTransform.rotation) * targetTransform.rotation;
            float angle = relativeOrientation.eulerAngles[(int)_rotateTransformer.RotationAxis];
            _rotateTransformer.SetRelativeAngle(angle);
        }

        private bool ApproxEqual(Transform to) => Mathf.Abs((transform.eulerAngles - to.eulerAngles).magnitude) < 2f;

        public static Pose GetInverseTransformedBy(Pose pose, Transform lhs)
        {
            return new Pose
            {
                position = lhs.InverseTransformPoint(pose.position),
                rotation = Quaternion.Inverse(lhs.rotation) * pose.rotation
            };
        }

        [Flags]
        public enum State
        {
            Raised = 1 << 0,
            Middle = 1 << 1,
            Lowered = 1 << 2
        }
    }
}
