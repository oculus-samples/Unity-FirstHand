// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Playables;

public class TimelineInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayableDirector>().Evaluate();
    }
}
