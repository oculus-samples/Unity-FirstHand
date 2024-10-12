// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Reparents an item based on ActiveStates
    /// </summary>
    public class ConditionalSetParent : MonoBehaviour
    {
        [SerializeField]
        private List<ConditionalTransform> _parents = new List<ConditionalTransform>();

        [SerializeField]
        private Transform _default;

        private List<IConditionalTransform> _internalParents = new List<IConditionalTransform>();

        private void Reset()
        {
            _default = transform.parent;
        }

        private void Awake()
        {
            _parents.ForEach(x => _internalParents.Add(x));
        }

        private void Update()
        {
            var activeParent = GetActiveParent();
            if (transform.parent != activeParent)
            {
                transform.SetParent(activeParent);
            }
        }

        private Transform GetActiveParent() => GetActiveOrDefault(_internalParents, _default);

        public static Transform GetActiveOrDefault(List<IConditionalTransform> options, Transform fallback)
        {
            var activeIndex = options.FindIndex(x => x.Active);
            return activeIndex >= 0 ? options[activeIndex].Transform : fallback;
        }

        /// <summary>
        /// Adds another IConditionalParent to the parent options, useful when the transform is only known at runtime
        /// </summary>
        public void Insert(int index, IConditionalTransform parent)
        {
            _internalParents.Insert(index, parent);
        }

        [Serializable]
        public struct ConditionalTransform : IConditionalTransform
        {
            [SerializeField] Transform _parent;
            [SerializeField] ReferenceActiveState _active;

            public Transform Transform => _parent;
            public bool Active => _active;
        }

        public interface IConditionalTransform
        {
            Transform Transform { get; }
            bool Active { get; }
        }
    }
}
