// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Sets the material for all graphics in children if they have not been specifically set to something
    /// Used to apply the pointable UI material to holographic interfaces
    /// </summary>
    public class UGUIDefaults : MonoBehaviour
    {
        static List<Graphic> _graphics = new List<Graphic>();

        [SerializeField]
        private Material _material;
        [SerializeField]
        private bool _applyInEditor = true;

        private void Awake()
        {
            if (Application.isEditor && !_applyInEditor) { return; }

            GetComponentsInChildren(true, _graphics);
            _graphics.ForEach(x =>
            {
                if (!(x is TMPro.TMP_Text) && (x.material == null || x.material == x.defaultMaterial))
                {
                    x.material = _material;
                }
            });
        }
    }
}
