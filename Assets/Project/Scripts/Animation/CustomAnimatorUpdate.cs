// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Controls the Update of an animator, with callbacks pre and post Update
    /// This allows other behaviours to hook into animator updates and modify pose the animator applies
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CustomAnimatorUpdate : MonoBehaviour
    {
        public event Action WhenWillUpdate = delegate { };
        public event Action WhenUpdated = delegate { };

        public float speedMultiplier = 1;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animator.speed = speedMultiplier;
        }

        private void OnEnable()
        {
            // previously called _animator.Update() but that was quite bad for performance
            // Coroutines are called after all Update() calls but before the animator updates
            // Leveraging that to call when will update improves performance
            // (similar to using Execution Order but this doesnt rely on a project settings change)
            StartCoroutine(MyRoutine());
            IEnumerator MyRoutine()
            {
                while (true)
                {
                    WhenWillUpdate();
                    yield return null;
                }
            }
        }

        void LateUpdate()
        {
            WhenUpdated();
        }
    }
}
