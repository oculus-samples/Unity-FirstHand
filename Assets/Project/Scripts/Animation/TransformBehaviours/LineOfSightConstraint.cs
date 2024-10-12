// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Fixes when a follow transform moves something out of sight
    /// </summary>
    public class LineOfSightConstraint : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _transforms = new List<Transform>();
        [SerializeField]
        private LayerMask _layerMask;
        [SerializeField, Optional]
        private Transform _overrideTarget;

        private FollowTransform _followTransform;
        private static RaycastHit[] _hits = new RaycastHit[12];

        private void Reset()
        {
            _transforms.Clear();
            _transforms.Add(transform);
        }

        private void OnEnable()
        {
            _followTransform = GetComponent<FollowTransform>();
            _followTransform.WhenTransformUpdated += EnsureLineOfSight;
        }

        private void OnDisable()
        {
            _followTransform.WhenTransformUpdated -= EnsureLineOfSight;
        }

        private void EnsureLineOfSight()
        {
            var start = _overrideTarget ? _overrideTarget.position : _followTransform.Source.position;

            var sumOffsets = Vector3.zero;
            var offsetCount = 0;

            for (int i = 0; i < _transforms.Count; i++)
            {
                var end = _transforms[i].position;
                int hitCount = LinecastNonAlloc(start, end, _hits, _layerMask, QueryTriggerInteraction.Ignore);

                for (int j = 0; j < hitCount; j++)
                {
                    if (_hits[j].rigidbody != null) continue; // assume colliders without rigidbodies are static geometry

                    sumOffsets += _hits[j].point - end;
                    offsetCount++;

                    break;
                }
            }

            if (offsetCount == 0) return;

            var averageOffset = sumOffsets / offsetCount;

            transform.position += averageOffset;
        }

        static int LinecastNonAlloc(Vector3 start, Vector3 end, RaycastHit[] hits, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            var direction = end - start;
            var distance = direction.magnitude;
            return Physics.RaycastNonAlloc(new Ray(start, direction), hits, distance, layerMask, queryTriggerInteraction);
        }
    }
}
