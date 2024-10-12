// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Localizes UI text elements
    /// </summary>
    public class LocalizedUI : MonoBehaviour
    {
        private static List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
        private static List<GameObject> _roots = new List<GameObject>();

        private void Start()
        {
            var scene = gameObject.scene;
            scene.GetRootGameObjects(_roots);

            for (int i = 0; i < _roots.Count; i++)
            {
                _roots[i].GetComponentsInChildren(true, _texts);
                _texts.ForEach(x =>
                {
                    if (IgnoreLocalization.ShouldIgnore(x)) return;
                    x.text = LocalizedText.GetUIText(x.text);
                });
            }

#if UNITY_EDITOR
            var directors = new List<PlayableDirector>();
            for (int i = 0; i < _roots.Count; i++)
            {
                _roots[i].GetComponentsInChildren(true, directors);
                foreach (var director in directors)
                {
                    var asset = director.playableAsset as TimelineAsset;
                    if (!asset) continue;

                    foreach (var track in asset.GetOutputTracks())
                    {
                        if (track.muted) continue;
                        if (!(track is SubtitlePlayableTrack subtitleTrack)) continue;

                        foreach (var clip in subtitleTrack.GetClips())
                        {
                            if (!(clip.asset is SubtitlePlayableClip subtitleClip)) continue;
                            if (subtitleClip.Mute) continue;

                            LocalizedText.GetSubtitle(subtitleClip.String);
                        }
                    }
                }
            }
#endif

            _texts.Clear();
            _roots.Clear();
        }

    }
}
