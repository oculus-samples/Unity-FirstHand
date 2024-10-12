// Copyright (c) Meta Platforms, Inc. and affiliates.

/*
 *Copyright(c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
*you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 *Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Threading.Tasks;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// A set of extensions for IInteractor including ForceSelect
    /// Useful for IInteractors that lack that functionality
    /// </summary>
    public static class InteractorExtensions
    {
        public static void ForceSelect<TInteractor, TInteractable>(this TInteractor interactor, TInteractable interactable)
                                    where TInteractor : Interactor<TInteractor, TInteractable>, IInteractor
                                    where TInteractable : Interactable<TInteractor, TInteractable>, IInteractable
        {
            ForceSelectAsync(interactor, interactable);
        }

        public static Task ForceSelectAsync<TInteractor, TInteractable>(this TInteractor interactor, TInteractable interactable)
                                    where TInteractor : Interactor<TInteractor, TInteractable>, IInteractor
                                    where TInteractable : Interactable<TInteractor, TInteractable>, IInteractable
        {
            if (interactor.isActiveAndEnabled == false) { return Task.FromException(new System.Exception("The interactor was disabled")); }

            TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
            InteractorExtensions<TInteractor, TInteractable>.ForceSelect(interactor, interactable, task);
            return task.Task;
        }

        public static void ForceRelease<TInteractor, TInteractable>(this TInteractor interactor)
            where TInteractor : Interactor<TInteractor, TInteractable>, IInteractor
            where TInteractable : Interactable<TInteractor, TInteractable>, IInteractable
        {
            TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
            InteractorExtensions<TInteractor, TInteractable>.ForceSelect(interactor, null, task);
        }
    }

    /// <summary>
    /// A set of extensions for IInteractor including ForceSelect
    /// Useful for IInteractors that lack that functionality
    /// </summary>
    public static class InteractorExtensions<TInteractor, TInteractable>
                                    where TInteractor : Interactor<TInteractor, TInteractable>
                                    where TInteractable : Interactable<TInteractor, TInteractable>
    {
        public static async void ForceSelect(TInteractor interactor, TInteractable interactable, TaskCompletionSource<bool> task = null)
        {
            if (interactor.SelectedInteractable == interactable)
            {
                task?.SetResult(true);
                return;
            }

            if (interactor.HasSelectedInteractable)
            {
                await ForceRelease(interactor);
            }

            if (interactable != null)
            {
                await ForceSelect(interactor, interactable);
            }

            task?.SetResult(true);
        }

        public static async Task ForceRelease(TInteractor interactor)
        {
            if (!interactor.HasSelectedInteractable) return;

            interactor.SetComputeShouldUnselectOverride(() => true);

            await DriveWhile(interactor, () => interactor.HasSelectedInteractable);

            interactor.ClearComputeShouldUnselectOverride();
        }

        private static async Task ForceSelect(TInteractor interactor, TInteractable interactable)
        {
            if (interactor.SelectedInteractable == interactable) return;

            interactor.SetComputeCandidateOverride(() => interactable);
            interactor.SetComputeShouldSelectOverride(() => ReferenceEquals(interactable, interactor.Candidate));

            await DriveWhile(interactor, () => interactor.SelectedInteractable != interactable);

            interactor.ClearComputeCandidateOverride();
            interactor.ClearComputeShouldSelectOverride();
        }

        /// <summary>
        /// Drives the interactor until the predicate becomes true
        /// </summary>
        private static async Task DriveWhile(TInteractor interactor, Func<bool> predicate)
        {
            // if the interactor drives itself we can call Drive as needed, giving ourselves a synchronous API
            if (interactor.IsRootDriver)
            {
                while (predicate()) interactor.Drive();
            }
            else
            {
                // the interactor is probably part of an InteractorGroup,
                // we dont have a clean way of getting a reference to it to Drive it
                // instead we'll start a coroutine and just wait for the InteractorGroup to Update
                await While(predicate);
            }

            Task While(Func<bool> predicate)
            {
                var task = new TaskCompletionSource<bool>();
                interactor.StartCoroutine(Routine());
                IEnumerator Routine()
                {
                    while (predicate()) yield return null;
                    task.SetResult(true);
                }
                return task.Task;
            }
        }
    }
}
