// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Raycasts object against layer when follow transform is updated
    /// </summary>
    [RequireComponent(typeof(FollowTransform))]
    public class RaycastSurfaceConstraint : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _direction = Vector3.down;
        [SerializeField]
        private float _rayDistance = 20f;
        [SerializeField]
        private LayerMask _layer;

        private FollowTransform _followTransform;

        private void OnEnable()
        {
            _followTransform = GetComponent<FollowTransform>();
            _followTransform.WhenTransformUpdated += ConstrainToRaycastSurface;
        }

        private void OnDisable()
        {
            _followTransform.WhenTransformUpdated -= ConstrainToRaycastSurface;
        }

        private void ConstrainToRaycastSurface()
        {
            if (Physics.Raycast(transform.position, _direction, out var hit, _rayDistance, _layer, QueryTriggerInteraction.Ignore))
            {
                transform.position = hit.point;
                transform.rotation = Quaternion.LookRotation(-hit.normal);
            }
        }
    }
}
