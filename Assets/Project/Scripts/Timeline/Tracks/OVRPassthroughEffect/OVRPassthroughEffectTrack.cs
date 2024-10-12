// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(1f, 0f, 0.9860897f)]
    [TrackClipType(typeof(OVRPassthroughEffectClip))]
    [TrackBindingType(typeof(OVRPassthroughLayer))]
    public class OVRPassthroughEffectTrack : TrackAsset
    {
        public static OVRPassthroughLayer Instance
        {
            get
            {
                if (!_instance) _instance = FindObjectOfType<OVRPassthroughLayer>();
                return _instance;
            }
        }
        static OVRPassthroughLayer _instance;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<OVRPassthroughEffectMixerBehaviour>.Create(graph, inputCount);
        }

        // Please note this assumes only one component of type OVRPassthroughLayer on the same gameobject.
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            OVRPassthroughLayer trackBinding = director.GetGenericBinding(this) as OVRPassthroughLayer;
            if (trackBinding == null) trackBinding = Instance;
            if (trackBinding == null) return;

            driver.AddFromName<OVRPassthroughLayer>(trackBinding.gameObject, "colorMapEditorBrightness");
            driver.AddFromName<OVRPassthroughLayer>(trackBinding.gameObject, "colorMapEditorContrast");
            driver.AddFromName<OVRPassthroughLayer>(trackBinding.gameObject, "colorMapEditorSaturation");
#endif
            base.GatherProperties(director, driver);
        }
    }
}
