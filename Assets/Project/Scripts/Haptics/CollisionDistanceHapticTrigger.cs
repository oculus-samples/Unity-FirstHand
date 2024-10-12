// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Haptics;
using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Play haptic based on collision
    /// </summary>
    public class CollisionDistanceHapticTrigger : MonoBehaviour
    {
        [SerializeField]
        private DistanceHapticSource _source;

        [SerializeField]
        private HapticClip _clip;

        private void Awake()
        {
            var rigidbody = GetComponentInParent<Rigidbody>();
            var listener = rigidbody.gameObject.AddComponent<CollisionEnterListener>();
            listener.WhenCollisionEnter += PlayClip;
        }

        private void PlayClip(GameObject bitThatCollidedWithSomething)
        {
            if (bitThatCollidedWithSomething == gameObject)
            {
                Play();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Play();
        }

        internal void Play()
        {
            _source.Clip = _clip;
            _source.Play();
        }

        class CollisionEnterListener : MonoBehaviour
        {
            public event Action<GameObject> WhenCollisionEnter;

            private void OnCollisionEnter(Collision collision)
            {
                var contact = collision.GetContact(0);
                WhenCollisionEnter?.Invoke(contact.thisCollider.gameObject);
            }

            private void OnTriggerEnter(Collider other)
            {
                WhenCollisionEnter?.Invoke(gameObject);
            }
        }
    }
}
