// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;
using UnityEngine.Android;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Returns true is voice permissions are allowed
    /// </summary>
    public class VoiceAvailabilityActiveState : MonoBehaviour, IActiveState
    {
        private bool _hasMicPermission = false;
        private bool _hasInternet = true;

        public bool Active => _hasMicPermission && _hasInternet;

        void OnEnable()
        {
            StartCoroutine(InternetCheck());
            StartCoroutine(MicCheck());

            // Checks for internet every 10 seconds, assumes its available to start with
            IEnumerator InternetCheck()
            {
                _hasInternet = true;
                var waitForSeconds = new WaitForSecondsRealtime(10f);
                while (true)
                {
                    yield return waitForSeconds;
                    _hasInternet = Application.internetReachability != NetworkReachability.NotReachable;
                }
            }

            // Checks for mic permissions, allowing 10 seconds for it to become active
            IEnumerator MicCheck()
            {
                var waitForSeconds = new WaitForSecondsRealtime(1f);
                for (int i = 0; i < 10; i++)
                {
                    _hasMicPermission = Permission.HasUserAuthorizedPermission(Permission.Microphone);
                    if (_hasMicPermission) yield break;

                    yield return waitForSeconds;
                }
            }
        }
    }
}
