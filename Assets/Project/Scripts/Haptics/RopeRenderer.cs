// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Locomotion;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles rope visual
    /// </summary>
    class RopeRenderer : MonoBehaviour
    {
        [SerializeField]
        RopePhysics _ropePhysics;
        [SerializeField]
        int _segments = 40;
        [SerializeField]
        Oculus.Interaction.TubeRenderer _ropeRenderer;

        TubeRendererInput _tubeRendererInput;

        private void Awake()
        {
            _tubeRendererInput = new TubeRendererInput(_segments);
            _ropePhysics.WhenUpdated += UpdateMesh;
        }

        private void UpdateMesh()
        {
            var points = _ropePhysics.Points;
            var pointCount = points.Count;
            var segsPerPoint = _segments / (pointCount - 1);

            int tubeIndex = 0;
            float segsUsed = 0;
            for (int i = 0; i < pointCount - 1; i++)
            {
                var a = points[i].Position;
                var b = points[i + 1].Position;

                var before = i > 0 ? points[i - 1].Position : a - (b - a);
                var after = i < pointCount - 2 ? points[i + 2].Position : b + (b - a);

                var spline = new CatmullRomCurve(before, a, b, after);

                var segs = i < pointCount - 2 ? segsPerPoint : (_segments - (pointCount - 2) * segsPerPoint) - 1;
                for (int j = 0; j < segs; j++)
                {
                    var t = j / (float)segs;
                    var p = spline.GetPoint(t);
                    _tubeRendererInput[tubeIndex++] = p;
                }
                segsUsed += segs;
            }

            _tubeRendererInput[_segments - 1] = points[pointCount - 1].Position;
            _tubeRendererInput.Apply(_ropeRenderer);
        }

        public struct CatmullRomCurve
        {
            public Vector3 p0, p1, p2, p3;

            public CatmullRomCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
            {
                this.p0 = p0;
                this.p1 = p1;
                this.p2 = p2;
                this.p3 = p3;
            }

            public Vector3 GetPoint(float t)
            {
                const float k0 = 0;
                float k1 = GetKnotInterval(p0, p1);
                float k2 = GetKnotInterval(p1, p2) + k1;
                float k3 = GetKnotInterval(p2, p3) + k2;

                float u = Mathf.LerpUnclamped(k1, k2, t);
                var A1 = Remap(k0, k1, p0, p1, u);
                var A2 = Remap(k1, k2, p1, p2, u);
                var A3 = Remap(k2, k3, p2, p3, u);
                var B1 = Remap(k0, k2, A1, A2, u);
                var B2 = Remap(k1, k3, A2, A3, u);
                return Remap(k1, k2, B1, B2, u);
            }

            static Vector3 Remap(float a, float b, Vector3 c, Vector3 d, float u)
            {
                return Vector3.LerpUnclamped(c, d, (u - a) / (b - a));
            }

            float GetKnotInterval(Vector3 a, Vector3 b)
            {
                return Mathf.Pow(Vector3.SqrMagnitude(a - b), 0.25f);
            }
        }
    }
}
