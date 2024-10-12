// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class PackagesInteractable : MonoBehaviour
    {
        [SerializeField]
        private GameObject _particleSystem;
        [SerializeField]
        FollowTransform _followTransform;
        [SerializeField]
        private AudioTrigger _audio;

        private Transform _target;

        public Transform Target => _target;

        public event Action<PackagesInteractable> WhenPackageIsCollected;

        private void OnTriggerEnter(Collider theCol)
        {
            if (theCol.TryGetComponent<CollisionDistanceHapticTrigger>(out var hapticTrigger))
            {
                Collect();
                _audio.Play();
                hapticTrigger.Play();
            }
        }

        private void Collect()
        {
            Instantiate(_particleSystem, transform.position, Quaternion.identity);

            gameObject.SetActive(false);
            _followTransform.Source = _target = null;

            WhenPackageIsCollected?.Invoke(this);
        }

        public void SetTargetAndShow(Transform target)
        {
            _target = target;
            _followTransform.Source = target;
            transform.position = target.position.SetY(-20);
            gameObject.SetActive(true);
        }

        private void Update()
        {
            _followTransform.UpdatePose();
        }
    }
}
