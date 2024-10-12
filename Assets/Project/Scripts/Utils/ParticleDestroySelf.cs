// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Particle system destroys self after its duration reaches 0
    /// </summary>
    public class ParticleDestroySelf : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
            var main = _particleSystem.main;
            Destroy(gameObject, main.duration);
        }
    }
}
