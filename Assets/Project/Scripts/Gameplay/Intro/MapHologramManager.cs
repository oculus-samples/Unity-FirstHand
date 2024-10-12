// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class MapHologramManager : MonoBehaviour
    {
        [SerializeField]
        List<MapSet> _sets;

        MapSet _playing;

        public bool IsPlaying => _playing != null;
        public bool HasPlayedAll => _sets.TrueForAll(x => x.played);

        private void Awake()
        {
            _sets.ForEach(x => x.button.onClick.AddListener(() => Click(x)));
        }

        void Click(MapSet set)
        {
            if (_playing != null)
            {
                Finish();
            }

            if (set.played) return;

            _playing = set;

            set.button.interactable = false;
            set.director.Play();
            set.played = true;
            set.progressTracker.SetProgress(1);

            TweenRunner.DelayedCall((float)set.director.duration, Finish).SetID(this);
        }

        private void Finish()
        {
            var director = _playing.director;
            director.time = director.playableAsset.duration - float.Epsilon;
            director.Evaluate();
            _playing.progressTracker.SetProgress(2);

            _playing = null;
            TweenRunner.Kill(this);
        }

        [Serializable]
        class MapSet
        {
            public Button button;
            public PlayableDirector director;
            public ProgressTracker progressTracker;
            [NonSerialized] public bool played = false;
        }
    }
}
