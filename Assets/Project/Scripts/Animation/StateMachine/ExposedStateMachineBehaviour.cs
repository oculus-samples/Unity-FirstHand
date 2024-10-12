// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// A StateMachineBehaviour that can have ExposedReferences
    /// </summary>
    public abstract class ExposedStateMachineBehaviour : StateMachineBehaviour
    {
        static SignalEmitter _emitter;

        protected static T Resolve<T>(ExposedReference<T> reference, Animator animator) where T : Object
        {
            return reference.Resolve(animator.GetComponent<IExposedPropertyTable>());
        }

        protected static bool TryResolve<T>(ExposedReference<T> reference, Animator animator, out T result) where T : Object
        {
            result = Resolve(reference, animator);
            return result != null;
        }
    }
}
