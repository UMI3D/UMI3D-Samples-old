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

public class SendProjectionOnEvent : MonoBehaviour
{
    public umi3d.edk.interaction.UMI3DInteractable UMI3DInteractable;

    public void Project(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content) {
        var Projection = UMI3DInteractable.GetProjectTool();
        Dispatch(Projection);
    }

    public void ProjectNonReleasable(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        var Projection = UMI3DInteractable.GetProjectTool(false);
        Dispatch(Projection);
    }

    public void Release(umi3d.edk.interaction.AbstractInteraction.InteractionEventContent content)
    {
        var UnProjection = UMI3DInteractable.GetReleaseTool();
        Dispatch(UnProjection);
    }

    void Dispatch(Operation operation)
    {
        var transaction = new Transaction() { reliable = true, Operations = new List<Operation>() { operation } };
        UMI3DServer.Dispatch(transaction);
    }

}
