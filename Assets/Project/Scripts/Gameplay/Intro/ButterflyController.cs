// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using static Oculus.Interaction.ComprehensiveSample.FollowTransform;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class ButterflyController : MonoBehaviour
    {
        public static readonly List<IButterflyTarget> Targets = new List<IButterflyTarget>();

        [SerializeField]
        TriggerZone _triggerZone;

        [SerializeField]
        Animator _animator;

        [SerializeField]
        Noise3 _noise;

        private float _searchDistance = 0.3f;

        Pose _targetPose;
        [SerializeField]
        private float _changeTargetDistance = 0.3f;
        [SerializeField]
        private float _nextTargetDistance = 1f;

        private Vector3 _velocity;
        private Vector3 _angularVelocity;

        private float _minfleeSpeed = 1f;
        private float _fear = 0;
        private float _boredom = 0;

        IButterflyTarget _target;
        IButterflyTarget _lastOccupiedTarget;

        bool HasLandingTarget => _target != null;

        private void Start()
        {
            _triggerZone.WhenAdded += TryFlee;
            Flee(transform.forward, Random.Range(_nextTargetDistance, 1f));

            ExplosiveForce.OnExplode += ReactToExplosion;
        }

        private void OnDestroy()
        {
            ExplosiveForce.OnExplode -= ReactToExplosion;
        }

        private void ReactToExplosion(Vector3 pos, float _, float force, float __)
        {
            if (force >= 0) return;
            var toPos = pos - transform.position;
            Flee(toPos, toPos.magnitude);
        }

        private void TryFlee(Collider obj)
        {
            if (obj.isTrigger) return;

            var rigidbody = obj.attachedRigidbody;
            if (rigidbody == null) return;

            var closestPoint = obj.ClosestPoint(transform.position);
            var velocity = rigidbody.GetPointVelocity(closestPoint);

            if (velocity.CompareMagnitude(_minfleeSpeed) > 0)
            {
                Vector3 fleeDirection = ((transform.position - closestPoint).normalized + Vector3.up).normalized;
                Flee(fleeDirection, _nextTargetDistance);
            }
        }

        private void Flee(Vector3 fleeDirection, float distance)
        {
            _boredom = 0;

            _targetPose.position = FindNewMidAirPose(fleeDirection, distance);

            if (_target != null)
            {
                _lastOccupiedTarget = _target;
                SetTarget(null);
            }

            // tween fear up to 1 quickly then back down to 0 slowly
            TweenRunner.Tween(_fear, 1, 0.1f, x => _fear = x)
                .SetID(this)
                .OnComplete(() => TweenRunner.Tween(_fear, 0, 0.9f, x => _fear = x).SetID(this));
        }

        private void Update()
        {
            UpdateTargetPose();

            var isAttached = HasLandingTarget && (transform.position - _targetPose.position).IsMagnitudeLessThan(0.03f);

            _animator.SetBool("Flying", !isAttached);

            var maxSpeed = isAttached ? float.MaxValue : Mathf.Lerp(0.3f, 1f, _fear);
            var smoothTime = isAttached ? 3 : Mathf.Lerp(100, 50, _fear);

            var noise = Vector3.zero;
            if (!HasLandingTarget) //only apply noise in mid air
            {
                noise = _noise.Evaluate(Time.time);
            }

            var nextPos = Vector3.SmoothDamp(transform.position, _targetPose.position + noise, ref _velocity, smoothTime * Time.deltaTime, maxSpeed);
            var aim = nextPos - transform.position;
            // workaround for Quaternion.LookRotation spamming logs when passed a near 0 vector
            if (aim.sqrMagnitude < 0.000000001f) aim = transform.forward;

            var rot = isAttached ? Quaternion.LookRotation(_targetPose.forward) : Quaternion.LookRotation(aim);
            transform.position = nextPos;
            transform.rotation = SmoothDampQuaternion(transform.rotation, rot, ref _angularVelocity, Mathf.Lerp(100, 50, _fear) * Time.deltaTime);

            UpdateFlapSpeed(aim.y);

            if (isAttached)
            {
                _boredom += Time.deltaTime * 0.1f;
                if (_boredom >= 1)
                {
                    Flee(Vector3.up, _searchDistance + 0.1f);
                }
            }
        }

        private void UpdateTargetPose()
        {
            if (FindLandingPose())
            {
                // if we find somewhere to land update the target to that
                _targetPose = _target.Pose;
            }
            else
            {
                // otherwise measure the distance to the current target, and make a new traget is we're close enough
                bool pickNewTarget = (_targetPose.position - transform.position).IsMagnitudeLessThan(_changeTargetDistance);
                if (pickNewTarget)
                {
                    Vector3 pos = FindNewMidAirPose(transform.forward, _nextTargetDistance);
                    _targetPose = new Pose(pos, Quaternion.identity);
                }
            }
        }

        /// <summary>
        /// Quaternion equivalent of Vector3.SmoothDamp
        /// </summary>
        public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
        {
            if (current == target)
            {
                currentVelocity = Vector3.zero;
                return current;
            }

            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
              Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
              Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
              Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }

        /// <summary>
        /// Finds a position in the air that the butterfly can see, not blocked by colliders
        /// </summary>
        private Vector3 FindNewMidAirPose(Vector3 direction, float distance)
        {
            var currentPose = transform.GetPose(Space.World);

            for (int i = 0; i < 5; i++)
            {
                float angle = Mathf.Lerp(45f, 120f, i / 4f);
                Vector3 offset = RandomInCone(new Pose(currentPose.position, Quaternion.LookRotation(direction)), angle, (float)distance);
                var ray = new Ray(currentPose.position, offset.normalized);

                if (!Physics.SphereCast(ray, 0.2f, (float)distance, ~0, QueryTriggerInteraction.Ignore))
                {
                    return currentPose.position + offset;
                }
            }

            return currentPose.position;
        }

        /// <summary>
        /// Returns a random offset from the current pose
        /// </summary>
        private static Vector3 RandomInCone(Pose currentPose, float angle, float distance)
        {
            var randomRotationOffset = Quaternion.Euler(Random.Range(-angle, angle), Random.Range(-angle, angle), 0);
            var offset = (currentPose.rotation * randomRotationOffset) * Vector3.forward * distance;
            return offset;
        }

        /// <summary>
        /// Sets _target to the nearest landing target
        /// </summary>
        private bool FindLandingPose()
        {
            SetTarget(null);
            // if a butterly is fleeing it shouldnt land
            if (_fear > 0) return false;

            foreach (var target in Targets)
            {
                if (target.Occupied || target == _lastOccupiedTarget) continue;

                if (target.Active && (target.Pose.position - transform.position).IsMagnitudeLessThan(_searchDistance))
                {
                    SetTarget(target);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the current target and marks it as occupied so butterflies dont try to land in the same place
        /// </summary>
        /// <param name="target"></param>
        private void SetTarget(IButterflyTarget target)
        {
            if (_target != null)
            {
                _target.Occupied = false;
                _target = null;
            }

            _target = target;

            if (_target != null)
            {
                _target.Occupied = true;
            }
        }

        /// <summary>
        /// uses hard tweaked values to speed up and slow down the wings flapping
        /// based on how much effor the butterfly is putting in
        /// </summary>
        /// <param name="upwardMovement"></param>
        private void UpdateFlapSpeed(float upwardMovement)
        {
            if (PauseHandler.IsTimeStopped) return;

            float upSpeed = (upwardMovement + 0.01f) / Time.deltaTime;
            float t = Mathf.Lerp(0.5f, 2, upSpeed / 5);
            _animator.SetFloat("FlapSpeed", t * 0.5f + 1.2f);
        }
    }

    public interface IButterflyTarget
    {
        bool Occupied { get; set; }
        Pose Pose { get; }
        bool Active { get; }
    }
}
