// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction.ComprehensiveSample;
using UnityEngine;

public class SnapZoneHighlight : RendererEffect
{
    [SerializeField] ReferenceActiveState _canDrop;
    [SerializeField] ReferenceActiveState _isHovered;
    [SerializeField] ReferenceActiveState _isSelected;
    [SerializeField] ReferenceActiveState _isOtherCanDrop;
    [SerializeField] private float _offsetScale = 0.01f;

    [SerializeField]
    bool _pulse;

    [SerializeField]
    bool _customLit;

    float _alpha = 1;

    bool _cacheCanDrop;
    bool _cacheIsHovered;
    bool _cacheIsSelected;
    bool _cacheIsOtherCanDrop;

    protected override void Update()
    {
        _cacheCanDrop = _canDrop;
        _cacheIsHovered = _isHovered;
        _cacheIsSelected = _isSelected;
        _cacheIsOtherCanDrop = _isOtherCanDrop;
        base.Update();
    }

    protected override void UpdateMaterial(Material material)
    {
        if (_customLit) return;

        base.UpdateMaterial(material);
        material.EnableKeyword("SNAPZONEHIGHLIGHT_ON");
    }

    protected override void UpdateProperties(MaterialPropertyBlock block)
    {
        base.UpdateProperties(block);
        float targetAlpha = _cacheIsSelected ? 0 : _cacheIsHovered ? 1 : _cacheCanDrop ? 0.6f : _cacheIsOtherCanDrop ? 0.05f : 1;
        _alpha = Mathf.MoveTowards(_alpha, targetAlpha, Time.deltaTime);

        if (_customLit)
        {
            block.SetFloat("_AlphaMultiplier", _alpha);
        }
        else
        {
            block.SetFloat("SnapZoneHighlight_MaxAlpha", _alpha);
            block.SetFloat("SnapZoneHighlight_Offset", _pulse && _cacheCanDrop ? _offsetScale : 0f);
        }
    }
}
