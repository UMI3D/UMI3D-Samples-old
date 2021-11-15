﻿/*
Copyright 2019 Gfi Informatique

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Generic;
using umi3d.common;
using umi3d.edk;
using UnityEngine;

[RequireComponent(typeof(umi3d.edk.UMI3DNodeAnimation))]
[RequireComponent(typeof(umi3d.edk.UMI3DNode))]
public class ContinousRotation : MonoBehaviour
{
    public float lapPerSec = 1f;
    public Vector3 axis = Vector3.up;
    public int lapSubdivision = 4;

    private void Start()
    {
        umi3d.edk.UMI3DNodeAnimation _animation = GetComponent<umi3d.edk.UMI3DNodeAnimation>();
        umi3d.edk.UMI3DNode node = GetComponent<umi3d.edk.UMI3DNode>();

        //ToUMI3DSerializable.ToSerializableVector4()

        HashSet<UMI3DUser> users = new HashSet<UMI3DUser>(UMI3DEnvironment.GetEntities<UMI3DUser>());

        if (lapPerSec <= 0) lapPerSec = 1;
        if (lapSubdivision <= 0) lapSubdivision = 1;
        float totalLapTime = 1f / lapPerSec;


        var op = new List<umi3d.edk.UMI3DNodeAnimation.OperationChain>();
        float curProgress = 0f;
        float deltaProgress = totalLapTime / lapSubdivision;
        Vector3 curRot = Vector3.zero;
        Vector3 deltaRot = axis*360/lapSubdivision;
        Quaternion defaultRot = node.objectRotation.GetValue();

        for (int i = 0; i<lapSubdivision; i++)
        {
            var operation = new SetEntityProperty()
            {
                users = users,
                entityId = node.Id(),
                property = UMI3DPropertyKeys.Rotation,
                value = ToUMI3DSerializable.ToSerializableVector4(Quaternion.Euler(curRot)* defaultRot, null)
            };
            op.Add(
                new umi3d.edk.UMI3DNodeAnimation.OperationChain()
                {
                    Operation = operation,
                    progress = curProgress
                });
            curProgress += deltaProgress;
            curRot += deltaRot;
        }
        _animation.ObjectDuration.SetValue(totalLapTime);
        _animation.ObjectAnimationChain.SetValue(op);
    }

}
