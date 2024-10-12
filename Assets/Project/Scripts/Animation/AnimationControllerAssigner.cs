// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Fix for animator that is controlled by a timeline and animator
    /// </summary>
    public class AnimationControllerAssigner : MonoBehaviour
    {
        [SerializeField] RuntimeAnimatorController _controller;
        [SerializeField] private bool _applyRootMotion;

        void Awake()
        {
            Animator animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = _controller;
            animator.applyRootMotion = _applyRootMotion;
        }
    }
}
