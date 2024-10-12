// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(UICanvasClip))]
    [TrackBindingType(typeof(UICanvas))]
    public class UICanvasTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<UICanvasMixerBehaviour>.Create(graph, inputCount);
        }

        // Please note this assumes only one component of type UICanvas on the same gameobject.
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            UICanvas trackBinding = director.GetGenericBinding(this) as UICanvas;
            if (trackBinding == null)
                return;

            driver.AddFromName<UICanvas>(trackBinding.gameObject, "_show");
            CanvasGroup group = trackBinding.GetComponent<CanvasGroup>();
            driver.AddFromName<CanvasGroup>(trackBinding.gameObject, "m_Alpha");
#endif
            base.GatherProperties(director, driver);
        }
    }
}
