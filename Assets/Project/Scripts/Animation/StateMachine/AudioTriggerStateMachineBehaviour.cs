// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays an AudioTrigger when the state is entered
    /// </summary>
    public class AudioTriggerStateMachineBehaviour : ExposedStateMachineBehaviour
    {
        [SerializeField]
        ExposedReference<AudioTrigger> _audioTrigger;
        [SerializeField]
        bool _playEachLoop;

        private int _lastLoop;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            _lastLoop = 0;
            PlayAudio(animator);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            int loop = (int)stateInfo.normalizedTime;
            if (_playEachLoop && stateInfo.loop && loop != _lastLoop)
            {
                _lastLoop = loop;
                PlayAudio(animator);
            }
        }

        private void PlayAudio(Animator animator)
        {
            if (TryResolve(_audioTrigger, animator, out var audioTrigger))
            {
                audioTrigger.Play();
            }
        }
    }
}
