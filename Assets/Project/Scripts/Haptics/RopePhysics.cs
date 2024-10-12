// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Adds physics to rope
    /// </summary>
    // based on http://www.mclimatiano.com/unity-rope-simulation-using-distance-constraints/
    public class RopePhysics : MonoBehaviour
    {
        [SerializeField]
        float _startLength = 0.3f;
        [SerializeField, FormerlySerializedAs("_pointsPerMeter")]
        float _segmentsPerMeter = 8f;
        [SerializeField]
        private float _stiffness = 1;
        [SerializeField]
        private float _gravity = 9f;
        [SerializeField]
        private int _relaxationIterations = 8;
        [SerializeField]
        private float _mass = 10;
        [SerializeField]
        private float _maxLength = 1.2f;

        public readonly List<RopePoint> Points = new List<RopePoint>();

        public float Length { get; private set; }

        public event Action WhenUpdated;

        private void Awake()
        {
            Points.Add(new RopePoint(transform.position, _mass));
            Points[0].Constraints.Add(new FixedConstraint(Points[0]));

            int pointCount = (int)(_segmentsPerMeter * _startLength) + 1;
            float segLength = 1 / _segmentsPerMeter;

            for (int i = 0; i < pointCount; i++)
            {
                var point = new RopePoint(transform.position - Vector3.up * segLength + Random.onUnitSphere * 0.001f, _mass);
                var distanceConstraint = new DistanceConstraint(point, Points[Points.Count - 1], segLength, _stiffness);
                point.Constraints.Add(distanceConstraint);

                Points.Add(point);
            }

            for (int i = 0; i < 5; i++)
            {
                UpdatePhysics();
            }
        }

        private void FixedUpdate()
        {
            UpdatePhysics();

            Length = 0;
            for (int i = 1; i < Points.Count; i++)
            {
                Length += Vector3.Distance(Points[i].Position, Points[i - 1].Position);
            }

            if (Points.Count < _maxLength * _segmentsPerMeter)
            {
                float segLength = 1 / _segmentsPerMeter;
                var currentLength = (Points.Count - 1) * segLength;
                if (Length > currentLength + segLength)
                {
                    SetLength(currentLength + segLength);
                }
            }

            WhenUpdated?.Invoke();
        }

        private void UpdatePhysics()
        {
            (Points[0].Constraints[0] as FixedConstraint).Position = transform.position;

            var gravity = Vector3.down * _gravity;
            Points.ForEach(p => p.ApplyForce(gravity * p.Mass));

            for (int iteration = 0; iteration < _relaxationIterations; iteration++)
            {
                Points.ForEach(p => p.SolveConstraints());
            }
            Points[0].SolveConstraints();

            Points.ForEach(p => p.Update(Time.fixedDeltaTime));
        }

        void SetLength(float length)
        {
            float segLength = 1 / _segmentsPerMeter;
            var currentLength = (Points.Count - 1) * segLength;

            var A = Points[0];
            if (currentLength < length)
            {
                while (currentLength < length)
                {
                    var C = Points[1];
                    var B = new RopePoint(Vector3.Lerp(A.Position, C.Position, 0.1f), _mass);
                    (C.Constraints[0] as DistanceConstraint).B = B;
                    B.Constraints.Add(new DistanceConstraint(B, A, segLength, _stiffness));
                    Points.Insert(1, B);
                    currentLength += segLength;
                }
            }
            else
            {
                while (currentLength > length)
                {
                    Points.RemoveAt(1);
                    (Points[1].Constraints[0] as DistanceConstraint).B = A;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (Points != null)
            {
                Points.ForEach(x => Gizmos.DrawWireSphere(x.Position, 0.03f));
            }
        }

        [System.Serializable]
        public class RopePoint
        {
            public Vector3 Position { get; set; }
            public Vector3 PrevPosition { get; set; }
            public float Mass { get; set; }

            public readonly List<Constraint> Constraints = new List<Constraint>();

            public RopePoint(Vector3 position, float mass)
            {
                Position = PrevPosition = position;
                Mass = mass;
            }

            public Vector3 Force { get; private set; }

            public void ApplyForce(Vector3 value)
            {
                Force += value;
            }

            public void SolveConstraints()
            {
                Constraints.ForEach(c => c.Solve());
            }

            public void Update(float deltaTime, float dampeningFactor = 0.9f)
            {
                if (Mathf.Approximately(Mass, 0.0f)) return;

                // Get acceleration from froce and reset force
                float invMass = 1.0f / Mass;
                Vector3 accel = Force * invMass;
                Vector3 vel = (Position - PrevPosition) * dampeningFactor;
                Force = Vector3.zero;

                // verlet integration
                Vector3 next = Position + vel + accel * deltaTime * deltaTime;

                // Update positions
                PrevPosition = Position;
                Position = next;
            }

            public bool TryGetConstraint<T>(out T result) where T : Constraint
            {
                for (int i = 0; i < Constraints.Count; i++)
                {
                    if (Constraints[i] is T typedConstraint)
                    {
                        result = typedConstraint;
                        return true;
                    }
                }
                result = default;
                return false;
            }
        }

        public interface Constraint
        {
            public abstract void Solve();
        }

        public class DistanceConstraint : Constraint
        {
            public DistanceConstraint(RopePoint a, RopePoint b, float distance, float stiffness)
            {
                A = a;
                B = b;
                Distance = distance;
                Stiffness = stiffness;
            }

            public float Distance { get; set; }
            public float Stiffness { get; set; }
            public RopePoint A { get; set; }
            public RopePoint B { get; set; }

            public void Solve()
            {
                var toB = A.Position - B.Position;
                var dist = Mathf.Max(toB.magnitude, 0.0001f);
                var dir = toB / dist;
                float distFactor = (Distance - dist) / dist;

                float m1 = 1.0f / A.Mass;
                float m2 = 1.0f / B.Mass;

                float stiffnessFactorA = (m1 / (m1 + m2)) * Stiffness;
                float stiffnessFactorB = Stiffness - stiffnessFactorA;

                A.Position += dir * dist * stiffnessFactorA * distFactor;
                B.Position -= dir * dist * stiffnessFactorB * distFactor;
            }
        }

        public class FixedConstraint : Constraint
        {
            public FixedConstraint(RopePoint point)
            {
                Point = point;
            }

            public Vector3 Position { get; set; }
            public RopePoint Point { get; set; }

            public void Solve()
            {
                Point.Position = Position;
            }
        }

        public class StraightenConstraint : Constraint
        {
            private RopePoint _current;
            public float MaxAngle = 90;

            public RopePoint Previous;
            public RopePoint Next;

            public Vector3 PrevPosition;
            public Vector3 NextPosition;

            public StraightenConstraint(RopePoint previous, RopePoint current, RopePoint next, float maxAngle) : this(current, maxAngle)
            {
                Previous = previous;
                Next = next;
            }

            public StraightenConstraint(RopePoint current, float maxAngle)
            {
                _current = current;
                MaxAngle = maxAngle;
            }

            public void Solve()
            {
                var prev = Previous != null ? Previous.Position : PrevPosition;
                var next = Next != null ? Next.Position : NextPosition;
                var line = new LineSegment(prev, next);
                var straightPos = line.GetClosestPointOnLine(_current.Position);

                _current.Position = Vector3.LerpUnclamped(_current.Position, straightPos, MaxAngle);
            }
        }
    }
}
