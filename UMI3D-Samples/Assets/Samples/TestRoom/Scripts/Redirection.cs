/*
Copyright 2019 - 2021 Inetum

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

using System.Collections;
using System.Collections.Generic;
using umi3d.common;
using umi3d.common.collaboration;
using umi3d.edk.collaboration;
using umi3d.worldController;
using UnityEngine;

public class Redirection : MonoBehaviour
{
    public void SendGateDto()
    {
        Debug.Log("Send Gate");
        var gate = new GateDto()
        {
            gateId = "SuperGateID",
            metaData = UMI3DNetworkingHelper.Write("Data").ToBytes()
        };
        var red = new RedirectionDto()
        {
            gate = gate,
            media = (UMI3DCollaborationServer.Instance.WorldController as StandAloneWorldControllerAPI).ToDto()
        };
        var request = new RedirectionRequest(true, red);
        request.Dispatch();
    }
}
