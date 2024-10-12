// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(ImageFillClip))]
    [TrackBindingType(typeof(Image))]
    public class ImageFillTrack : TrackAsset
    {
        [SerializeField]
        public bool _isMask;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            ScriptPlayable<ImageFillMixerBehaviour> playable = ScriptPlayable<ImageFillMixerBehaviour>.Create(graph, inputCount);
            playable.GetBehaviour()._isMask = _isMask;
            return playable;
        }

        // Please note this assumes only one component of type Image on the same gameobject.
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            Image trackBinding = director.GetGenericBinding(this) as Image;
            if (trackBinding == null)
            {
                return;
            }
            driver.AddFromName<Image>(trackBinding.gameObject, "m_FillAmount");
#endif
            base.GatherProperties(director, driver);
        }
    }
}
