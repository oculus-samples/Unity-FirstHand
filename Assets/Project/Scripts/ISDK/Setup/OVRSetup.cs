// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections;
using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Sets the initial FPS and Foveation level
    /// </summary>
    public class OVRSetup : MonoBehaviour
    {
        IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);

            yield return new WaitUntil(() => OVRManager.display != null);

            Debug.Log("OVRSetup");
            OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.HighTop;
            OVRManager.useDynamicFoveatedRendering = false;
            OVRManager.display.displayFrequency = 72;
        }
    }
}
