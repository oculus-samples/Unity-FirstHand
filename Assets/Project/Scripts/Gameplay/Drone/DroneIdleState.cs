// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// State machine behavior that sends events when the state is entered and when it is exited.
    /// This is used by the flight behavior and AI actions to determine when animator state machines are in the default
    /// idle state and new actions can be performed.
    /// </summary>
    public class DroneIdleState : ExposedStateMachineBehaviour
    {
        public Action WhenEnteredIdleState = delegate { };
        public Action WhenExitedIdleState = delegate { };
        public bool IsIdle { get; private set; }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            IsIdle = true;
            WhenEnteredIdleState();
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            IsIdle = false;
            WhenExitedIdleState();
        }
    }
}
