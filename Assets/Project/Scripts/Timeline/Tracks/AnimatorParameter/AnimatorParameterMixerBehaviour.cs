// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public abstract class AnimatorParameterMixerBehaviour : PlayableBehaviour
    {
        protected Animator _trackBinding;

        internal string _parameterID;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _trackBinding = playerData as Animator;
            if (_trackBinding == null || !_trackBinding.isActiveAndEnabled || _trackBinding.runtimeAnimatorController == null)
                return;

            AnimatorProcessFrame(playable);
        }

        protected abstract void AnimatorProcessFrame(Playable playable);
    }
}
