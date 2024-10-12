// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Oculus.Interaction.ComprehensiveSample
{

}

[Serializable]
public class OVRScreenFadeBehaviour : PlayableBehaviour
{
    public float alpha;
    public Color color;

    public override void OnPlayableCreate(Playable playable) { }
}
