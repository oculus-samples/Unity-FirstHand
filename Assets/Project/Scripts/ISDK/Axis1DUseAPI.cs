// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public class Axis1DUseAPI : MonoBehaviour, IFingerUseAPI
{
    [SerializeField, Interface(typeof(IAxis1D))]
    private MonoBehaviour _axis;
    protected IAxis1D Axis;

    bool _isPressed = false;
    float _value = 0;

    protected virtual void Awake()
    {
        Axis = _axis as IAxis1D;
    }

    void Update()
    {
        var nextValue = Axis.Value();
        if (Mathf.Abs(nextValue - _value) < 0.02f) return;

        _isPressed = nextValue > _value;
        _value = nextValue;
    }

    public float GetFingerUseStrength(HandFinger finger)
    {
        return _isPressed ? 1 : 0;
    }
}
