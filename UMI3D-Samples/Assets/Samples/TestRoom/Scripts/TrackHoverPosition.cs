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

using inetum.unityUtils;
using System.Collections.Generic;
using umi3d.common;
using umi3d.edk;
using UnityEngine;
using static umi3d.edk.interaction.UMI3DInteractable;

public class TrackHoverPosition : MonoBehaviour
{
    public Transform prefab;
    Dictionary<string, UMI3DModel> trackers = new Dictionary<string, UMI3DModel>();

    string ToName(UMI3DUser user, uint boneType)
    {
        return $"{user.Id()}:{boneType}";
    }

    public void OnHoverEnter(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content) {
        if (!trackers.ContainsKey(ToName(content.user, content.boneType)))
        {
            trackers[ToName(content.user, content.boneType)] = Instantiate(prefab, transform).gameObject.GetOrAddComponent<UMI3DModel>();
            var transaction = new Transaction();
            transaction.reliable = true;
            transaction += trackers[ToName(content.user, content.boneType)].GetLoadEntity();
            transaction.Dispatch();
        }
    }
    public void OnHoverExit(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content) {
        if (trackers.ContainsKey(ToName(content.user, content.boneType)))
        {
            var transaction = new Transaction();

            var del = new DeleteEntity()
            {
                entityId = trackers[ToName(content.user, content.boneType)].Id(),
                users = UMI3DServer.Instance.UserSet()
            };
            transaction.AddIfNotNull(del);
            transaction.reliable = true;
            transaction.Dispatch();

            Destroy(trackers[ToName(content.user, content.boneType)].gameObject);
            trackers.Remove(ToName(content.user, content.boneType));
        }
    }
    public void OnHovered(HoverEventContent content) {
        if (trackers.ContainsKey(ToName(content.user, content.boneType)))
        {
            var t = trackers[ToName(content.user, content.boneType)];
            var transaction = new Transaction();
            var op =  t.objectPosition.SetValue(content.position);
            if (op != null) transaction += op; 
            op = t.objectRotation.SetValue(Quaternion.FromToRotation(transform.up, content.normal));
            if (op != null) transaction += op;
            transaction.reliable = false;
            transaction.Dispatch();
        }
    }
}
