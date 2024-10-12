// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public static class ColliderExtension
    {
        public static bool IsPointWithinCollider(this Collider collider, Vector3 point)
        {
            if (!collider.bounds.Contains(point))
            {
                return false;
            }

            Vector3 closestPoint = collider.ClosestPoint(point);
            if (collider is MeshCollider)
            {
                return (closestPoint - point).sqrMagnitude < collider.contactOffset * collider.contactOffset;
            }
            return closestPoint.Equals(point);
        }
    }
}
