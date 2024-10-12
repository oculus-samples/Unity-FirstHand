// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Audio trigger plays on collision enter
    /// </summary>
    public class PhysicsAudioTrigger : MonoBehaviour
    {
        [SerializeField]
        private AudioTrigger _audioTrigger;

        private void OnCollisionEnter()
        {
            _audioTrigger.Play();
        }
    }
}
