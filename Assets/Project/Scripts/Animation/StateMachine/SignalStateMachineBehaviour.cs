// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Add ability to add Signal Emitter to Timeline
    /// </summary>
    public class SignalStateMachineBehaviour : ExposedStateMachineBehaviour
    {
        private static SignalEmitter _emitter;

        [SerializeField]
        ExposedReference<SignalReceiver> _signalReceiver;

        [SerializeField, FormerlySerializedAs("_signal")]
        SignalAsset _enterSignal;
        [SerializeField]
        SignalAsset _updateSignal;
        [SerializeField]
        SignalAsset _exitSignal;
        [SerializeField]
        TimedSignal[] _timedSignals;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            InvokeSignal(animator, _enterSignal);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            InvokeSignal(animator, _exitSignal);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            InvokeSignal(animator, _updateSignal);

            int loop = (int)stateInfo.normalizedTime;
            float time = stateInfo.normalizedTime - loop;
            loop++;

            for (int i = 0; i < _timedSignals.Length; i++)
            {
                if (loop != _timedSignals[i].lastInvokedLoop && time > _timedSignals[i].time)
                {
                    InvokeSignal(animator, _timedSignals[i].signal);
                    _timedSignals[i].lastInvokedLoop = loop;
                }
            }
        }

        private void InvokeSignal(Animator animator, SignalAsset signal)
        {
            if (signal == null) { return; }

            if (TryResolve(_signalReceiver, animator, out var signalReceiver))
            {
                if (_emitter == null)
                {
                    _emitter = CreateInstance<SignalEmitter>();
                }

                _emitter.asset = signal;
                signalReceiver.OnNotify(default, _emitter, null);
            }
        }

        [Serializable]
        struct TimedSignal
        {
            public float time;
            public SignalAsset signal;
            [NonSerialized]
            public int lastInvokedLoop;
        }
    }
}
