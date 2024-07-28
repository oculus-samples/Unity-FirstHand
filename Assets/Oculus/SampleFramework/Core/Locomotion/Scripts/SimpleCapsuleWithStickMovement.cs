/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
    public bool EnableLinearMovement = true;
    public bool EnableRotation = true;
    public bool HMDRotatesPlayer = true;
    public bool RotationEitherThumbstick = false;
    public float RotationAngle = 45.0f;
    public float Speed = 0.0f;
    public OVRCameraRig CameraRig;

    private bool ReadyToSnapTurn;
    private Rigidbody _rigidbody;

    public event Action CameraUpdated;
    public event Action PreCharacterMove;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (CameraRig == null) CameraRig = GetComponentInChildren<OVRCameraRig>();
    }

    void Start()
    {

    }

    private void FixedUpdate()
    {
        if (CameraUpdated != null) CameraUpdated();
        if (PreCharacterMove != null) PreCharacterMove();

        if (HMDRotatesPlayer) RotatePlayerToHMD();
        if (EnableLinearMovement) StickMovement();
        if (EnableRotation) SnapTurn();
    }

    void RotatePlayerToHMD()
    {
        Transform root = CameraRig.trackingSpace;
        Transform centerEye = CameraRig.centerEyeAnchor;

        Vector3 prevPos = root.position;
        Quaternion prevRot = root.rotation;

        transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

        root.position = prevPos;
        root.rotation = prevRot;
    }

    void StickMovement()
    {
        Quaternion ort = CameraRig.centerEyeAnchor.rotation;
        Vector3 ortEuler = ort.eulerAngles;
        ortEuler.z = ortEuler.x = 0f;
        ort = Quaternion.Euler(ortEuler);

        Vector3 moveDir = Vector3.zero;
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        moveDir += ort * (primaryAxis.x * Vector3.right);
        moveDir += ort * (primaryAxis.y * Vector3.forward);
        //_rigidbody.MovePosition(_rigidbody.transform.position + moveDir * Speed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + moveDir * Speed * Time.fixedDeltaTime);
    }

    void SnapTurn()
    {
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) ||
            (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)))
        {
            if (ReadyToSnapTurn)
            {
                ReadyToSnapTurn = false;
                transform.RotateAround(CameraRig.centerEyeAnchor.position, Vector3.up, -RotationAngle);
            }
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) ||
            (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
        {
            if (ReadyToSnapTurn)
            {
                ReadyToSnapTurn = false;
                transform.RotateAround(CameraRig.centerEyeAnchor.position, Vector3.up, RotationAngle);
            }
        }
        else
        {
            ReadyToSnapTurn = true;
        }
    }
}
