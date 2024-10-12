// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Sets a LinerRenderers positions to match a set of ordered transforms
    /// Used to render the line from the players hand to the raycast target during the turret sequence
    /// </summary>
    [RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
    public class LineBetweenTransforms : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _transforms;

        [SerializeField]
        private bool _smooth;

        private LineRenderer _lineRenderer;
        private static Vector3[] _positions = new Vector3[20];

        public List<Transform> Transforms => _transforms;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        void Update()
        {
            if (!_smooth)
            {
                if (_lineRenderer.positionCount != _transforms.Count)
                {
                    _lineRenderer.positionCount = _transforms.Count;
                }

                for (int i = 0; i < _transforms.Count; i++)
                {
                    _lineRenderer.SetPosition(i, _transforms[i].position);
                }
            }
            else
            {
                if (_lineRenderer.positionCount != _positions.Length)
                {
                    _lineRenderer.positionCount = _positions.Length;
                }

                Transform start = _transforms[0];
                Transform end = _transforms[_transforms.Count - 1];
                GetBezierPositions(start, end, _positions);

                for (int i = 0; i < _positions.Length; i++)
                {
                    _lineRenderer.SetPosition(i, _positions[i]);
                }
            }
        }

        public static void GetBezierPositions(Transform start, Transform end, Vector3[] positions)
        {
            var line = end.position - start.position;
            var midPointLin = start.position + line * 0.5f;
            var plane = new Plane(line, midPointLin);
            var bendDirection = Vector3.Lerp(line, start.forward, Vector3.Dot(start.forward, line.normalized));
            plane.Raycast(new Ray(start.position, bendDirection), out var midBezDist);
            var midBez = start.position + bendDirection * midBezDist;
            Debug.DrawLine(midBez, midBez + Vector3.up * 0.1f);

            Vector3 p0 = start.position;
            Vector3 p1 = midBez;
            Vector3 p2 = end.position;
            for (int i = 0; i < positions.Length; i++)
            {
                var t = i / (positions.Length - 1.0f);
                positions[i] = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
            }
        }
    }
}
