// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Checks if expectation matches scene understanding values
    /// </summary>
    public class SceneUnderstandingActveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        ActiveStateExpectation _finishedLoading = ActiveStateExpectation.Any;
        [SerializeField]
        ActiveStateExpectation _hasSceneUnderstanding = ActiveStateExpectation.Any;
        [SerializeField]
        ActiveStateExpectation _requestSceneCapture = ActiveStateExpectation.Any;

        public bool Active =>
            _requestSceneCapture.Matches(SceneUnderstandingLoader.RequestSceneCapture) &&
            _finishedLoading.Matches(SceneUnderstandingLoader.HasSceneUnderstanding.HasValue) &&
            _hasSceneUnderstanding.Matches(SceneUnderstandingLoader.HasSceneUnderstanding.GetValueOrDefault());
    }
}
