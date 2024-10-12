// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls the LineRenderers emitted by the Cameras in the platform minigame in Street
    /// </summary>
    public class CameraLaser : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private LayerMask _mask = ~0;
        [SerializeField]
        private List<LaserInfo> _lasers = new List<LaserInfo>();
        [SerializeField]
        private List<LeverTransformVisual> _leverTransformVisuals = new List<LeverTransformVisual>();
        [SerializeField]
        private Vector2 _anglePerRenderer = new Vector3(0, 10);
        [SerializeField]
        private Color _normalColor, _suspiciousColor, _detectedColor;
        [SerializeField]
        private float _laserRange = 12f;

        private CameraStates _cameraStates = CameraStates.Normal;

        [SerializeField]
        private float _susDuration = 4f;
        private float _suspiciousTimer;
        private float[] _lastDistance;

        private const float ResetTimer = 4f;

        private Material _material;
        private PlayerHitReaction _lastHit;
        [SerializeField]
        private float _rootScale = 1;

        [SerializeField]
        ReferenceActiveState _lookForPlayer = ReferenceActiveState.Optional();
        [SerializeField, Optional]
        private ConditionalDifficultyDecrease _conditionalDifficultyDecrease;

        public bool Active => _cameraStates != CameraStates.Normal;

        public CameraStates CameraStates => _cameraStates;

        private void Start()
        {
            _suspiciousTimer = _susDuration;
            _lastDistance = new float[_lasers.Count];
            _material = new Material(_lasers[0].LineRenderer.sharedMaterial);
            _material.color = _normalColor;

            if (_conditionalDifficultyDecrease != null)
            {
                _conditionalDifficultyDecrease.WhenPlayerHasFailed += IncreaseSusDuration;
            }

            for (int i = 0; i < _lasers.Count; i++)
            {
                _lasers[i].LineRenderer.sharedMaterial = _material;
            }
        }

        private void Update()
        {
            bool canSeePlayer = false;
            for (int i = 0; i < _lasers.Count; i++)
            {
                bool raycast = Time.frameCount % _lasers.Count == i; //raycast 1 laser per frame
                canSeePlayer |= UpdateLaser(i, raycast);
            }

            if (!canSeePlayer && _cameraStates != CameraStates.Normal)
            {
                Vector3 directionToPlayer = _lastHit.transform.position - transform.position;
                canSeePlayer = RaycastPlayer(new Ray(transform.position, directionToPlayer), out var _);
            }

            UpdateState(canSeePlayer && _lookForPlayer);
        }

        /// <summary>
        /// Updates the lasers position and color, return true if the laser can see the player
        /// </summary>
        private bool UpdateLaser(int index, bool raycast)
        {
            float suspiciousAmount = _suspiciousTimer / _susDuration;

            Vector2 anglePerLaser = Vector2.Lerp(Vector2.zero, _anglePerRenderer, suspiciousAmount);
            Vector2 startAngle = -anglePerLaser * (_lasers.Count - 1) * 0.5f;
            Vector2 laserAngle = startAngle + index * anglePerLaser;

            var lineRenderer = _lasers[index].LineRenderer;
            var linePose = new Pose(transform.position, transform.rotation * Quaternion.Euler(laserAngle.x, laserAngle.y, 0));
            lineRenderer.transform.SetPose(linePose);

            var ray = new Ray(linePose.position, linePose.rotation * Vector3.forward);
            bool canSeePlayer = false;
            var distance = _lastDistance[index];
            if (raycast)
            {
                canSeePlayer = RaycastPlayer(ray, out distance);
                _lastDistance[index] = distance;
            }

            lineRenderer.transform.localScale = new Vector3(1, 1, distance);
            return canSeePlayer;
        }

        private bool RaycastPlayer(Ray ray, out float hitDistance)
        {
            bool canSeePlayer = false;
            bool hitAnything = Physics.Raycast(ray, out var hit, _laserRange, _mask, QueryTriggerInteraction.Collide);
            if (hitAnything)
            {
                var componentSource = hit.rigidbody ? hit.rigidbody.gameObject : hit.collider.gameObject;
                canSeePlayer = componentSource.TryGetComponent(out PlayerHitReaction hitThing);
                if (canSeePlayer)
                {
                    _lastHit = hitThing;
                }
            }
            hitDistance = hitAnything ? hit.distance - 0.1f : _laserRange;
            return canSeePlayer;
        }

        private void UpdateState(bool canSeePlayer)
        {
            if (_cameraStates == CameraStates.Normal && canSeePlayer)
            {
                _cameraStates = CameraStates.Suspicious;
                _suspiciousTimer = _susDuration - 1; // when becoming suspiscious, stay suspicious for at least 1 second
                TweenRunner.Kill(_material);
                SetMaterialColor(_suspiciousColor);
            }
            else if (_cameraStates == CameraStates.Suspicious && canSeePlayer)
            {
                _suspiciousTimer -= Time.deltaTime;
                if (_suspiciousTimer <= 0)
                {
                    _cameraStates = CameraStates.Detected;
                    SetMaterialColor(_detectedColor);

                    StartCoroutine(ResetLevers());
                    IEnumerator ResetLevers()
                    {
                        for (int i = 0; i < _leverTransformVisuals.Count; i++)
                        {
                            if (_leverTransformVisuals[i])
                            {
                                _leverTransformVisuals[i].ResetPosition();
                                yield return 0.5f;
                            }
                        }
                    }
                }
            }
            else if (!canSeePlayer && _cameraStates != CameraStates.Normal)
            {
                _suspiciousTimer += Time.deltaTime;

                if (_suspiciousTimer >= ResetTimer)
                {
                    _cameraStates = CameraStates.Normal;
                    _material.color = Color.clear;
                    SetMaterialColor(_normalColor, 1.5f);
                }
            }
        }

        void SetMaterialColor(Color color, float delay = 0)
        {
            TweenRunner.Kill(_material);
            if (delay > 0) TweenRunner.DelayedCall(delay, () => _material.color = color).SetID(_material);
            else _material.color = color;
        }
        private void IncreaseSusDuration()
        {
            _susDuration = 8f;
        }
    }

    [System.Serializable]
    public class LaserInfo
    {
        [SerializeField]
        private LineRenderer _lineRenderer;
        public LineRenderer LineRenderer => _lineRenderer;
    }

    public enum CameraStates
    {
        Normal,
        Suspicious,
        Detected
    }
}
