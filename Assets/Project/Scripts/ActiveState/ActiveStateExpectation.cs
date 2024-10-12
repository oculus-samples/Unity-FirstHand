// Copyright (c) Meta Platforms, Inc. and affiliates.

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Used to compare a bool or IActiveState to an expected value
    /// </summary>
    public enum ActiveStateExpectation
    {
        True,
        False,
        Any
    }

    /// <summary>
    /// Adds methods to ActiveStateMatch for brevity
    /// </summary>
    public static class ActiveStateExpectationExtensions
    {
        /// <summary>
        /// Returns true if the value matches the expectation or if the expectaion is any
        /// </summary>
        public static bool Matches(this ActiveStateExpectation expectation, bool value)
        {
            return expectation == ActiveStateExpectation.Any || expectation == ActiveStateExpectation.True == value;
        }

        /// <summary>
        /// Returns true if the activeState matches the expectation or if the expectaion is any
        /// </summary>
        public static bool Matches(this ActiveStateExpectation expectation, IActiveState activeState)
        {
            return expectation.Matches(activeState.Active);
        }
    }
}
