// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// ActiveState for StringPropertyHistory
    /// </summary>
    public class StringPropHistoryActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private StringPropertyHistory _stringPropertyHistory;

        [SerializeField]
        private ActiveStateExpectation _canGoBack = ActiveStateExpectation.Any;

        [SerializeField]
        private ActiveStateExpectation _canGoForward = ActiveStateExpectation.Any;

        [SerializeField]
        private ActiveStateExpectation _backContainsValue = ActiveStateExpectation.Any;
        [SerializeField]
        private string _backValue;
        [SerializeField]
        private int _maxBackSearch = -1;

        [SerializeField]
        private ActiveStateExpectation _forwardContainsValue = ActiveStateExpectation.Any;
        [SerializeField]
        private string _forwardValue;
        [SerializeField]
        private int _maxForwardSearch = -1;

        public bool Active
        {
            get
            {
                // check if go back/fwd
                if (!_canGoBack.Matches(_stringPropertyHistory.CanGoBack)) return false;
                if (!_canGoForward.Matches(_stringPropertyHistory.CanGoForward)) return false;

                // check the back stack
                if (_backContainsValue != ActiveStateExpectation.Any)
                {
                    var hasBack = _stringPropertyHistory.FindInBack(_backValue, _maxBackSearch) >= 0;
                    if (!_backContainsValue.Matches(hasBack)) return false;
                }

                // check the fwd stack
                if (_forwardContainsValue != ActiveStateExpectation.Any)
                {
                    var hasForward = _stringPropertyHistory.FindInBack(_forwardValue, _maxForwardSearch) >= 0;
                    if (!_forwardContainsValue.Matches(hasForward)) return false;
                }

                return true;
            }
        }
    }
}
