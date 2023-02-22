/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
