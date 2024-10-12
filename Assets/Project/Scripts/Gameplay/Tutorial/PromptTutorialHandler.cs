// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.Locomotion;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Manages the tutorial prompts
    /// </summary>
    public class PromptTutorialHandler : MonoBehaviour
    {
        [SerializeField]
        private ConfigurableActiveState _locomotionPrompt, _snapRotatePrompt;
        [SerializeField]
        private PlayerLocomotor _playerLocomotor;
        [SerializeField]
        ReferenceActiveState _shouldCountdown;

        private float _locomotionCountdown, _snapTurnCountdown;
        private float _countdownMaxTimer = 20f;
        private float _additionalTime = 10f;

        private bool IsPromptCurrentlyDisplaying => _locomotionPrompt.ActiveSelf || _snapRotatePrompt.ActiveSelf;

        private void Start()
        {
            ResetPrompts();
        }

        private void OnEnable() => _playerLocomotor.WhenLocomotionEventHandled += UpdateActiveState;
        private void OnDisable() => _playerLocomotor.WhenLocomotionEventHandled -= UpdateActiveState;

        private void UpdateActiveState(LocomotionEvent locomotionEvent, Pose _)
        {
            if (locomotionEvent.IsTeleport()) ResetTeleportPrompt();
            if (locomotionEvent.IsSnapTurn()) ResetSnapPrompt();
        }

        private void Update()
        {
            if (!_shouldCountdown)
            {
                ResetPrompts();
            }
            else
            {
                DisplayPrompt(_locomotionPrompt, ref _locomotionCountdown);
                DisplayPrompt(_snapRotatePrompt, ref _snapTurnCountdown);
            }
        }

        private void DisplayPrompt(ConfigurableActiveState prompt, ref float timer)
        {
            timer -= Time.deltaTime;

            if (!IsPromptCurrentlyDisplaying && timer <= 0f)
            {
                prompt.ActiveSelf = true;
                _countdownMaxTimer += _additionalTime;
            }
        }

        private void ResetPrompts()
        {
            ResetTeleportPrompt();
            ResetSnapPrompt();
        }

        void ResetTeleportPrompt()
        {
            _locomotionPrompt.ActiveSelf = false;
            _locomotionCountdown = _countdownMaxTimer;
        }

        void ResetSnapPrompt()
        {
            _snapRotatePrompt.ActiveSelf = false;
            _snapTurnCountdown = _countdownMaxTimer + 1f;
        }
    }
}
