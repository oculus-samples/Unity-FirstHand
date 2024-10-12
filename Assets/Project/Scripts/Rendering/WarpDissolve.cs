// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Renderers = System.Collections.Generic.List<UnityEngine.Renderer>;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Dissolve effect from when an object vanishes and then reappears
    /// </summary>
    public class WarpDissolve : MonoBehaviour, ITimeControl
    {
        [SerializeField]
        private float _warpTime;
        [SerializeField]
        private Material _material;
        [SerializeField]
        private List<RendererFilter> _passFilters = new List<RendererFilter>();

        [SerializeField, Tooltip("Initially the effect was assumed to always " +
            "disappear and reappear (like a teleport) but later the effect was also" +
            "used to materialize things, and the maths was tricky so it was simplified" +
            "but we dont want to break the old timelines")]
        private bool _legacyTimeControl = true;
        [SerializeField]
        private float _controlTrackTime = 0.5f;

        private Renderers _renderers = new Renderers();
        private Renderers _copies = new Renderers();
        private Material _materialCopy;

        private void Awake()
        {
            _materialCopy = new Material(_material);
            GetComponentsInChildren(false, _renderers);

            RendererFilter.RemoveAll(_renderers, _passFilters);
            _renderers.RemoveAll(x => !RendererCopy.CanCopy(x));

            for (int i = 0; i < _renderers.Count; i++)
            {
                _copies.Add(RendererCopy.CreateCopyChildWithMaterial(_renderers[i], _materialCopy));
            }

            SetWarpTime(_warpTime);
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                SetWarpTime(_warpTime);
            }
        }

        void SetWarpTime(float warp)
        {
            if (_materialCopy)
            {
                SetRenderersEnabled(_copies, warp > 0 && warp < 1);
                SetRenderersEnabled(_renderers, warp < 0.5f);
                _materialCopy.SetFloat("_CutOff", Mathf.Abs((warp * 2) - 1));
            }
        }

        static void SetRenderersEnabled(Renderers renderers, bool enabled)
        {
            renderers.ForEach(x => x.forceRenderingOff = !enabled);
        }

        void ITimeControl.SetTime(double time)
        {
            if (!Application.isPlaying) return;

            if (_legacyTimeControl)
            {
                var wt = 1 - Mathf.Abs(((float)(time / _controlTrackTime) / 2f) - 1);
                SetWarpTime(wt);
            }
            else
            {
                float normalizedTime = (float)time / _controlTrackTime;
                float pingPong = Mathf.PingPong(normalizedTime, 1);
                SetWarpTime(pingPong);
            }
        }

        void ITimeControl.OnControlTimeStart()
        {
            //throw new System.NotImplementedException();
        }

        void ITimeControl.OnControlTimeStop()
        {
            if (Application.isPlaying)
            {
                SetWarpTime(0);
            }
        }
    }
}
