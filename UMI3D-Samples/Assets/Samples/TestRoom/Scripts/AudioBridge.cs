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

using umi3d.edk;
using umi3d.edk.collaboration;
using UnityEngine;

public class AudioBridge : MonoBehaviour
{
    private void Start()
    {
        UMI3DCollaborationServer.Instance.OnUserJoin.AddListener(newUser);

    }

    void newUser(UMI3DUser user)
    {
        foreach(var userA in UMI3DCollaborationServer.Collaboration.Users)
        {
            if (userA == user) continue;
            if (!UMI3DCollaborationServer.WebRTC.ContainsChannel(userA, user, "Audio"))
            {
                UMI3DCollaborationServer.WebRTC.OpenChannel(userA, user, "Audio", umi3d.common.DataType.Audio, false);
            }

        }
    }

}
