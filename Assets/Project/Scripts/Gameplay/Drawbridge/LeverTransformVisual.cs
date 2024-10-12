// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adds auto complete methods for the levers on the bridge and platforms
    /// </summary>
    public class LeverTransformVisual : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("_endPos")]
        private Transform _startPos;
        [SerializeField, FormerlySerializedAs("_autoCompletePos")]
        private Transform _endPos;
        [SerializeField]
        private OneGrabRotateTransformer _rotateTransformer;
        [SerializeField]
        private GameObject _interactables;
        [SerializeField]
        private UnityEvent _onReset;

        private InteractionTracker _tracker;

        private void Start()
        {
            _tracker = new InteractionTracker(_interactables.GetComponentInParent<IPointableElement>());
            _tracker.WhenSelectRemoved += HandleAutoComplete;
        }

        private void HandleAutoComplete(IInteractorView obj)
        {
            if (_tracker.SelectingInteractors.Count > 0) return;

            var angle = _rotateTransformer.GetRelativeAngle().y;
            SetComplete(angle > 60);
        }

        public void ResetPosition()
        {
            SetComplete(false);
            _onReset.Invoke();
        }

        public void AutoComplete() => SetComplete(true);

        private void SetComplete(bool complete)
        {
            var targetPose = complete ? _endPos : _startPos;
            TweenRunner.TweenTransform(transform, targetPose, 0.3f)
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
    }
}
