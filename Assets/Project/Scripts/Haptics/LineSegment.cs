// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public struct LineSegment
    {
        Vector3 _start, _end;

        public LineSegment(Vector3 start, Vector3 end)
        {
            _start = start;
            _end = end;
        }

        public float GetDistanceToRay(Ray ray)
        {
            var closestPointOnRay = GetClosestPointOnRay(ray);
            var closestPointOnLine = GetClosestPointOnLine(closestPointOnRay);
            return (closestPointOnLine - closestPointOnRay).magnitude;
        }

        public Vector3 GetClosestPointOnLine(Vector3 point)
        {
            var direction = _end - _start;
            var angleCompare = point - _start;
            float magnitudeMax = direction.magnitude;
            direction.Normalize();
            float dotP = Vector3.Dot(angleCompare, direction);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
            return _start + direction * dotP;
        }

        public Vector3 GetClosestPointOnLineUnbounded(Vector3 point)
        {
            var direction = _end - _start;
            var angleCompare = point - _start;
            direction.Normalize();
            float dotP = Vector3.Dot(angleCompare, direction);
            return _start + direction * dotP;
        }

        public float GetDistanceToPoint(Vector3 point)
        {
            var pointOnLine = GetClosestPointOnLine(point);
            return (point - pointOnLine).magnitude;
        }

        public Vector3 GetClosestPointOnRay(Ray ray)
        {
            Raycast(ray, out var distance);
            return ray.GetPoint(distance);
        }

        public float InverseLerp(Vector3 pointOnLine)
        {
            Vector3 AB = _end - _start;
            Vector3 AV = pointOnLine - _start;
            return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
        }

        public bool IsDivergent(Ray ray)
        {
            return !Raycast(ray, out _);
        }

        private readonly bool Raycast(in Ray ray, out float distance)
        {
            var direction = (_end - _start);
            var up = Vector3.Cross(ray.direction, direction);
            var plane = new Plane(_start, _end, (_start + up));
            return plane.Raycast(ray, out distance);
        }
    }
}
