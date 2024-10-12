// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Pauses Haptics level until user confirms using controller
    /// or bypasses it with hands.
    /// </summary>
    public class HapticsPassthroughContoller : MonoBehaviour
    {
        [SerializeField]
        private OVRPassthroughLayer _passthroughLayer;
        [SerializeField]
        private ProgressTracker _progressTracker;
        [SerializeField]
        private ReferenceActiveState _usingControllers;
        [SerializeField]
        private PauseHandler _pauseHandler;
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private LayerMask _passthroughMask;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);

            if (!_usingControllers)
            {
                _pauseHandler.SetPausedWithState("modal");
                _passthroughLayer.gameObject.SetActive(true);
                var cullingMask = _mainCamera.cullingMask;
                _mainCamera.cullingMask = _passthroughMask;
                _mainCamera.clearFlags = CameraClearFlags.SolidColor;
                _mainCamera.backgroundColor = new Color(0, 0, 0, 0);

                while (_progressTracker.Progress < 2 && !_usingControllers)
                {
                    yield return null;
                }

                if (_usingControllers && _progressTracker.Progress < 2)
                {
                    _progressTracker.SetProgress(2);
                }

                _pauseHandler.ResumeGame();
                _passthroughLayer.gameObject.SetActive(false);
                _mainCamera.clearFlags = CameraClearFlags.Skybox;
                _mainCamera.cullingMask = cullingMask;
            }
        }
    }
}
