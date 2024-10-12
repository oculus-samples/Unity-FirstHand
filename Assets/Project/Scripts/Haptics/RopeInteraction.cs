// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using static Oculus.Interaction.ComprehensiveSample.RopePhysics;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles rope interactions
    /// </summary>
    public class RopeInteraction : MonoBehaviour
    {
        [SerializeField]
        RopePhysics _ropePhysics;

        [SerializeField]
        Grabbable _grabbableTemplate;

        [SerializeField]
        Grabbable _grabbableEnd;

        List<Constraint> _ropeGrabbables = new List<Constraint>();

        private void Start()
        {
            List<RopePoint> points = _ropePhysics.Points;
            _ropeGrabbables.Add(new Constraint(_grabbableEnd, points[points.Count - 1]));
        }

        private void UpdateGrabbables()
        {
            List<RopePhysics.RopePoint> points = _ropePhysics.Points;
            for (int i = 1; i < points.Count - 3; i++)
            {
                RopePhysics.RopePoint point = points[i];
                if (_ropeGrabbables.FindIndex(x => x.point == point) == -1)
                {
                    var thing = Instantiate(_grabbableTemplate, transform);
                    _ropeGrabbables.Add(new Constraint(thing, point));
                }
            }

            for (int i = _ropeGrabbables.Count - 1; i >= 0; i--)
            {
                Constraint c = _ropeGrabbables[i];
                if (points.FindIndex(x => x == c.point) == -1)
                {
                    Destroy(c.grabbable);
                    _ropeGrabbables.RemoveAt(i);
                }
            }
        }

        private void Update()
        {
            UpdateGrabbables();

            for (int i = 0; i < _ropeGrabbables.Count; i++)
            {
                UpdateConstraint(_ropeGrabbables[i]);
            }
        }

        private static void UpdateConstraint(Constraint c)
        {
            var grabbable = c.grabbable;
            var point = c.point;
            var constraint = c.constriant;
            var isHeld = grabbable && grabbable.SelectingPoints != null && grabbable.SelectingPointsCount > 0;

            if (isHeld && constraint == null)
            {
                c.constriant = constraint = new RopePhysics.FixedConstraint(point);
                point.Constraints.Add(constraint);
            }
            else if (!isHeld && constraint != null)
            {
                point.Constraints.Remove(constraint);
                c.constriant = constraint = null;
            }

            if (!isHeld)
            {
                var pos = point.Position;
                var rot = Quaternion.identity;
                if (point.TryGetConstraint<DistanceConstraint>(out var dc))
                {
                    rot = Quaternion.LookRotation(dc.A.Position - dc.B.Position, grabbable.transform.forward) * Quaternion.Euler(-90, 0, 0);
                }
                grabbable.transform.SetPositionAndRotation(pos, rot);
            }
            else
            {
                constraint.Position = grabbable.transform.position;
            }
        }

        class Constraint
        {
            public Grabbable grabbable;
            public RopePhysics.RopePoint point;
            public RopePhysics.FixedConstraint constriant;

            public Constraint(Grabbable grabbable, RopePhysics.RopePoint point)
            {
                this.grabbable = grabbable;
                this.point = point;
            }
        }
    }
}
