// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class MaskOffset : Mask
    {
        [SerializeField]
        int _offset = 0;

        [NonSerialized]
        private Material _maskMaterial;

        [NonSerialized]
        private Material _unmaskMaterial;

        protected override void OnDisable()
        {
            StencilMaterial.Remove(_maskMaterial);
            _maskMaterial = null;
            StencilMaterial.Remove(_unmaskMaterial);
            _unmaskMaterial = null;
            base.OnDisable();
        }

        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!MaskEnabled())
                return baseMaterial;

            var rootSortCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
            var stencilDepth = MaskUtilities.GetStencilDepth(transform, rootSortCanvas) + _offset;
            if (stencilDepth >= 8)
            {
                Debug.LogWarning("Attempting to use a stencil mask with depth > 8", gameObject);
                return baseMaterial;
            }

            int desiredStencilBit = 1 << stencilDepth;

            // if we are at the first level...
            // we want to destroy what is there
            if (desiredStencilBit == 1)
            {
                var maskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Replace, CompareFunction.Always, showMaskGraphic ? ColorWriteMask.All : 0);
                StencilMaterial.Remove(_maskMaterial);
                _maskMaterial = maskMaterial;

                var unmaskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, 0);
                StencilMaterial.Remove(_unmaskMaterial);
                _unmaskMaterial = unmaskMaterial;
                graphic.canvasRenderer.popMaterialCount = 1;
                graphic.canvasRenderer.SetPopMaterial(_unmaskMaterial, 0);

                return _maskMaterial;
            }

            //otherwise we need to be a bit smarter and set some read / write masks
            var maskMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit | (desiredStencilBit - 1), StencilOp.Replace, CompareFunction.Equal, showMaskGraphic ? ColorWriteMask.All : 0, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
            StencilMaterial.Remove(_maskMaterial);
            _maskMaterial = maskMaterial2;

            graphic.canvasRenderer.hasPopInstruction = true;
            var unmaskMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit - 1, StencilOp.Replace, CompareFunction.Equal, 0, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
            StencilMaterial.Remove(_unmaskMaterial);
            _unmaskMaterial = unmaskMaterial2;
            graphic.canvasRenderer.popMaterialCount = 1;
            graphic.canvasRenderer.SetPopMaterial(_unmaskMaterial, 0);

            return _maskMaterial;
        }
    }
}
