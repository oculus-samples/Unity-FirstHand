// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class ConfigActiveStateMixerBehaviour : PlayableBehaviour
    {
        ConfigurableActiveState _trackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _trackBinding = playerData as ConfigurableActiveState;

            if (_trackBinding == null)
                return;

            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                if (playable.GetInputWeight(i) > 0)
                {
                    _trackBinding.ActiveSelf = true;
                    return;
                }
            }

            _trackBinding.ActiveSelf = false;
        }
    }
}
