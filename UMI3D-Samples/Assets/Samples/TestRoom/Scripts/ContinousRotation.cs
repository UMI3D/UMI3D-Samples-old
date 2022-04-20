/*
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

    umi3d.edk.UMI3DNodeAnimation _animation;
    umi3d.edk.UMI3DNode node;

    bool clockwise = false;

    private void Start()
    {
        _animation = GetComponent<umi3d.edk.UMI3DNodeAnimation>();
        node = GetComponent<umi3d.edk.UMI3DNode>();

        //ToUMI3DSerializable.ToSerializableVector4()
        SetRotation();
    }

    public void OnChangeRotation()
    {
        var t = new Transaction();
            t.reliable = true;
        t.AddIfNotNull(SetRotation());
        t.Dispatch();
    }

    public void OnRemoveRotation()
    {
        var t = new Transaction();
        t.reliable = true;
        var l = _animation.ObjectAnimationChain.GetValue();
        if (l != null && l.Count > 0)
        {
            int i = l.Count / 2;
            t.AddIfNotNull(_animation.ObjectAnimationChain.RemoveAt(i));
            t.Dispatch();
        }
    }

    public void OnAddRotation()
    {
        var t = new Transaction();
        t.reliable = true;
        var l = _animation.ObjectAnimationChain.GetValue();
        if (l != null )
        {
            int i = l.Count / 2;
            t.AddIfNotNull(_animation.ObjectAnimationChain.Add(
                    new umi3d.edk.UMI3DNodeAnimation.OperationChain()
                    {
                        Operation = new SetEntityProperty()
                        {
                            users = null,
                            entityId = node.Id(),
                            property = UMI3DPropertyKeys.Rotation,
                            value = ToUMI3DSerializable.ToSerializableVector4(Quaternion.identity, null)

                        },
                        progress = _animation.ObjectDuration.GetValue()/2
                    }
                ));
            t.Dispatch();
        }
    }

    public void OnSetRotation()
    {
        var t = new Transaction();
        t.reliable = true;
        var l = _animation.ObjectAnimationChain.GetValue();
        if (l != null)
        {
            int i = l.Count / 2;
            var s = l[i];
            t.AddIfNotNull(_animation.ObjectAnimationChain.SetValue(i,
                    new umi3d.edk.UMI3DNodeAnimation.OperationChain()
                    {
                        Operation = new SetEntityProperty()
                        {
                            users = null,
                            entityId = node.Id(),
                            property = UMI3DPropertyKeys.Rotation,
                            value = ToUMI3DSerializable.ToSerializableVector4(Quaternion.identity, null)

                        },
                        progress = s.progress
                    }
                ));
            t.Dispatch();
        }
    }


    List<Operation> SetRotation()
    {
        clockwise = !clockwise;

        if (lapPerSec <= 0) lapPerSec = 1;
        if (lapSubdivision <= 0) lapSubdivision = 1;
        float totalLapTime = 1f / lapPerSec;


        var op = new List<umi3d.edk.UMI3DNodeAnimation.OperationChain>();
        float curProgress = 0f;
        float deltaProgress = totalLapTime / lapSubdivision;
        Vector3 curRot = Vector3.zero;
        Vector3 deltaRot = axis * 360 / lapSubdivision;
        if (!clockwise)
            deltaRot *= -1;
        Quaternion defaultRot = node.objectRotation.GetValue();

        for (int i = 0; i < lapSubdivision; i++)
        {
            var operation = new SetEntityProperty()
            {
                users = null,
                entityId = node.Id(),
                property = UMI3DPropertyKeys.Rotation,
                value = ToUMI3DSerializable.ToSerializableVector4(Quaternion.Euler(curRot) * defaultRot, null)
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

        var ops = new List<Operation>();

        ops.Add( _animation.ObjectDuration.SetValue(totalLapTime));
        ops.Add( _animation.ObjectAnimationChain.SetValue(op));

        return ops;
    }

}
