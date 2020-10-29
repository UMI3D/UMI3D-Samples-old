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
using umi3d.edk;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float ForwardDelta = 0.5f;
    Vector3 localPos;

    private void Awake()
    {
        localPos = transform.localPosition;
    }

    HashSet<string> users = new HashSet<string>();

    string ToName(UMI3DUser user, string boneId)
    {
        return $"{user.Id()}:{boneId}";
    }

    public void Forward(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        var name = ToName(content.user, content.boneType);
        if(!users.Contains(name))
            users.Add(name);
        if (users.Count > 0) transform.localPosition = localPos + Vector3.right * ForwardDelta;
    }

    public void Backward(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        var name = ToName(content.user, content.boneType);
        if (users.Contains(name))
            users.Remove(name);
        if (users.Count <= 0) ResetPosition();
    }

    public void ResetPosition()
    {
        transform.localPosition = localPos;
    }

}
