// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction;
using Oculus.Interaction.ComprehensiveSample;
using Oculus.Interaction.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Duplicated version of HandPhysicsCapsules that doesnt disable the colliders when the visual is turned off
/// </summary>
public class HandPhysicsCapsules : MonoBehaviour
{
    [SerializeField, Interface(typeof(IHandVisual))]
    private MonoBehaviour _handVisual;
    private IHandVisual HandVisual;

    [SerializeField]
    private JointsRadiusFeature _jointsRadiusFeature;

    [Space]
    [SerializeField]
    [Tooltip("Capsules will be generated as triggers")]
    private bool _asTriggers = false;
    [SerializeField]
    [Tooltip("Capsules will be generated in this Layer")]
    private int _useLayer = 0;
    [SerializeField]
    [Tooltip("Capsules reaching this joint will not be generated")]
    private HandFingerJointFlags _mask = HandFingerJointFlags.All;

    private Action _whenCapsulesGenerated = delegate { };
    public event Action WhenCapsulesGenerated
    {
        add
        {
            _whenCapsulesGenerated += value;
            if (_capsulesGenerated)
            {
                value.Invoke();
            }
        }
        remove
        {
            _whenCapsulesGenerated -= value;
        }
    }

    private Transform _rootTransform;
    public Transform RootTransform => _rootTransform;

    private List<BoneCapsule> _capsules;
    public IList<BoneCapsule> Capsules { get; private set; }

    private Rigidbody[] _rigidbodies;
    private bool _capsulesAreActive;
    private bool _capsulesGenerated;

    protected bool _started;

    #region Editor events
    protected virtual void Reset()
    {
        _useLayer = this.gameObject.layer;
        this.TryGetComponent(out HandVisual);
    }
    #endregion

    protected virtual void Awake()
    {
        HandVisual = _handVisual as IHandVisual;
    }

    protected virtual void Start()
    {
        this.BeginStart(ref _started);
        this.AssertField(HandVisual, nameof(HandVisual));
        this.AssertField(_jointsRadiusFeature, nameof(_jointsRadiusFeature));
        GenerateCapsules();
        this.EndStart(ref _started);
    }

    private void GenerateCapsules()
    {
        _rigidbodies = new Rigidbody[(int)HandJointId.HandMaxSkinnable];

        Transform _holder = new GameObject("Capsules").transform;
        _holder.SetParent(transform, false);
        _holder.localPosition = Vector3.zero;
        _holder.localRotation = Quaternion.identity;
        _holder.gameObject.layer = _useLayer;

        int capsulesCount = Constants.NUM_HAND_JOINTS;
        _capsules = new List<BoneCapsule>(capsulesCount);
        Capsules = _capsules.AsReadOnly();
        for (int i = (int)HandJointId.HandThumb0; i < (int)HandJointId.HandEnd; ++i)
        {
            HandJointId currentJoint = (HandJointId)i;
            HandJointId parentJoint = HandJointUtils.JointParentList[i];
            if (parentJoint == HandJointId.Invalid
                || ((1 << (int)currentJoint) & (int)_mask) == 0)
            {
                continue;
            }


            Vector3 boneEnd = HandVisual.GetJointPose(currentJoint, Space.World).position;

            if (!TryGetJointRigidbody(parentJoint, out Rigidbody body))
            {
                Pose parentPose = HandVisual.GetJointPose(parentJoint, Space.World);
                body = CreateDisabledJointRigidbody(parentJoint, _holder, parentPose);
            }

            string boneName = $"{parentJoint}-{currentJoint} CapsuleCollider";
            float boneRadius = _jointsRadiusFeature.GetJointRadius(parentJoint);
            float offset = currentJoint >= HandJointId.HandMaxSkinnable ? -boneRadius
                : parentJoint == HandJointId.HandStart ? boneRadius
                : 0f;

            CapsuleCollider collider = CreateCollider(boneName,
                body.transform, boneEnd, boneRadius, offset);

            BoneCapsule capsule = new BoneCapsule(parentJoint, currentJoint, body, collider);
            _capsules.Add(capsule);

        }

        IgnoreSelfCollisions();
        _capsulesAreActive = false;
        _capsulesGenerated = true;
        _whenCapsulesGenerated.Invoke();
    }

    private void IgnoreSelfCollisions()
    {
        for (int i = 0; i < _capsules.Count; i++)
        {
            for (int j = i + 1; j < _capsules.Count; j++)
            {
                Physics.IgnoreCollision(_capsules[i].CapsuleCollider, _capsules[j].CapsuleCollider);
            }
        }
    }

