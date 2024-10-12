// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class SubtitlePlayableClip : PlayableAsset, ITimelineClipAsset
    {
        public SubtitlePlayableBehaviour template = new SubtitlePlayableBehaviour();
        public ExposedReference<Transform> Transform;

        public CharacterSubtitlePreset Preset;
        [TextArea(5, 10)]
        public string String;
        public bool Mute = false;

        public float DurationOverride = -1;

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SubtitlePlayableBehaviour>.Create(graph, template);
            SubtitlePlayableBehaviour clone = playable.GetBehaviour();
            clone.Transform = Transform.Resolve(graph.GetResolver());
            clone.Preset = Preset;
            clone.String = String;
            clone.Mute = Mute;
            clone.DurationOverride = DurationOverride;
            return playable;
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class WordsPerMinuteContextMenu
    {
        static WordsPerMinuteContextMenu()
        {
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.String)
                return;

            Type type = property.serializedObject.targetObject.GetType();
            if (type != typeof(SubtitlePlayableClip) && type != typeof(SubtitleHints) && type != typeof(SubtitleInformation))
                return;

            var value = property.stringValue;

            var words = SubtitleManager.GetWordCount(value);
            var duration140 = SubtitleManager.GetDurationEstimate(value, 140);
            var duration170 = SubtitleManager.GetDurationEstimate(value, 170);

            menu.AddDisabledItem(new GUIContent($"140:{duration140}, 170:{duration170}, WC:{words}"));
        }
    }
#endif
}
