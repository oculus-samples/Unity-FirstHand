// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class AnimatorParameterTrack<T> : TrackAsset where T : AnimatorParameterMixerBehaviour, IPlayableBehaviour, new()
    {
        [SerializeField]
        public string parameter;
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                if (clip.blendInCurveMode == TimelineClip.BlendCurveMode.Auto)
                {
                    clip.mixInCurve = AnimationCurve.Linear(0, 0, 1, 1);
                }
                if (clip.blendOutCurveMode == TimelineClip.BlendCurveMode.Auto)
                {
                    clip.mixOutCurve = AnimationCurve.Linear(0, 1, 1, 0);
                }
            }

            var mixer = ScriptPlayable<T>.Create(graph, inputCount);
            mixer.GetBehaviour()._parameterID = parameter;
            return mixer;
        }

    }
}
