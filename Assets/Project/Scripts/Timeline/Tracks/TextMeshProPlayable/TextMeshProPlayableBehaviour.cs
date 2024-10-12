// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{
    [Serializable]
    public class TextMeshProPlayableBehaviour : PlayableBehaviour
    {
        public string String;

        public override void OnPlayableCreate(Playable playable)
        {

        }
    }
}
