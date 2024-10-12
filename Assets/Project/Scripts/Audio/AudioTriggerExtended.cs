// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Audio trigger extensions to add fade in & out
    /// </summary>
    public class AudioTriggerExtended : AudioTrigger
    {
        [Header("Extended")]
        [SerializeField]
        private float _fadeIn;
        [SerializeField]
        private float _fadeOut;

        public float FadeOut => _fadeOut;

        protected override void Start()
        {
            base.Start();
        }

        new public void PlayAudio()
        {
            this.PlayAudio(_fadeIn, false);
        }

        public void StopAudio()
        {
            this.Stop(_fadeOut);
        }
    }
}
