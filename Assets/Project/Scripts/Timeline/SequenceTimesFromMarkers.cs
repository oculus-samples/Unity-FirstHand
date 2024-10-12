// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Populates the times of ActiveStateTimelineSequence via a ContextMenu using Markers in the timeline asset
    /// </summary>
    public class SequenceTimesFromMarkers : MonoBehaviour
    {
        private void Awake()
        {
            if (isActiveAndEnabled)
            {
                Populate();
            }
        }

        // for enabled box
        private void OnEnable()
        {

        }

        [ContextMenu("Populate")]
        void Populate()
        {
            var sequence = GetComponent<ActiveStateTimelineSequence>();
            var director = sequence.PlayableDirector;
            var steps = sequence.ActiveStateTimes;

            var markers = MarkerTrack.GetTimeRanges(director);
            for (int i = 0; i < markers.Count; i++)
            {
                TimeRange marker = markers[i];
                if (steps.FindIndex(x => x.Name == marker.TimelineName) == -1)
                {
                    steps.Insert(Math.Min(i, steps.Count), new ActiveStateTime(marker.TimelineName));
                }
            }

            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                var time = MarkerTrack.GetTimeRange(director, step.Name);
                step.Time = (float)time.End;
                steps[i] = step;
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(sequence);
#endif
        }
    }
}
