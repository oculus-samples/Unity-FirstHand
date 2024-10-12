// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Voice;
using Oculus.VoiceSDK.Utilities;
using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles the drones actions when a voice command is called
    /// </summary>
    public class DroneVoiceController : MonoBehaviour
    {
        [SerializeField]
        private AppVoiceExperience _appVoiceExperience;

        [SerializeField]
        ReferenceActiveState _shouldListen;

        [SerializeField]
        private DroneCommandPreset _flyTo, _pickUp, _drop, _scan;

        [SerializeField]
        Animator _animator;

        [SerializeField]
        UICanvas _canListen, _listening, _understood, _confused;

        private bool _waitingForInput;
        private bool _isListening;
        private bool _started;

        public bool IsListening => _isListening;

        private void Start()
        {
            _appVoiceExperience.VoiceEvents.OnRequestCreated.AddListener(x => HandleVoiceEvent(true));
            _appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(() => HandleVoiceEvent(false));

            StartCoroutine(FixForBigFrameDrop());
            IEnumerator FixForBigFrameDrop()
            {
                yield return new WaitForSeconds(0.4f);
                _appVoiceExperience.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);

                if (MicPermissionsManager.HasMicPermission())
                {
                    _appVoiceExperience.Activate();
                    yield return new WaitForSeconds(0.1f);
                    _appVoiceExperience.Deactivate();
                }

                _started = true;
            }
        }

        void HandleVoiceEvent(bool listening)
        {
            _isListening = listening;

            if (_listening.IsShown != listening) _listening.Show(listening);

            if (listening)
            {
                if (_canListen.IsShown) _canListen.Show(false);
                if (_confused.IsShown) _confused.Show(false);
                if (_understood.IsShown) _understood.Show(false);
            }

            if (!listening)
            {
                // if we stopped listening wait a sec then try to perform a "confused" null command
                // if the drone is busy then it will be ignored
                TweenRunner.DelayedCall(0.00001f, () => PerformCommand(null, null));
            }
        }

        private void Update()
        {
            if (!_started) return;

            if (!_shouldListen && (_isListening || _waitingForInput)) SetWaitingForInput(false);

            bool canStartListening = _shouldListen && !_waitingForInput && !DroneCommandHandler.Instance.IsBusy;
            if (canStartListening) SetWaitingForInput(true);
        }

        public void HandleFlyToResponse(string[] info)
        {
            var subject = string.Join(" ", info).Trim().ToLower();
            PerformCommand(subject, _flyTo);
        }

        public void HandlePickUpResponse(string[] info)
        {
            var subject = string.Join(" ", info).Trim().ToLower();
            PerformCommand(subject, _pickUp);
        }

        public void HandleDropResponse(string[] info)
        {
            var subject = string.Join(" ", info).Trim().ToLower();
            PerformCommand(subject, _drop);
        }

        public void HandleScanResponse(string[] info)
        {
            var subject = string.Join(" ", info).Trim().ToLower();
            PerformCommand(subject, _scan);
        }

        public void PerformCommand(string subjectID, DroneCommandPreset command)
        {
            if (DroneCommandHandler.Instance.IsBusy) return;

            bool success = DroneCommandHandler.Instance.PerformCommand(subjectID, command);
            bool confused = !success;

            Debug.Log($"PerformCommand {success}");

            if (_canListen.IsShown) _canListen.Show(false);
            if (_listening.IsShown) _listening.Show(false);
            if (_confused.IsShown != confused) _confused.Show(confused);
            if (_understood.IsShown != success) _understood.Show(success);

            TweenRunner.DelayedCall(1f, () =>
            {
                if (_confused.IsShown) _confused.Show(false);
                if (_understood.IsShown) _understood.Show(false);
                if (_waitingForInput && !_canListen.IsShown) _canListen.Show(true);
            });

            if (confused)
            {
                DroneCommandHandler.Instance.SetBusy(1f);
                _animator.SetTrigger("confused");
            }
        }

        public bool SetWaitingForInput(bool waitForInput)
        {
            if (!MicPermissionsManager.HasMicPermission()) return false;

            _waitingForInput = waitForInput;

            if (waitForInput)
            {
                _appVoiceExperience.Activate();
                if (!_canListen.IsShown) _canListen.Show(true);
                return true;
            }

            if (!waitForInput)
            {
                _appVoiceExperience.Deactivate();
                if (_canListen.IsShown) _canListen.Show(false);
                return true;
            }

            return false;
        }
    }
}
