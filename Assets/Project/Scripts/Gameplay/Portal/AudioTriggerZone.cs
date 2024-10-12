// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Plays audio when player enters or exits trigger zone
    /// </summary>
    public class AudioTriggerZone : MonoBehaviour
    {
        [SerializeField]
        private TriggerZone _triggerZone;
        [SerializeField]
        private AudioTrigger _handIn, _handOut;

        TriggerZoneList<Collider> _zoneList;

        private void Awake()
        {
            _zoneList = new TriggerZoneList<Collider>(_triggerZone);
            _zoneList.WhenAdded += PlayEnterAudio;
            _zoneList.WhenRemoved += PlayExitAudio;
        }

        private void PlayExitAudio(Collider obj)
        {
            _handIn.Play();
        }

        private void PlayEnterAudio(Collider obj)
        {
            _handOut.Play();
        }
    }
}
