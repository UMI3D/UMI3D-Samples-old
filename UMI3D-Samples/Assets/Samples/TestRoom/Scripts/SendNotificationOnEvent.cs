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
using static umi3d.common.NotificationDto;

public class SendNotificationOnEvent : MonoBehaviour
{
    public UMI3DNode Node;

    public void Global(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content) {
        var notif = new UMI3DNotification(NotificationPriority.Low, "Global", "This is a global notif", 5f, null, null);
        Dispatch(notif);
    }
    public void Callback(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        var notif = new UMI3DNotification(NotificationPriority.High, "Redirection", "This is a redirection notif", new string[] { "Redirect", "Stay"}, null, null);
        Dispatch(notif);
    }
    public void Local(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        var notif = new UMI3DNotificationOnObject("Global", "This is a global notif", 5f, null, null, Node);
        Dispatch(notif);
    }

    void Dispatch(UMI3DNotification notif)
    {
        var transaction = new Transaction() { reliable = true };
        transaction.AddIfNotNull(notif.GetLoadEntity());
        transaction.Dispatch();
    }

}
