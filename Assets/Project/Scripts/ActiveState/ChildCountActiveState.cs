// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true if the number of children in a transform
    /// is equal to the specified amount in _childCount
    /// </summary>
    public class ChildCountActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField, FormerlySerializedAs("_range")]
        private FloatRanges _childCount = new FloatRanges(1, 1);

        public bool Active => _childCount.Contains(transform.childCount);
    }
}
