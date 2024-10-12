// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction;
using UnityEngine;

public class LateLookAtTarget : MonoBehaviour
{
    [SerializeField]
    private Transform _toRotate, _target;

    private void Start()
    {
        this.AssertField(_toRotate, nameof(_toRotate));
        this.AssertField(_target, nameof(_target));
    }

    private void LateUpdate()
    {
        Vector3 dirToTarget = (_target.position - _toRotate.position).normalized;
        _toRotate.LookAt(_toRotate.position - dirToTarget, Vector3.up);
    }
}
