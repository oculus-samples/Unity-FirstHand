// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Collectable coin controller
    /// </summary>
    public class CollectCoin : MonoBehaviour
    {
        static List<string> _keys = new List<string>();

        [SerializeField] StringPropertyPlayerPref _pref;
        [SerializeField] PlayableDirector _pickupTimeline;
        [SerializeField] PlayableDirector _spinTimeline;
        [SerializeField] PlayableDirector _destroyTimeline;

        //Distance Coin Collections
        [SerializeField] float _speed = 1f;

        InteractionTracker _interactionTracker;

        private void Start()
        {
            _pref.Initialize();
            if (_pref.Property.Value == "yes")
            {
                gameObject.SetActive(false);
                return;
            }

            _interactionTracker = new InteractionTracker(GetComponent<IPointableElement>());
            _interactionTracker.WhenChanged += TryCollect;

            void TryCollect()
            {
                bool isGrabbed = _interactionTracker.IsGrabbed();
                bool isCollected = _pref.Property.Value == "yes";

                if (!isCollected && isGrabbed) PickUpCoin();
                else if (isCollected && !isGrabbed) RemoveCoin();
            }
        }

        // called by unity event
        public void PickUpCoin()
        {
            _spinTimeline.Stop();
            _pickupTimeline.Play();

            var prop = _pref.Property;
            prop.Value = "yes";

        }

        public void RemoveCoin()
        {
            StartCoroutine(RemoveRoutine());
            IEnumerator RemoveRoutine()
            {
                yield return new WaitForSeconds(0.15f);
                if (_interactionTracker.IsGrabbed()) yield break;

                _destroyTimeline.Play();
                yield return new WaitForDirector(_destroyTimeline);
                gameObject.SetActive(false);
            }
        }


    }
}
