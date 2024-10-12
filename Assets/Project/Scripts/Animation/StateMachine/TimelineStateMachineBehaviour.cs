// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Uses an ExposedReference to control a PlayableDirector using an Animator
    /// </summary>
    public class TimelineStateMachineBehaviour : ExposedStateMachineBehaviour
    {
        [SerializeField]
        ExposedReference<PlayableDirector> _playableDirector;
        [SerializeField]
        bool _syncNormalizedTime = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            GetDirector(animator).Play();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            if (_syncNormalizedTime)
            {
                var nTime = stateInfo.normalizedTime;
                var director = GetDirector(animator);
                director.playableGraph.GetRootPlayable(0).SetSpeed(stateInfo.speedMultiplier);
                director.time = nTime * director.duration;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            GetDirector(animator).Stop();
        }

        PlayableDirector GetDirector(Animator animator) => Resolve(_playableDirector, animator);
    }
}
