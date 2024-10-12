// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    [RequireComponent(typeof(OVRSceneManager))]
    public class SceneUnderstandingLoader : MonoBehaviour
    {
        public static bool? HasSceneUnderstanding { get; private set; } = null;
        public static event Action WhenHasSceneUnderstanding;
        public static event Action WhenSceneUnderstandingLost;
        private static bool _hasAlreadyFailed = false;
        public static bool RequestSceneCapture { get; private set; } = false;

        private static bool ForceDenySceneCapture { get; } = true;

        [SerializeField]
        ReferenceActiveState _canRequestCapture = ReferenceActiveState.Optional();

        private OVRSceneManager _sceneManager;

        private IEnumerator Start()
        {
            HasSceneUnderstanding = null;
            yield return null;
            StartLoadSequence();
        }

        void SetHasSceneUnderstanding(bool success, bool cacheFailure = true)
        {
            HasSceneUnderstanding = success;

            if (!success && cacheFailure) _hasAlreadyFailed = true;
            WhenHasSceneUnderstanding?.Invoke();
        }

        void Log(string message)
        {
            Debug.Log($"SCENE UNDERSTANDING: {message}");
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && HasSceneUnderstanding.GetValueOrDefault())
            {
                StartCoroutine(FixAnchors());
                IEnumerator FixAnchors()
                {
                    Log("Scene Anchor Fix Start");
                    yield return new WaitForSecondsRealtime(0.1f);

                    List<OVRSceneAnchor> anchors = new List<OVRSceneAnchor>();
                    OVRSceneAnchor.GetSceneAnchors(anchors);
                    if (anchors.Count == 0)
                    {
                        Log("Scene Anchor Fix Found no Anchors");
                        yield break;
                    }

                    var cameraPosition = Camera.main.transform.position;
                    var invalidCount = 0;
                    for (int i = 0; i < anchors.Count; i++)
                    {
                        var vec = anchors[i].transform.position - cameraPosition;
                        if (vec.magnitude < 0.1f * 0.1f)
                        {
                            invalidCount++;
                        }
                    }

                    if (invalidCount > anchors.Count / 2)
                    {
                        Log("Scene Anchor Fix Found Invalid Anchors");
                        HasSceneUnderstanding = null;
                        WhenSceneUnderstandingLost?.Invoke();
                        yield return null;
                        StartLoadSequence(false);
                    }
                }
            }
        }

        void StartLoadSequence(bool cacheFailure = true)
        {
            //Has previously failed and will not try to load again
            if (_hasAlreadyFailed)
            {
                Log("User denied understanding 1");
                SetHasSceneUnderstanding(false);
                return;
            }

            StartCoroutine(LoadSequenceRoutine());
            IEnumerator LoadSequenceRoutine()
            {
                _sceneManager = GetComponent<OVRSceneManager>();

                var waitForSceneLoad = new WaitForSceneLoad(_sceneManager);

                while (waitForSceneLoad.FailedToStart)
                {
                    yield return null;
                    if (!waitForSceneLoad.Retry())
                    {
                        Log("Load Scene Model Failed To Start");
                        SetHasSceneUnderstanding(false, cacheFailure);
                        yield break;
                    }
                }

                yield return waitForSceneLoad;

                if (waitForSceneLoad.Succeeded)
                {
                    Log("Load Scene Model Succeeded");
                    SetHasSceneUnderstanding(true);
                    yield break; // ALL GOOD!!
                }

                Log("Load Scene Model Failed");

                if (_hasAlreadyFailed || ForceDenySceneCapture)
                {
                    Log("User denied understanding 2");
                    SetHasSceneUnderstanding(false, cacheFailure);
                    yield break;
                }

                Log("Should Request Scene Capture");
                //Shouldnt get here
                RequestSceneCapture = true;

                yield return new WaitWhile(() => !_canRequestCapture);

                Log("Request Scene Capture");
                var waitForSceneCapture = new WaitForSceneCapture(_sceneManager);
                if (waitForSceneCapture.FailedToStart)
                {
                    RequestSceneCapture = false;
                    Log("No scene model and could request to create one! Probably editor over link");
                    SetHasSceneUnderstanding(false, cacheFailure);
                    yield break;
                }

                yield return waitForSceneCapture;

                RequestSceneCapture = false;

                if (!waitForSceneCapture.Succeeded)
                {
                    Log("Couldnt capture, user probably hit cancel");
                    SetHasSceneUnderstanding(false, cacheFailure);
                    yield break;
                }

                // user has gone sucesssfully through scene setup
                // so we can load the model again
                Log("Start LoadSceneModel again");

                waitForSceneLoad = new WaitForSceneLoad(_sceneManager);

                if (waitForSceneLoad.FailedToStart)
                {
                    Log("LoadSceneModel Failed To Start 2");
                    SetHasSceneUnderstanding(false, cacheFailure);
                    yield break; // shouldnt happen
                }

                yield return waitForSceneLoad;

                if (!waitForSceneLoad.Succeeded)
                {
                    Log("LoadSceneModel 2 Failed");
                    SetHasSceneUnderstanding(false, cacheFailure);
                    yield break; // shouldnt happen
                }

                Log("LoadSceneModel 2 Succeeded");
                // done!
                SetHasSceneUnderstanding(true);
            }
        }

        /// <summary>
        /// Starts LoadSceneModel and waits till it succeeeds or fails
        /// </summary>
        class WaitForSceneLoad : CustomYieldInstruction
        {
            bool? _loadSucceeded;
            private readonly OVRSceneManager _sceneManager;

            public bool FailedToStart { get; private set; }

            int _attempts = 0;
            private float _startTime;

            public WaitForSceneLoad(OVRSceneManager sceneManager)
            {
                _sceneManager = sceneManager;
                _sceneManager.SceneModelLoadedSuccessfully += SceneModelLoadedSuccessfully;
                _sceneManager.NoSceneModelToLoad += NoSceneModelToLoad;
                Retry();

                _startTime = Time.unscaledTime;
            }

            public bool Retry()
            {
                _attempts++;
                FailedToStart = !_sceneManager.LoadSceneModel();
                if (FailedToStart && _attempts == 30)
                {
                    CleanUp();
                    return false;
                }
                return true;
            }

            public override bool keepWaiting
            {
                get
                {
                    if ((Time.unscaledTime - _startTime) > 5f) SetResult(false);
                    return !_loadSucceeded.HasValue;
                }
            }

            public bool Succeeded => _loadSucceeded.Value;

            void SceneModelLoadedSuccessfully() => SetResult(true);
            void NoSceneModelToLoad() => SetResult(false);
            void SetResult(bool result)
            {
                CleanUp();
                _loadSucceeded = result;
            }

            void CleanUp()
            {
                _sceneManager.SceneModelLoadedSuccessfully -= SceneModelLoadedSuccessfully;
                _sceneManager.NoSceneModelToLoad -= NoSceneModelToLoad;
            }
        }

        class WaitForSceneCapture : CustomYieldInstruction
        {
            OVRSceneManager _sceneManager;
            private bool? _result;
            public bool Succeeded => _result.Value;

            public readonly bool FailedToStart;

            public WaitForSceneCapture(OVRSceneManager sceneManager)
            {
                _sceneManager = sceneManager;
                _sceneManager.SceneCaptureReturnedWithoutError += SceneCaptureReturnedWithoutError;
                _sceneManager.UnexpectedErrorWithSceneCapture += UnexpectedErrorWithSceneCapture;
                FailedToStart = !_sceneManager.RequestSceneCapture();
                if (FailedToStart)
                {
                    CleanUp();
                }
            }

            private void CleanUp()
            {
                _sceneManager.SceneCaptureReturnedWithoutError -= SceneCaptureReturnedWithoutError;
                _sceneManager.UnexpectedErrorWithSceneCapture -= UnexpectedErrorWithSceneCapture;
            }

            private void SceneCaptureReturnedWithoutError() => SetResult(true);
            private void UnexpectedErrorWithSceneCapture() => SetResult(false);

            private void SetResult(bool v)
            {
                CleanUp();
                _result = v;
            }

            public override bool keepWaiting
            {
                get
                {
                    return !_result.HasValue;
                }
            }
        }
    }
}
