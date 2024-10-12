// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true if the camera is in view of the object this class is attached to
    /// </summary>
    public class IsInViewActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField, Tooltip("The radius of the object, a large object is more likely to be in view")]
        private float _radius = 0f;

        [SerializeField, Tooltip("The FOV of the player, smaller numbers will require a more direct gaze, -1 uses the camera's fov")]
        private float _overrideFov = -1f;

        [SerializeField, Tooltip("The minimum distance to consider in view, objects that are closer to the player are not in view")]
        private float _minDistance = -1f;

        [SerializeField, Tooltip("The max distance to that player, if the object is further than this its not in view")]
        private float _maxDistance = -1f;

        // TODO is occluded
        // TODO repurpose subtitle code

        private static Camera _mainCamera;

        public bool Active => IsInFOV();

        private bool IsInFOV() => IsInFOV(transform.position, _radius, _maxDistance, _minDistance, _overrideFov);

        public static bool IsInFOV(Vector3 position, float radius = 0, float maxDistance = -1, float minDistance = -1, float fov = -1)
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return false;
            }

            var toThis = position - _mainCamera.transform.position;
            var distance = toThis.magnitude;
            if (maxDistance > 0 && distance - radius > maxDistance) return false;
            if (minDistance > 0 && distance < minDistance) return false;

            var radiusToAngle = Mathf.Atan2((float)radius, distance) * Mathf.Rad2Deg;
            var apature = radiusToAngle + (fov > 0 ? fov : _mainCamera.fieldOfView) * 0.5f;

            return Vector3.Angle(_mainCamera.transform.forward, toThis) < apature;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsInFOV() ? Color.green : Color.white;
            Gizmos.DrawWireSphere(transform.position, _radius);

            if (_mainCamera)
            {
                Gizmos.matrix = Matrix4x4.TRS(_mainCamera.transform.position, _mainCamera.transform.rotation, Vector3.one);
                Gizmos.DrawFrustum(Vector3.zero, _overrideFov > 0 ? _overrideFov : _mainCamera.fieldOfView, _maxDistance > 0 ? _maxDistance : 100, _minDistance > 0 ? _minDistance : 0, 1);
            }
        }
    }
}
