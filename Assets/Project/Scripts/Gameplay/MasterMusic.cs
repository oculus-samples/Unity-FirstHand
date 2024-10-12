// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Singleton that handles the currenltly playing audio track
    /// </summary>
    public class MasterMusic : MonoBehaviour
    {
        private static MasterMusic _instance;
        public static MasterMusic Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MasterMusic>();
                }
                return _instance;
            }
        }

        [SerializeField]
        private List<MusicPair> _music = new List<MusicPair>();

        private AudioTrigger _currentMusic;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Play(string id)
        {
            SetCurrentMusic(_music.Find(x => x.ID == id).Music);
        }

        private void SetCurrentMusic(AudioTrigger music)
        {
            if (_currentMusic == music) { return; }

            var previous = _currentMusic;
            if (previous != null)
            {
                previous.Stop();
            }

            _currentMusic = music;
            music.Play();
        }

        [Serializable]
        struct MusicPair
        {
            [SerializeField]
            string _id;
            [SerializeField]
            AudioTrigger _music;

            public string ID => _id;
            public AudioTrigger Music => _music;
        }
    }
}
