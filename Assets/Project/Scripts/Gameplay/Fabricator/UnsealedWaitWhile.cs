// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Used to keep a coroutine running until the predicate returns true
    /// Adapted from https://docs.unity3d.com/ScriptReference/CustomYieldInstruction.html
    /// </summary>
    class UnsealedWaitWhile : CustomYieldInstruction
    {
        private Func<bool> _predicate;
        public override bool keepWaiting => _predicate();
        public UnsealedWaitWhile(Func<bool> predicate) => _predicate = predicate;
    }

    /// <summary>
    /// Yields until the playing PlayableDirector reaches a time
    /// </summary>
    class WaitForDirector : UnsealedWaitWhile
    {
        public WaitForDirector(PlayableDirector x, float time = -1) : base(() => x.time < (time < 0 ? x.duration : time) - double.Epsilon) { }
    }

    /// <summary>
    /// Yields until the DropZoneInteractable is held/selected
    /// </summary>
    class WaitForDropZone : UnsealedWaitWhile
    {
        public WaitForDropZone(SnapInteractable x) : base(() => x.SelectingInteractorViews.Count() <= 0) { }
    }

    static class WaitForSecondsNonAlloc
    {
        static Dictionary<float, WaitForSeconds> _instances = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            if (!_instances.TryGetValue(seconds, out var result))
            {
                result = new WaitForSeconds(seconds);
                _instances.Add(seconds, result);
            }
            return result;
        }
    }
}
