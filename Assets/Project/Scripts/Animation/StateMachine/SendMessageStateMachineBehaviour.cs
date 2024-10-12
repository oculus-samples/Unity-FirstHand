// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Sends a message to the target object when the state is entered
    /// </summary>
    // TODO Use UnityEvent but serialization is tricky
    public class SendMessageStateMachineBehaviour : ExposedStateMachineBehaviour
    {
        [SerializeField]
        ExposedReference<Behaviour> _targetObject;
        [SerializeField]
        string _tragetMethod;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Resolve(_targetObject, animator).SendMessage(_tragetMethod);
        }
    }
}