    private bool TryGetJointRigidbody(HandJointId joint, out Rigidbody body)
    {
        body = _rigidbodies[(int)joint];
        return body != null;
    }

    private Rigidbody CreateDisabledJointRigidbody(HandJointId joint,
        Transform holder, Pose pose)
    {
        string name = $"{joint} Rigidbody";
        Rigidbody rigidbody = new GameObject(name)
            .AddComponent<Rigidbody>();
        rigidbody.mass = 1.0f;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        rigidbody.transform.SetParent(holder, false);
        rigidbody.transform.SetPose(pose);
        rigidbody.gameObject.SetActive(false);
        rigidbody.gameObject.layer = _useLayer;

        _rigidbodies[(int)joint] = rigidbody;
        return rigidbody;
    }

    private CapsuleCollider CreateCollider(string name,
        Transform from, Vector3 to, float radius, float offset)
    {
        CapsuleCollider collider = new GameObject(name)
            .AddComponent<CapsuleCollider>();
        collider.isTrigger = _asTriggers;

        Vector3 boneDirection = to - from.position;
        Quaternion boneRotation = Quaternion.LookRotation(boneDirection);
        float boneLength = boneDirection.magnitude;

        boneLength -= Mathf.Abs(offset);

        collider.radius = radius;
        collider.height = boneLength + radius * 2.0f;
        collider.direction = 2;
        collider.center = Vector3.forward * (boneLength * 0.5f + Mathf.Max(0f, offset));

        Transform capsuleTransform = collider.transform;
        capsuleTransform.SetParent(from, false);
        capsuleTransform.SetPositionAndRotation(from.position, boneRotation);
        collider.gameObject.layer = _useLayer;

        return collider;
    }

    protected virtual void OnEnable()
    {
        if (_started)
        {
            HandVisual.WhenHandVisualUpdated += HandleHandVisualUpdated;
        }
    }

    protected virtual void OnDisable()
    {
        if (_started)
        {
            HandVisual.WhenHandVisualUpdated -= HandleHandVisualUpdated;
            EnableRigidbodies(false);
        }
    }

    private void EnableRigidbodies(bool enable)
    {
        if (_rigidbodies != null
            || enable == _capsulesAreActive)
        {
            return;
        }
        foreach (Rigidbody body in _rigidbodies)
        {
            body.gameObject.SetActive(enable);
        }
        _capsulesAreActive = enable;
    }

    private void HandleHandVisualUpdated()
    {
        _capsulesAreActive = true;

        for (int i = 0; i < (int)HandJointId.HandMaxSkinnable; ++i)
        {
            Rigidbody jointbody = _rigidbodies[i];
            if (jointbody == null)
            {
                continue;
            }
            GameObject jointGO = jointbody.gameObject;

            Pose bonePose = HandVisual.GetJointPose((HandJointId)i, Space.World);
            bool justActivated = false;
            if (!jointGO.activeSelf)
            {
                jointGO.SetActive(true);
                justActivated = true;
            }

            if (!(jointbody.position - bonePose.position).IsMagnitudeLessThan(0.3f))
            {
                //if the hand is moving at about 20m/s something went wrong
                justActivated = true;
            }

            if (_asTriggers)
            {
                jointbody.transform.SetPositionAndRotation(bonePose.position, bonePose.rotation);
            }
            else if (justActivated)
            {
                jointbody.position = bonePose.position;
                jointbody.rotation = bonePose.rotation;
            }
            else
            {
                jointbody.MovePosition(bonePose.position);
                jointbody.MoveRotation(bonePose.rotation);
            }
        }
    }

    #region Inject

    public void InjectAllOVRHandPhysicsCapsules(IHandVisual handVisual,
        bool asTriggers, int useLayer)
    {
        InjectHandVisual(handVisual);
        InjectAsTriggers(asTriggers);
        InjectUseLayer(useLayer);
    }

    public void InjectHandVisual(IHandVisual handVisual)
    {
        _handVisual = handVisual as MonoBehaviour;
        HandVisual = handVisual;
    }

    public void InjectAsTriggers(bool asTriggers)
    {
        _asTriggers = asTriggers;
    }
    public void InjectUseLayer(int useLayer)
    {
        _useLayer = useLayer;
    }

    #endregion
}
