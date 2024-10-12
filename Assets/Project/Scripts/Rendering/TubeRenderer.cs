// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
    /// <summary>
    /// A tube renderer with the same API as LineRenderer
    /// </summary>
    public class TubeRenderer : Interaction.TubeRenderer
    {
    }

    public class TubeRendererInput
    {
        private TubePoint[] _points;

        public Vector3 this[int i]
        {
            get => _points[i].position;
            set => _points[i].position = value;
        }

        public TubeRendererInput(int count)
        {
            _points = new TubePoint[count];
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i].rotation = Quaternion.identity;
            }
        }

        public void Apply(Interaction.TubeRenderer tubeRenderer)
        {
            var pointCount = _points.Length;
            var last = pointCount - 1;

            var rot = tubeRenderer.transform.rotation * _points[0].rotation;
            _points[0].rotation = Look(Delta(_points[0], _points[1]), rot * Vector3.up);
            _points[0].relativeLength = 0;

            float totalLength = 0;
            for (int i = 1; i <= last; i++)
            {
                var pointBefore = _points[i - 1];
                var point = _points[i];
                var direction = Delta(pointBefore, point);

                _points[i].rotation = Look(direction, pointBefore.rotation * Vector3.up);
                totalLength = _points[i].relativeLength = totalLength + direction.magnitude;
            }

            totalLength = Mathf.Max(totalLength, 0.0001f);

            for (int i = 0; i < pointCount; i++)
            {
                float n = _points[i].relativeLength / totalLength;
                _points[i].relativeLength = n;
                _points[i].position = tubeRenderer.transform.InverseTransformPoint(_points[i].position);
                _points[i].rotation = Quaternion.Inverse(tubeRenderer.transform.rotation) * _points[i].rotation;
            }

            tubeRenderer.RenderTube(_points);

            Quaternion Look(Vector3 vec, Vector3 up)
            {
                if (vec.sqrMagnitude < 0.0000001f) return Quaternion.identity;
                return Quaternion.LookRotation(vec, up);
            }

            Vector3 Delta(TubePoint a, TubePoint b) => b.position - a.position;
        }
    }
}
