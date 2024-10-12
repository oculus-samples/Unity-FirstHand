// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Rotates a UI element based on specified values
    /// </summary>
    public class UIRotationTweener : MonoBehaviour
    {
        [SerializeField]
        private RotationAxis _rotationAxis = RotationAxis.Y;
        [SerializeField]
        private float _rotationAmount;
        [SerializeField]
        private float _duration;

        private void OnEnable()
        {
            UpdateRotationAxis();
        }

        private void UpdateRotationAxis()
        {
            switch (_rotationAxis)
            {
                case RotationAxis.X:
                    UpdateTween(Vector3.right);
                    break;

                case RotationAxis.Y:
                    UpdateTween(Vector3.up);
                    break;

                case RotationAxis.Z:
                    UpdateTween(Vector3.forward);
                    break;
            }
        }

        private void UpdateTween(Vector3 direction)
        {
            TweenRunner.TweenEulerAnglesLocal(transform, transform.localEulerAngles + _rotationAmount * direction, _duration).SetLoops(-1);
        }

        public enum RotationAxis
        {
            X, Y, Z
        }
    }
}
