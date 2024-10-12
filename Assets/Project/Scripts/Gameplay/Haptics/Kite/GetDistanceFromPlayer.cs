// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class GetDistanceFromPlayer : MonoBehaviour
    {
        [SerializeField]
        private Transform _controller;
        public Transform Controller => _controller;

        public Vector3 Position => _controller.position;

        public Vector3 PlayerPosition => _mainCamera.transform.position;

        private static Camera _mainCamera;
        private Transform _playerTransform;

        private void Awake()
        {
            if (!_mainCamera) _mainCamera = Camera.main;
            if (!_mainCamera) return;
            _playerTransform = _mainCamera.transform;
        }

        public float DistanceFromPlayer()
        {
            var distance = Vector3.Distance(_controller.position, _playerTransform.position);
            return distance;
        }
    }
}
