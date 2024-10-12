// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Handles different functionlity for the pause menu, such as
    /// Pausing or resuming the game and returning to main menu
    /// </summary>
    public class PauseHandler : MonoBehaviour
    {
        public static bool IsPaused { get; private set; }

        private const OVRInput.Button _start = OVRInput.Button.Start;
        [Header("References")]
        [SerializeField]
        private StringPropertyRef _pauseProperty;
        [SerializeField]
        private ReferenceActiveState _canPause;
        [SerializeField]
        private FollowTransform _poser;

        private IEnumerator Start()
        {
            // If a scene requests permission (mic, scene understanding) then the app will lose focus
            // dont register until enough time has passed for all requests
            yield return new WaitForSeconds(2f);
            OVRManager.InputFocusLost += PauseGame;
        }

        private void OnDestroy()
        {
            OVRManager.InputFocusLost -= PauseGame;
        }

        private void Update()
        {
            if (OVRInput.GetDown(_start) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if (_pauseProperty.Value == "modal") return;

                if (_canPause.HasReference) TogglePauseAfterDelay();
                else TogglePause();
            }
        }

        private void TogglePause() => SetPaused(!IsPaused);
        public void ResumeGame() => SetPaused(false);
        private void PauseGame() => SetPaused(true);

        public void SetPaused(bool pause)
        {
            IsPaused = pause;
            _pauseProperty.Value = pause ? "homepage" : "";
            Time.timeScale = pause ? 0 : 1;
            AudioListener.pause = pause;
            if (IsPaused)
            {
                _poser.UpdatePose(smoothing: false);
            }
        }

        public void SetPausedWithState(string state)
        {
            SetPaused(true);
            _pauseProperty.Value = state;
        }

        public async void LoadScene(string toLoad)
        {
            await MasterLoader.LoadLevel(toLoad);
            await MasterLoader.ActivateAndUnloadOthers(toLoad);
            IsPaused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
        }

        /// <summary>
        /// Used to check the pause gesture was genuinly a pause
        /// In FH Part 1 users could accidentally trigger pause when making a palm facing fist to block
        /// </summary>
        private void TogglePauseAfterDelay()
        {
            StartCoroutine(PauseRoutine());
            IEnumerator PauseRoutine()
            {
                for (float i = 0; i < 0.1f; i += Time.unscaledDeltaTime)
                {
                    if (!_canPause) yield break;
                    yield return null;
                }
                TogglePause();
            }
        }

        public static bool IsTimeStopped => Mathf.Approximately(Time.timeScale, 0) || Mathf.Approximately(Time.deltaTime, 0);
    }
}
