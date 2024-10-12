// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Fabricator audio trigger event handler
    /// </summary>
    public class AudioTriggerAnimationEventHandler : MonoBehaviour
    {
        public AudioTrigger fabricatorBoxOpenLeft;
        public AudioTrigger fabricatorBoxOpenRight;
        public AudioTrigger fabricatorUnlock;
        public AudioTrigger fabricatorCrankOpen;
        public AudioTrigger fabricatorMountFoldIn;
        public AudioTrigger fabricatorMountFoldUpOut;
        public AudioTrigger fabricatorMountFoldDownIn;

        public List<NamedAudioTrigger> audios = new List<NamedAudioTrigger>();

        public void playFabricatorCrankOpen()
        {
            if (fabricatorCrankOpen != null)
            {
                fabricatorCrankOpen.Play();
                Debug.Log("Played audio 1", this);
            }
        }

        public void playFabricatorUnlockAudio()
        {
            if (fabricatorUnlock != null)
            {
                fabricatorUnlock.Play();
                Debug.Log("Played audio 2", this);
            }
        }

        public void playFabricatorBoxOpenAudio()
        {
            if (fabricatorBoxOpenLeft != null && fabricatorBoxOpenRight != null)
            {
                fabricatorBoxOpenLeft.Play();
                fabricatorBoxOpenRight.Play();
                Debug.Log("Played audio 3", this);
            }
        }

        public void playFabricatorMountFoldIn()
        {
            if (fabricatorMountFoldIn != null)
            {
                fabricatorMountFoldIn.Play();
                Debug.Log("Played audio 4", this);
            }
        }

        public void playFabricatorMountFoldUpOut()
        {
            if (fabricatorMountFoldUpOut != null)
            {
                fabricatorMountFoldUpOut.Play();
                Debug.Log("Played audio 5", this);
            }
        }

        public void playFabricatorMountFoldDownIn()
        {
            if (fabricatorMountFoldDownIn != null)
            {
                fabricatorMountFoldDownIn.Play();
                Debug.Log("Played audio 6");
            }
        }

        public void PlayAudio(string name)
        {
            for (int i = 0; i < audios.Count; i++)
            {
                if (audios[i].name == name)
                {
                    audios[i].audio.Play();
                }
            }
        }

        [System.Serializable]
        public struct NamedAudioTrigger
        {
            public string name;
            public AudioTrigger audio;
        }
    }
}
