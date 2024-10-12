// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Updates a one grab rotate transformer when the object is moved externally to the transformer, eg. via animation
    /// This component works for rotations in +/-180 degrees range only
    /// </summary>
    public class OneGrabRotateTransformerAnimationUpdater : ActiveStateObserver
    {
        [SerializeField] private OneGrabRotateTransformer _transformer;

        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(_transformer);
        }

        protected override void HandleActiveStateChanged() { }

        protected override void Update()
        {
            base.Update();
            if (!Active)
            {
                UpdateTransformer();
            }
        }

        private void UpdateTransformer()
        {
            Transform targetTransform = _transformer.transform;
            Transform pivotTransform = _transformer.Pivot;
            Quaternion relativeOrientation = Quaternion.Inverse(pivotTransform.rotation) * targetTransform.rotation;
            float angle = relativeOrientation.eulerAngles[(int)_transformer.RotationAxis];
            _transformer.SetRelativeAngle(angle);
        }
    }
}
